using Drastic.Tempest.Providers.Network;
using SimpleChat.Core;

namespace SimpleChatMaui;

public partial class SetupPage : ContentPage
{
    private IServiceProvider provider;
    public SetupPage(IServiceProvider provider)
    {
        this.provider = provider;
        InitializeComponent();
    }

    private async void ConnectButton_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(this.NetworkAddressEntry.Text) || string.IsNullOrWhiteSpace(this.PortEntry.Text)) {
            return;
        }

        try
        {
            this.NetworkAddressEntry.IsEnabled = false;
            this.PortEntry.IsEnabled = false;
            this.ConnectButton.IsEnabled = false;
            var ip = this.NetworkAddressEntry.Text.Trim();
            var port = Convert.ToInt32(this.PortEntry.Text.Trim());
            var connection = new NetworkClientConnection(SimpleChatProtocol.Instance);
            var client = new ChatClient(connection);
            var result = await client.ConnectAsync(new Drastic.Tempest.Target(ip, port));
            if (result.Result == Drastic.Tempest.ConnectionResult.Success)
            {
                var chatPage = new MainPage(client, this.provider);
                this.GetParentWindow().Page = chatPage;
            }
            else
            {
                throw new Exception("Failed to connect to server");
            }
        }
        catch (Exception)
        {
            this.NetworkAddressEntry.IsEnabled = true;
            this.PortEntry.IsEnabled = true;
            this.ConnectButton.IsEnabled = true;
            // TODO: Show Error Dialog
        }


    }
}