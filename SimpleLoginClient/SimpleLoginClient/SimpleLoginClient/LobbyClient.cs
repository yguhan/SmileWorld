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
    class LobbyClient
    {
        Socket lobbySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Form2 form;
        lobbyInfo lbInfo = new lobbyInfo();
        //string receiverId;
        string readData = null;

        public LobbyClient()
        {
        }

        public LobbyClient(Form2 form2, lobbyInfo userInfo)
        {
            form = form2;
            lbInfo = userInfo;
            lobbySocket.Connect("127.0.0.1", 8001);
            
            Thread ctThread = new Thread(getMessage);
            ctThread.Start();
        }

        public void sendMessage(lobbyInfo lbInfoFromForm2)
        {
            //lobbyInfo 
            lbInfo = lbInfoFromForm2;
            
            /*
            lobbyInfo chInfo = new lobbyInfo();
            
            chInfo.id = lbInfo.id;
            chInfo.task = lbInfo.task;
            chInfo.msg = form.msgInput.Text;
            */

            string output = JsonConvert.SerializeObject(lbInfo);
            byte[] outStream = System.Text.Encoding.UTF8.GetBytes(output);

            lobbySocket.Send(outStream, SocketFlags.None);


        }

        private void getMessage()
        {
            while (true)
            {
                int buffSize = 0;
                byte[] inStream = new byte[10025];
                buffSize = lobbySocket.ReceiveBufferSize;
                lobbySocket.Receive(inStream);

                string returndata = null;
                returndata = System.Text.Encoding.UTF8.GetString(inStream);
                readData = returndata;

                lobbyInfo getInfo = JsonConvert.DeserializeObject<lobbyInfo>(returndata);
                lbInfo = getInfo;
                if (getInfo != null)
                {
                    if (getInfo.task == "lobbyIn")
                    {
                        readData = "--------------------" + getInfo.id + " Joined ! --------------------";
                        msg();
                        form.listV();
                        //msg();
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

        public lobbyInfo lbInfoFromLobby()
        {
            return lbInfo;
        }



    }
}
