using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;


namespace SimpleLoginClient
{
    public partial class Form2 : Form
    {
        System.Net.Sockets.TcpClient clientLobbySocket = new System.Net.Sockets.TcpClient();
        NetworkStream serverStream = default(NetworkStream);
        string readData = null;


        List<string> lobbyMem = new List<string>();

        Form1 form1;
        UserInfo userInfo = new UserInfo();
        UserInfo reInfo = new UserInfo();
        
         public Form2()
     
         {
             InitializeComponent();
             

         }
             
        public Form2(Form1 _form) 
        {
            InitializeComponent();

           
            form1 = _form;
            clientLobbySocket.Connect("127.0.0.1", 8001);
            serverStream = clientLobbySocket.GetStream();

            userInfo.id = Form1.ActiveForm.Controls["textBox1"].Text.ToString();
            userInfo.passwd = 0.ToString();
            userInfo.task = "login";

            reInfo.task = "chatAll";

            string output = JsonConvert.SerializeObject(userInfo);
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(output);

            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
       
            Thread ctThread = new Thread(getMessage);
            ctThread.Start();

            listView2.View = View.Details;
            listView2.BeginUpdate();



        }

        
        private void button1_Click(object sender, EventArgs e)
        {

        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            chatInfo chInfo = new chatInfo();
            chInfo.task = reInfo.task;
            chInfo.id = userInfo.id;
            chInfo.msg = textBox5.Text;

            if (chInfo.task == "chatTarget") {
                chInfo.chatList.Add(chInfo.id);
                chInfo.chatList.Add(reInfo.id);
            }
  
            string output = JsonConvert.SerializeObject(chInfo);
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(output);



            reInfo.task = null;
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            reInfo.task = "chatAll";
            /*
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(textBox5.Text + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
            */
             /*
               public class chatInfo
                    {
                        public string task;
                        public string id;
                        public string[] idList;
                        public string msg;
                    }
            */
        }

        private void getMessage()
        {
            while (true)
            {
                serverStream = clientLobbySocket.GetStream();
                int buffSize = 0;
                byte[] inStream = new byte[10025];
                buffSize = clientLobbySocket.ReceiveBufferSize;
                serverStream.Read(inStream, 0, buffSize);


                string returndata = null;
                returndata = System.Text.Encoding.ASCII.GetString(inStream);
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
                        msg();;
                        lobbyMem = chInfo.lobbyList;
                        listV();
                    }
                }
                
            }
        }

       
        private void listV()
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(listV));
            else
            {
                listView2.Items.Clear();
  
                foreach (string mem in lobbyMem)
                {
                    ListViewItem lvi = new ListViewItem(mem);
                    listView2.Items.Add(lvi);
                }

                
                Console.WriteLine(lobbyMem);
            }
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int indexNum = listView2.FocusedItem.Index;
            string itemText = listView2.Items[indexNum].SubItems[0].Text;
            reInfo.task = "chatTarget";
            reInfo.id = itemText;
        }
    

        private void msg()
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(msg));
            else
                textBox4.Text = textBox4.Text + Environment.NewLine + " >> " + readData;
        }

   
 
 
    }
     
  
}
