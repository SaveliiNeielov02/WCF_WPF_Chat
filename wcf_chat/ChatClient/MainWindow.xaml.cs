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
        public class Chat 
        {
            public string intercolcutor { set;  get; }
            public ListBox chatHistory { get; set; }
        };
        List<Chat> chats = new List<Chat>();
        bool isConnected = false;
        ServiceChatClient client;
        string openedChat;
        List<Button> buttons = new List<Button>();
        int ID;
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
                client = new ServiceChatClient(new System.ServiceModel.InstanceContext(this));
                ID = client.Connect(tbUserName.Text);
                tbUserName.IsEnabled = false;
                bConnDicon.Content = "Отключиться";
                isConnected = true;
                client.InstallChats();
            }
        }

        void DisconnectUser()
        {
            if (isConnected)
            {
                bConnDicon.Content = "Подключиться";
                isConnected = false;
                tbUserName.IsEnabled = true;
                client.Disconnect(ID);
                client = null;
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

        public void MsgCallback(string msg, string sender, string recipient)
        {
            void chatView(string chatWith) 
            {
                foreach (var user in chats)
                {
                    if (user.intercolcutor == chatWith)
                    {
                        foreach (var message in user.chatHistory.Items)
                        {
                            lbChat.Items.Add((string)message);
                        }
                    }
                }
            }
           
                if (chats.Select(_ => _.intercolcutor).Contains(sender))
                {
                foreach (var el in chats)
                {
                    if (el.intercolcutor == sender)
                    {
                        lbChat.Items.Clear();
                        el.chatHistory.Items.Add(msg);
                        chatView(openedChat);
                        tbMessage.Clear();
                    }
                }
                }
                else if (chats.Select(_ => _.intercolcutor).Contains(recipient))
                {
                    foreach (var el in chats)
                    {
                        if (el.intercolcutor == recipient)
                        {
                            lbChat.Items.Clear();
                            el.chatHistory.Items.Add(msg);
                            chatView(openedChat);
                            tbMessage.Clear();
                        }
                    }
                }
            
            
        }
        public void GetChats(string userName) 
        {
                Button but = new Button();
                but.Content = userName;
                but.Width = BB1.Width-15;
                but.Click += ChatBtnClick;
            if (!buttons.Select(_ => _.Content).Contains(but.Content))
            {
                BB1.Items.Add(but);
                buttons.Add(but);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisconnectUser();
        }

        private void tbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (client != null)
                {
                    foreach (var but in buttons) 
                    {
                        if (!but.IsEnabled) 
                        {
                            client.SendMsg(tbMessage.Text, ID, but.Content.ToString());
                            tbMessage.Clear();
                        }
                    }
                }
            }
        }

        private void ChatBtnClick(object sender, RoutedEventArgs e)
        {
                openedChat = ((Button)sender).Content.ToString();
                foreach (var but in buttons)
                {
                    but.IsEnabled = true;
                }
                if (!chats.Select(_ => _.intercolcutor).Contains(((Button)sender).Content))
                {
                
                    lbChat.Items.Clear();
                    Chat chat = new Chat()
                    {
                        intercolcutor = (((Button)sender).Content).ToString(),
                        chatHistory = new ListBox()
                    };
                    chat.chatHistory.Width = lbChat.Width;
                    chat.chatHistory.Height = lbChat.Height;
                    chats.Add(chat);
                    chats.Last().chatHistory.Items.Add("Это начало вашего диалога с " + (((Button)sender).Content).ToString());
                    ((Button)sender).IsEnabled = false;
                    lbChat.Items.Add(chats.Last().chatHistory.Items[0]);
                    int secondUserId = client.GetIdByName((((Button)sender).Content).ToString());
                    string firstUserName = client.GetNameById(ID);
                    client.SetDialog(secondUserId, firstUserName);
                
                
                }
                else
                {
                    ((Button)sender).IsEnabled = false;
                    foreach (var el in chats)
                    {
                    if (el.intercolcutor == (((Button)sender).Content).ToString())
                    {
                        lbChat.Items.Clear();
                        foreach (var msg in el.chatHistory.Items)
                        {
                            lbChat.Items.Add((string)(msg));
                        }
                    }
                    }
                }
        }
        public void SetNewDialog(string withUser) 
        {
            if (!chats.Select(_ => _.intercolcutor).Contains(withUser))
            {
                Chat newChat = new Chat()
                {
                    intercolcutor = withUser,
                    chatHistory = new ListBox()
                };
                chats.Add(newChat);
                chats.Last().chatHistory.Items.Add("Это начало вашего общения с " + withUser);
                
                
            }
        }
        
    }
}
