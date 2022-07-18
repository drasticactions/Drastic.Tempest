namespace SimpleChat.Core
{
    public class ChatEventArgs
        : UserEventArgs
    {
        public ChatEventArgs(User user, string message)
            : base(user)
        {
            Message = message;
        }

        public string Message
        {
            get;
            private set;
        }
    }
}
