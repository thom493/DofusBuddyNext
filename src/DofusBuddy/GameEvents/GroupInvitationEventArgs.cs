using System;

namespace DofusBuddy.GameEvents
{
    public class GroupInvitationEventArgs : EventArgs
    {
        public string SenderName { get; set; }

        public string ReceiverName { get; set; }

        public GroupInvitationEventArgs(string senderName, string receiverName)
        {
            SenderName = senderName;
            ReceiverName = receiverName;
        }
    }
}
