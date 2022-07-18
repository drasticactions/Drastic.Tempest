using System.ComponentModel;

namespace SimpleChat.Core
{
    public class User
        : INotifyPropertyChanged
    {
        public User(int id, string nickname, UserState state)
        {
            if (nickname == null)
                throw new ArgumentNullException("nickname");

            Id = id;
            Nickname = nickname;
            State = state;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int Id
        {
            get;
            private set;
        }

        public string Nickname
        {
            get;
            set;
        }

        public UserState State
        {
            get;
            set;
        }
    }
}
