using System;
using System.Collections.Generic;
using System.Linq;
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
using ChatClient.ServiceChat;

namespace ChatClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IServiceChatCallback
    {
        bool isConnected = false;
        ServiceChatClient client;
        int ID;
        string KeyToSerpent;
        Serpent serpent;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        void ConnectUser()
        {
            
            if (!isConnected)
            {
                serpent = new Serpent();
                KeyToSerpent = Convert.ToBase64String(serpent.GenerateKey());
                client = new ServiceChatClient(new System.ServiceModel.InstanceContext(this));
                ID = client.Connect(tbUserName.Text,KeyToSerpent);
                tbUserName.IsEnabled = false;
                bConnDicon.Content = "Disconnect";
                isConnected = true;
            }
        }

        void DisconnectUser()
        {
            if (isConnected)
            {
                client.Disconnect(ID);
                client = null;
                tbUserName.IsEnabled = true;
                bConnDicon.Content = "Connect";
                isConnected = false;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                DisconnectUser();
            }
            else
            {
                ConnectUser();
            }

        }

        public void MsgCallback(string msg)
        {
            string keyToDecrypt = "";
            if (!msg.Contains("Connect") && !msg.Contains("disconnect"))
            {
                keyToDecrypt = msg.Substring(msg.Length - 44);
                msg = msg.Remove(msg.Length - 44);
                int index = msg.LastIndexOf(' ');
                string k = msg.Substring(index + 1);
                msg = msg.Remove(index);
                k = serpent.Decrypt(k, keyToDecrypt);
                msg += k;
            }
            lbChat.Items.Add(msg);
            lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count-1]);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisconnectUser();
        }

        private void tbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (client!=null)
                {
                    string msg = Converts.BinaryToString(serpent.Encrypt(tbMessage.Text, Convert.FromBase64String(KeyToSerpent)));
                    msg += KeyToSerpent;
                    client.SendMsg(msg, ID);
                    tbMessage.Text = string.Empty;
                }               
            }
        }
    }
}
