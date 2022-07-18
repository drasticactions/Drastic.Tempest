using Drastic.Tempest;

namespace SimpleChat.Core
{
    public abstract class SimpleChatMessage
        : Message
    {
        protected SimpleChatMessage(SimpleChatMessageType type)
            : base(SimpleChatProtocol.Instance, (ushort)type)
        {
        }
    }
}
