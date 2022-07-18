namespace SimpleChatMaui;

public partial class App : Application
{
    IServiceProvider provider;

    public App(IServiceProvider provider)
    {
        InitializeComponent();

        this.provider = provider;

        MainPage = new SetupPage(this.provider);
    }
}
