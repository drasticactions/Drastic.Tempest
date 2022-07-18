using SimpleChat.Core;

namespace SimpleChatMaui
{
    public class AppDispatcher : IAppDispatcher
    {
        IDispatcher _dispatcher;

        public AppDispatcher(IDispatcher dispatcher)
        {
            this._dispatcher = dispatcher;
        }

        public bool Dispatch(Action action)
        {
            return this._dispatcher.Dispatch(action);
        }
    }
}
