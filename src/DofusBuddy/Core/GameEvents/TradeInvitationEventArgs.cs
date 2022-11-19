using System;

namespace DofusBuddy.Core.GameEvents
{
    public class TradeInvitationEventArgs : EventArgs
    {
        public string SenderId { get; set; }

        public string ReceiverId { get; set; }

        public TradeInvitationEventArgs(string senderId, string receiverId)
        {
            SenderId = senderId;
            ReceiverId = receiverId;
        }
    }
}
