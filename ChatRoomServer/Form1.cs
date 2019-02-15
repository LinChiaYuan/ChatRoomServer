/**
 *  author  :   Chia Yuan Lin
 * 
 *  email   :   lo919201@gmail.com
 * 
 * **/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatRoomServer
{
    public partial class Form1 : Form
    {
        private IPEndPoint ipep;
        private Socket newsock;
        private bool ServerStatus = false;
        private List<ChatMember> MemberList;
        private List<Message> MessageList;
        private bool TimerLock = false;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Form.CheckForIllegalCrossThreadCalls = false;
            ipep = new IPEndPoint(IPAddress.Any, 8888);
            BtnStart.Text = ServerStatus == false ? "啟動" : "關閉";
            MemberList = new List<ChatMember>();
            MessageList = new List<Message>();
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (ServerStatus)
            {
                newsock.Close();
                BtnStart.Text = "啟動";
                ListBChat.Items.Add("伺服器 關閉");
                ServerStatus = false;
            }
            else
            {
                newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                newsock.Bind(ipep);
                newsock.Listen(10);

                BtnStart.Text = "關閉";
                ListBChat.Items.Add("伺服器 啟動");
                ServerStatus = true;

                Thread t = new Thread(() =>
                {
                    AcceptListener();
                });
                t.IsBackground = true;
                t.Start();
            }
        }

        public void AcceptListener()
        {
            try
            {
                while (true)
                {
                    if (MemberList != null && MemberList.Count < 8 )
                    {
                        Socket client = newsock.Accept();
                        Thread t = new Thread(() =>
                        {
                            TcpListener(client);
                        });
                        t.IsBackground = true;
                        t.Start();
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                ListBChat.Items.Add("伺服器 斷線");
            }
        }

        public void TcpListener(Socket socket)
        {
            IPEndPoint clientep = (IPEndPoint)socket.RemoteEndPoint;
            ChatMember chatMember = new ChatMember("" + clientep, socket);
            ListBChat.Items.Add(chatMember.GetIP() + "連線");
            MemberList.Add(chatMember);

            ReBuildListBOnline();
            try
            {
                SendMsg(chatMember.GetClient(), chatMember.GetIP());

                while (true)
                {
                    byte[] data = new byte[1024];
                    int recv = chatMember.GetClient().Receive(data);
                    if (recv == 0)
                        break;
                    string package = "" + Encoding.UTF8.GetString(data, 0, recv);
                    string[] msg = package.Split(',');
                    if (msg.Length == 2)
                        MessageList.Add(new Message(msg[0], msg[1]));

                }
            }
            catch (Exception e)
            {
            }
            chatMember.SetStatus(false);
            chatMember.GetClient().Close();
           
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            string msg = "Server," + TxtInput.Text;
            foreach (ChatMember member in MemberList)
            {
                byte[] data = Encoding.UTF8.GetBytes(msg);
                member.GetClient().Send(data);
            }
            ListBChat.Items.Add("Server => " + TxtInput.Text);
            TxtInput.Clear();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!TimerLock)
            {
                TimerLock = true;
                if (MemberList != null && MemberList.Count > 0)
                {
                    for (int i = 0; i < MemberList.Count; i++)
                    {
                        if (!MemberList[i].GetStatus())
                        {
                            ListBChat.Items.Add(MemberList[i].GetIP() + "離線");
                            MemberList.RemoveAt(i);
                            ReBuildListBOnline();
                            break;
                        }
                    }
                }
                if (MessageList != null && MessageList.Count > 0)
                {
                    if (MemberList != null && MemberList.Count > 0)
                    {
                        for (int i = 0; i < MemberList.Count; i++)
                        {
                            if (MemberList[i].GetStatus() 
                                && !MemberList[i].GetIP().Equals(MessageList[0].GetIP()))
                            {
                                SendMsg(MemberList[i].GetClient()
                                    , MessageList[0].GetIP() + "," + MessageList[0].GetMsg());
                            }
                        }
                        ListBChat.Items.Add(MessageList[0].GetIP() + " => " + MessageList[0].GetMsg());
                        MessageList.RemoveAt(0);
                    }
                }
                TimerLock = false;
            }
        }

        private void ReBuildListBOnline()
        {
            ListBOnline.Items.Clear();
            foreach (ChatMember member in MemberList)
            {
                ListBOnline.Items.Add(member.GetIP());
            }
        }

        private void SendMsg(Socket s, String msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            s.Send(data);

        }
    }
}
