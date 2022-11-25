using System;

namespace DofusBuddy.GameEvents
{
    public class ChatMessageEventArgs : EventArgs
    {
        public string CharacterId { get; set; }

        public string CharacterName { get; set; }

        public ChatMessageEventArgs(string characterId, string characterName)
        {
            CharacterId = characterId;
            CharacterName = characterName;
        }
    }
}
