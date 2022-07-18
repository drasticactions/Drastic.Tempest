namespace SimpleChat.Core
{
    public enum SimpleChatMessageType
        : ushort
    {
        SetNickname = 1,
        Say = 2,
        Chat = 3,
        UserStateChanged = 4,
    }
}
