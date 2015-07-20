using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace SimpleLoginClient
{
    class ChatClient
    {
        Socket chatSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Form2 form;
        string userId;
        string readData = null;

        public ChatClient()
        {
        }

        public ChatClient(Form2 form2, string Id)
        {
            form = form2;
            userId = Id;
            chatSocket.Connect("127.0.0.1", 10000);
            Thread ctThread = new Thread(getMessage);
            ctThread.Start();
        }

        public void sendMessage()
        {
            chatInfo chInfo = new chatInfo();
            chInfo.task = "chatAll";
            chInfo.id = userId;
            chInfo.msg = form.msgInput.Text;

            if (chInfo.task == "chatTarget")
            {
                chInfo.chatList.Add(chInfo.id);
            }

            string output = JsonConvert.SerializeObject(chInfo);
            byte[] outStream = System.Text.Encoding.UTF8.GetBytes(output);




            chatSocket.Send(outStream, SocketFlags.None);


        }

        private void getMessage()
        {
            while (true)
            {
                int buffSize = 0;
                byte[] inStream = new byte[10025];
                buffSize = chatSocket.ReceiveBufferSize;
                chatSocket.Receive(inStream);

                string returndata = null;
                returndata = System.Text.Encoding.UTF8.GetString(inStream);
                readData = returndata;

                chatInfo chInfo = JsonConvert.DeserializeObject<chatInfo>(returndata);

                if (chInfo != null)
                {
                    if (chInfo.task == "chatAll")
                    {
                        readData = chInfo.id + " says : " + chInfo.msg;
                        msg();
                    }
                    else if (chInfo.task == "chatTarget")
                    {
                        readData = chInfo.chatList[0] + " says to " + chInfo.chatList[1] + " : " + chInfo.msg;
                        msg();
                    }

                    else if (chInfo.task == "lobbyIn")
                    {

                        readData = chInfo.msg;
                        msg();
                    }
                }

            }
        }

        private void msg()
        {
            if (form.InvokeRequired)
                form.Invoke(new MethodInvoker(msg));
            else
                form.chatLog.Text = form.chatLog.Text + Environment.NewLine + " >> " + readData;
        }
    }
}
