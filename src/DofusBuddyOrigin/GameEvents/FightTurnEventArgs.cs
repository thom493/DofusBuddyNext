using System;

namespace DofusBuddy.GameEvents
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
