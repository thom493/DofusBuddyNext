using System;

namespace DofusBuddy.Core.GameEvents
{
    public class FightTurnEventArgs : EventArgs
    {
        public string CharacterId { get; set; }

        public FightTurnEventArgs(string characterId)
        {
            CharacterId = characterId;
        }
    }
}
