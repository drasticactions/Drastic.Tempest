using Drastic.Tempest;
using Drastic.Tempest.Providers.Network;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SimpleChat.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimpleChat
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private TempestServer server;
        private NetworkClientConnection clientConnection;
        private NetworkConnectionProvider provider;
        private Target target;

        public MainWindow()
        {
            this.InitializeComponent();
            this.target = new Target("127.0.0.1", 8888);
            this.provider = new NetworkConnectionProvider(SimpleChatProtocol.Instance, this.target, 100);
            this.clientConnection = new NetworkClientConnection(SimpleChatProtocol.Instance);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.server is null)
            {
                this.server = new ChatServer(this.provider);
                this.server.ConnectionMade += Server_ConnectionMade;
            }

            if (this.server.IsRunning)
            {
                this.server.Stop();
                this.StartServerButton.Content = "Start Server";
            }
            else
            {
                this.server.Start();
                this.StartServerButton.Content = "Stop Server";
            }
        }

        private void Server_ConnectionMade(object sender, ConnectionMadeEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.ToString());
        }

        private async void JoinServerButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.server is null || !this.server.IsRunning)
            {
                return;
            }

            var connection = new NetworkClientConnection(SimpleChatProtocol.Instance);
            var client = new ChatClient(connection);
            var result = await client.ConnectAsync(new Target(this.target.Hostname, this.target.Port));

            var window = new ChatWindow(client);
            window.Activate();
        }
    }
}
