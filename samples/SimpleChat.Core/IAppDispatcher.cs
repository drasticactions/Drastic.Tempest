namespace SimpleChat.Core
{
    public interface IAppDispatcher
    {
        bool Dispatch(Action action);
    }
}
