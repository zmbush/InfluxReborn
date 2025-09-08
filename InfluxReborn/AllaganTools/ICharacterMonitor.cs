using System.Collections.Generic;

namespace InfluxReborn.AllaganTools;

internal interface ICharacterMonitor
{
    IEnumerable<Character> PlayerCharacters { get; }
    IEnumerable<Character> All { get; }
}
