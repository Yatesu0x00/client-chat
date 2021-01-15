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
using System.Net;
using System.Net.Sockets;
using System.Windows.Threading;
using System.Threading;
namespace ClientChat

{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        myDialog dlg;
        public Socket s { get; set; }
        public IPAddress host { get; set; }
        Thread thread_1;

        private bool connectStatus = false;
        private byte[] bytes;
        private int zustand;
        private string data;

        public MainWindow()
        {
            InitializeComponent();

            tbChatShowMessages.IsReadOnly = true;
            defaultBtns();
            disconnect.IsEnabled = false;
        }
        
        private void defaultBtns() 
        {
            tbNickname.IsEnabled = false;
            btnAnmelden.IsEnabled = false;
            btnAbmelden.IsEnabled = false;
            tbChatMessage.IsEnabled = false;
            btnAbsenden.IsEnabled = false;
            //disconnect.IsEnabled = false;
        }

        private void ThreadFunction()
        {
            data = "";
            bytes = new byte[10000];

            while(connectStatus == true)
            {
                try
                {
                    if (zustand == 1)
                    {
                        Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                        {
                            data = tbNickname.Text + "+";
                        }));

                        s.Send(Encoding.ASCII.GetBytes(data));
                        s.Receive(bytes);
                        zustand = 0;
                    }
                    else if (zustand == 2)
                    {
                        Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                        {
                            data = tbNickname.Text + "-";
                        }));

                        s.Send(Encoding.ASCII.GetBytes(data));
                        s.Receive(bytes);

                        zustand = 0;
                    }
                    else if (zustand == 3)
                    {
                        Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                        {
                            data = tbNickname.Text + "*";
                        }));

                        s.Send(Encoding.ASCII.GetBytes(data));
                        s.Receive(bytes);

                        zustand = 0;
                    }
                    //else if (zustand == 4)
                    //{
                    //    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    //    {
                    //        data = tbNickname.Text + "#";
                    //    }));

                    //    s.Send(Encoding.ASCII.GetBytes(data));
                    //    s.Receive(bytes);

                    //    zustand = 0;
                    //}
                    else
                    {
                        if (s.Available > 0)
                        {
                            s.Receive(bytes);
                        }
                    }
                    Thread.Sleep(50);
                }
                catch { }
            }
        }

        private void connect_Click(object sender, RoutedEventArgs e)
        {
            dlg = new myDialog();
            dlg.ShowDialog();

            if (dlg.DialogResult == true)
            {
                disconnect.IsEnabled = true;
                connect.IsEnabled = false;
                tbNickname.IsEnabled = true;
                btnAnmelden.IsEnabled = true;

                host = IPAddress.Parse(dlg.ipAddress);

                s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s.Connect(host, dlg.port);

                connectStatus = true;

                thread_1 = new Thread(ThreadFunction);
                thread_1.Start();
            }
        }

        private void disconnect_Click(object sender, RoutedEventArgs e)
        {
            connectStatus = false;
            zustand = 3;
            s.Close();
            connect.IsEnabled = true;
            disconnect.IsEnabled = false;
        }

        private void btnAnmelden_Click(object sender, RoutedEventArgs e)
        {
            zustand = 1;
            tbNickname.IsEnabled = false;
            btnAnmelden.IsEnabled = false;
            btnAbmelden.IsEnabled = true;
            tbChatShowMessages.IsEnabled = true;
            tbChatMessage.IsEnabled = true;
            btnAbsenden.IsEnabled = true;
        }

        private void btnAbmelden_Click(object sender, RoutedEventArgs e)
        {
            zustand = 2;
            tbNickname.IsEnabled = true;
            btnAnmelden.IsEnabled = true;
            btnAbmelden.IsEnabled = false;
            tbChatShowMessages.IsEnabled = false;
            tbChatMessage.IsEnabled = false;
            btnAbsenden.IsEnabled = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                thread_1.Abort();
            }
            catch { }
        }
    }
}
