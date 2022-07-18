using SimpleChat.Core;

namespace SimpleChatMaui;

public partial class MainPage : ContentPage
{
    int count = 0;

    private ChatViewModel chatViewModel;

    public MainPage(ChatClient client, IServiceProvider provider)
    {
        InitializeComponent();
        this.BindingContext = this.chatViewModel = new ChatViewModel(client, provider);
    }
}

