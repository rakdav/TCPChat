using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TCPChat
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int port = 8888;
        private IPAddress[] ipServer;
        private TcpListener server;
        private IPAddress localAddr;
        public MainWindow()
        {
            InitializeComponent();
            String host = Dns.GetHostName();
            ipServer = Dns.GetHostEntry(host).AddressList;
            IPServ.ItemsSource = ipServer.ToList();
        }

        private void IPServ_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            localAddr = IPAddress.Parse(IPServ.SelectedItem.ToString());
            Task.Run(() => Listener());
        }
        private async void Listener()
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                server = new TcpListener(localAddr, port);
                server.Start();
                
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();
                    byte[] data = new byte[256];
                    int bytes = stream.Read(data, 0, data.Length);
                    string message = Encoding.UTF8.GetString(data, 0, bytes);
                    builder.Append(message+" "+client.Client.RemoteEndPoint.ToString());
                    Dispatcher.Invoke(() => Message.Items.Add(builder.ToString()));
                    builder.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(IPAddress.Parse(IPClient.Text), port);
                string message = Mes.Text;
                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
                stream.Close();
                client.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        }
    }
}
