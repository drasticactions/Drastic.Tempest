using Microsoft.UI.Dispatching;
using SimpleChat.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleChat
{
    public class ChatWindowViewModel
            : INotifyPropertyChanged
    {
        private readonly ChatClient client;
        private DispatcherQueue thread;

        public ChatWindowViewModel(ChatClient client, DispatcherQueue thread)
        {
            this.client = client;
            this.client.Chat += OnChat;
            this.thread = thread;
            this.sendMessage = new DelegatedCommand<string>(SendMessage, CanSendMessage);
        }

        private bool CanSendMessage(string s)
        {
            return !String.IsNullOrWhiteSpace(s);
        }

        private async void SendMessage(string s)
        {
            await this.client.SendMessage(s);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ChatLog
        {
            get { return this.chatLog.ToString(); }
        }

        private readonly DelegatedCommand<string> sendMessage;
        public ICommand Send
        {
            get { return this.sendMessage; }
        }

        private string message;
        public string Message
        {
            get { return this.message; }
            set
            {
                if (this.message == value)
                    return;

                this.message = value;
                OnPropertyChanged("Message");
            }
        }

        private readonly StringBuilder chatLog = new StringBuilder();
        private readonly Stack<string> previousMessages = new Stack<string>(32);

        private void OnPropertyChanged(string name)
        {
            var changed = PropertyChanged;
            if (changed != null)
                changed(this, new PropertyChangedEventArgs(name));
        }

        private void OnChat(object sender, ChatEventArgs e)
        {
            this.thread.TryEnqueue(() => {
                this.chatLog.AppendLine(String.Format("[{0}] {1}", e.User.Nickname, e.Message));
                OnPropertyChanged("ChatLog");
            });
        }
    }
}
