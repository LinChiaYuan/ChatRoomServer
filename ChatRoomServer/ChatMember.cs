/**
 *  author  :   Chia Yuan Lin
 * 
 *  email   :   lo919201@gmail.com
 * 
 * **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoomServer
{
    class ChatMember
    {
        private string IP = "";
        private bool Status = false;
        private Socket client = null;

        public ChatMember(string ip, Socket s)
        {
            IP = ip;
            Status = true;
            client = s;
        }

        public Socket GetClient()
        {
            return client;
        }

        public string GetIP()
        {
            return IP;
        }

        public bool GetStatus()
        {
            return Status;
        }

        public void SetStatus(bool status)
        {
            Status = status;
        }
    }
}
