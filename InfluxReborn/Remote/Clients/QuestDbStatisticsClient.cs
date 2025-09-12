﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;
using QuestDB;
using QuestDB.Enums;
using QuestDB.Senders;
using QuestDB.Utils;

namespace InfluxReborn.Remote.Clients;

internal sealed class QuestDbStatisticsClient : BaseStatisticsClient
{
    private readonly IPluginLog _pluginLog;
    private readonly Configuration.ServerConfiguration _serverConfiguration;

    private readonly ISender? _sender;

    public QuestDbStatisticsClient(
        IChatGui chatGui,
        Configuration configuration,
        GameData gameData,
        IClientState clientState,
        IPluginLog pluginLog)
        : base(chatGui, configuration, gameData, clientState, pluginLog)
    {
        _pluginLog = pluginLog;
        _serverConfiguration = configuration.Server.Copy();

        if (Enabled && Uri.TryCreate(_serverConfiguration.Server, UriKind.Absolute, out Uri? uri))
        {
            ProtocolType protocol = uri.Scheme switch
            {
                "https" => ProtocolType.https,
                "http" => ProtocolType.http,
                "tcps" => ProtocolType.tcps,
                "tcp" => ProtocolType.tcp,
                _ => throw new NotSupportedException($"Unsupported protocol: {uri.Scheme}")
            };
            int port = uri.Port;
            if (port == -1)
            {
                port = protocol switch
                {
                    ProtocolType.https => 443,
                    ProtocolType.http => 80,
                    ProtocolType.tcp or ProtocolType.tcps => 9009,
                    _ => throw new NotSupportedException($"Unsupported protocol: {protocol}")
                };
            }

            _sender = Sender.New(new SenderOptions
            {
                addr = $"{uri.Host}:{port}",
                protocol = protocol,
                username = _serverConfiguration.Username,
                password = _serverConfiguration.Password,
            });
        }
    }

    public override bool Enabled
        => _serverConfiguration is { Enabled: true, Type: Configuration.ERemoteType.QuestDb } &&
           !string.IsNullOrEmpty(_serverConfiguration.Server) &&
           Uri.TryCreate(_serverConfiguration.Server, UriKind.Absolute, out _);

    protected override async Task SaveStatistics(List<StatisticsValues> values)
    {
        if (_sender == null)
            return;

        try
        {
            var tables = values.GroupBy(x => x.Name);
            foreach (var table in tables)
            {
                string tableName = table.Key;
                if (!string.IsNullOrEmpty(_serverConfiguration.TablePrefix))
                    tableName = $"{_serverConfiguration.TablePrefix}_{tableName}";
                _sender.Transaction(tableName);

                foreach (var value in table)
                {
                    var sender = _sender;
                    foreach (var tag in value.Tags)
                        sender = sender.Symbol(tag.Key, tag.Value);

                    foreach (var field in value.BoolFields)
                        sender = sender.Column(field.Key, field.Value);

                    foreach (var field in value.LongFields)
                        sender = sender.Column(field.Key, field.Value);

                    foreach (var field in value.UlongFields)
                        sender = sender.Column(field.Key, (long)field.Value);

                    await sender.AtAsync(value.Time).ConfigureAwait(false);
                }

                await _sender.CommitAsync().ConfigureAwait(false);

            }

            _pluginLog.Verbose($"QuestDB: Sent {values.Count} data points to server");
        }
        catch (Exception)
        {
            if (_sender.WithinTransaction)
                _sender.Rollback();
            throw;
        }
    }

    public override async Task<(bool Success, string Error)> TestConnection(CancellationToken cancellationToken)
    {
        string tablePrefix = _serverConfiguration.TablePrefix;
        if (_sender == null)
            return (false, "InfluxDB client is not initialized");

        try
        {
            _sender.Transaction("fakeTable");
            _sender.Column("key", "value");
            await _sender.AtAsync(DateTime.UtcNow, cancellationToken).ConfigureAwait(false);
            await _sender.CommitAsync(cancellationToken).ConfigureAwait(false);

        }
        catch (Exception e)
        {
            _pluginLog.Error(e, "Unable to connect to QuestDB server");
            return (false, "Failed to connect to QuestDB server");
        }

        cancellationToken.ThrowIfCancellationRequested();

        return (true, string.Empty);
    }

    public override void Dispose()
    {
        _sender?.Dispose();
    }
}
