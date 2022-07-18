namespace SimpleChat.Core
{
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
}
