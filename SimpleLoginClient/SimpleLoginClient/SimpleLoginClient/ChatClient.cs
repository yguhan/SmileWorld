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
        chatInfo usrInfo = new chatInfo();
        //string receiverId;
        string readData = null;

        public ChatClient()
        {
        }

        public ChatClient(Form2 form2, chatInfo userInfo)
        {
            form = form2;
            usrInfo = userInfo;
            chatSocket.Connect("127.0.0.1", 10000);
            Thread ctThread = new Thread(getMessage);
            ctThread.Start();
        }

        public void sendMessage(chatInfo chInfoFromForm2)
        {
            //chInfo.msg = form.msgInput.Text;

            chatInfo chInfo = chInfoFromForm2;
            
            //chInfo.id = usrInfo.id;
           // chInfo.task = task;
            chInfo.msg = form.msgInput.Text;
            if (chInfo.task == "chatTarget" || chInfo.task =="chatAll")
            {
                if (chInfo.task == "chatTarget")
                {
                    foreach (string chatMem in usrInfo.chatList)
                    {
                        chInfo.chatList.Add(usrInfo.id);
                    }
                }
                string output = JsonConvert.SerializeObject(chInfo);
                byte[] outStream = System.Text.Encoding.UTF8.GetBytes(output);
                chatSocket.Send(outStream, SocketFlags.None);
                form.changeTask("chatAll");
            }             
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
                usrInfo = chInfo;
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
                        /*
                    else if (chInfo.task == "lobbyIn")
                    {
                        form.listV();
                        //msg();
                    }
                   */
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

        public chatInfo chInfoFromChat() 
        {
            return usrInfo;
        }



    }
}
