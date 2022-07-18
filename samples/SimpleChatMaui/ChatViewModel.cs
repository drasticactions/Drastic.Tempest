using SimpleChat.Core;
using System.Collections.ObjectModel;

namespace SimpleChatMaui
{
    public class ChatViewModel : BaseViewModel
    {
        private readonly ChatClient client;
        private AsyncCommand<string> sendMessageAsync;
        private AsyncCommand startServerAsync;
        private AsyncCommand stopServerAsync;
        private string nickname;
        private ObservableCollection<string> chatLog;

        public ChatViewModel(ChatClient client, IServiceProvider services)
            : base(services)
        {
            // TODO: Properly set up user and register them.
            this.nickname = "Test";
            this.chatLog = new ObservableCollection<string>();
            this.client = client;
            this.client.Chat += Client_Chat;
            this.client.UserStateChanged += Client_UserStateChanged;
            this.client.Connected += Client_Connected;
            this.client.Disconnected += Client_Disconnected;
            this.stopServerAsync = new AsyncCommand(this.StopServerAsync, () => this.client.IsConnected, this.Dispatcher, this.ErrorHandler);
            this.startServerAsync = new AsyncCommand(this.StartServerAsync, () => !this.client.IsConnected, this.Dispatcher, this.ErrorHandler);

            this.sendMessageAsync = new AsyncCommand<string>(this.SendMessageAsync, (value) => !string.IsNullOrWhiteSpace(value), this.ErrorHandler);
        }

        public ObservableCollection<string> ChatLog => this.chatLog;

        public AsyncCommand StartServerAsyncCommand => this.startServerAsync;

        public AsyncCommand StopServerAsyncCommand => this.stopServerAsync;

        public AsyncCommand<string> SendMessageCommandAsync => this.sendMessageAsync;

        private async Task SendMessageAsync(string message)
        {
            var result = await this.client.SendMessage(new ChatMessage() { Nickname = this.nickname, Message = message });
            // TODO: If failed, send error message.
        }

        public override void RaiseCanExecuteChanged()
        {
            base.RaiseCanExecuteChanged();
            this.StartServerAsyncCommand.RaiseCanExecuteChanged();
            this.StopServerAsyncCommand.RaiseCanExecuteChanged();
        }

        private Task StartServerAsync()
        {
            this.RaiseCanExecuteChanged();
            return Task.CompletedTask;
        }

        private Task StopServerAsync()
        {
            this.RaiseCanExecuteChanged();
            return Task.CompletedTask;
        }

        private void Client_Disconnected(object sender, Drastic.Tempest.ClientDisconnectedEventArgs e)
        {
        }

        private void Client_Connected(object sender, Drastic.Tempest.ClientConnectionEventArgs e)
        {
        }

        private void Client_UserStateChanged(object sender, UserEventArgs e)
        {
        }

        private void Client_Chat(object sender, ChatEventArgs e)
        {
            // TODO: Use User and Message in a bounded view.
            this.Dispatcher.Dispatch(() => this.ChatLog.Add(String.Format("[{0}] {1}", e.User.Nickname, e.Message)));
        }
    }
}
