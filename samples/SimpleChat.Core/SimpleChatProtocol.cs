using Drastic.Tempest;

namespace SimpleChat.Core
{
    public static class SimpleChatProtocol
    {
        public static readonly Protocol Instance = new Protocol(42, 1);

        static SimpleChatProtocol()
        {
            Instance.Discover();
        }
    }
}
