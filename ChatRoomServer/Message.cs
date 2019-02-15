/**
 *  author  :   Chia Yuan Lin
 * 
 *  email   :   lo919201@gmail.com
 * 
 * **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoomServer
{
    class Message
    {
        private string IP = "";
        private string Msg = "";

        public Message(string ip, string msg)
        {
            IP = ip;
            Msg = msg;
        }

        public string GetIP()
        {
            return IP;
        }

        public string GetMsg()
        {
            return Msg;
        }
    }
}
