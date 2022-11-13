using System;

namespace DofusBuddy.Core.GameEvents
{
    public class ChatMessageEventArgs : EventArgs
    {
        public string CharacterId { get; set; }

        public string Message { get; set; }

        public ChatMessageEventArgs(string characterId, string message)
        {
            CharacterId = characterId;
            Message = message;
        }
    }
}
