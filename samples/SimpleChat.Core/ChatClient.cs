using Drastic.Tempest;
using System.Collections.ObjectModel;

namespace SimpleChat.Core
{
    public class ChatClient
               : TempestClient
    {
        public ChatClient(IClientConnection connection)
            : base(connection, MessageTypes.Reliable)
        {
            this.RegisterMessageHandler<UserStateChangedMessage>(OnUserStateChangedMessage);
            this.RegisterMessageHandler<ChatMessage>(OnSayMessage);
        }

        public event EventHandler<ChatEventArgs> Chat;
        public event EventHandler<UserEventArgs> UserStateChanged;

        public IEnumerable<User> Users
        {
            get { return this.users; }
        }

        public Task<bool> SendMessage(ChatMessage chat)
        {
            return Connection.SendAsync(chat);
        }

        private readonly ObservableCollection<User> users = new ObservableCollection<User>();

        private void OnUserStateChangedMessage(MessageEventArgs<UserStateChangedMessage> e)
        {
        }

        private void OnSayMessage(MessageEventArgs<ChatMessage> obj)
        {
            this.Chat?.Invoke(this, new ChatEventArgs(new User(0, obj.Message.Nickname, UserState.Present), obj.Message.Message));
        }
    }
}
