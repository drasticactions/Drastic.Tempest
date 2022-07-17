using Drastic.Tempest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace SimpleChat.Client
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

        public Task<bool> SendMessage(string message)
        {
            return Connection.SendAsync(new ChatMessage { Nickname = "Test", Message = message });
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

    public class UserEventArgs
        : EventArgs
    {
        public UserEventArgs(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            User = user;
        }

        public User User
        {
            get;
            private set;
        }
    }

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
