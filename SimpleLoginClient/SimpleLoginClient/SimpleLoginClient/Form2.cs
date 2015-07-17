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

        Form1 form1;
        UserInfo userInfo = new UserInfo();
        
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

            string output = JsonConvert.SerializeObject(userInfo);
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(output);

            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
       
            Thread ctThread = new Thread(getMessage);
            ctThread.Start();
        }

        /*
        private void button1_Click(object sender, EventArgs e)
        {

        }
        */
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            chatInfo chInfo = new chatInfo();
            chInfo.task = "chatAll";
            chInfo.id = userInfo.id;
            chInfo.msg = textBox5.Text;
            string output = JsonConvert.SerializeObject(chInfo);
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(output);

            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
       

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
                string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                //readData = returndata;
                chatInfo chInfo = JsonConvert.DeserializeObject<chatInfo>(returndata);


                //
                //{
                if (chInfo != null)
                {
                    if (chInfo.task == "chatAll" || chInfo.task == "chatTarget")
                    { 
                        readData = chInfo.id + " says : " + chInfo.msg;
                        msg();
                    }
                    if (chInfo.task == "lobbyMem")
                    {
                       // listV(chInfo.id);
                    }
                 /*
                        listView2.Clear();
                        foreach (string member in chInfo.idList)
                        {
                            listView2.Items.Add(member);
                        }
                  */
                    
                }
                    //}
            }
        }

        /*
        private void listV(string Uid)
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker( listV(string) ));
            else
            {
                listView2.View = View.Details;
                listView2.BeginUpdate();

                ListViewItem lvi = new ListViewItem(Uid);
                listView2.Items.Add(lvi);
            }
        }
        */

        private void msg()
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(msg));
            else
                textBox4.Text = textBox4.Text + Environment.NewLine + " >> " + readData;
        }
      /*  
        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            while (true)
            {

                serverStream = clientLobbySocket.GetStream();
                int buffSize = 0;
                byte[] inStream = new byte[10025];
                buffSize = clientLobbySocket.ReceiveBufferSize;
                serverStream.Read(inStream, 0, buffSize);
                string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                //readData = returndata;
                chatInfo chInfo = JsonConvert.DeserializeObject<chatInfo>(returndata);
                if (chInfo != null)
                {
                    if (chInfo.task == "lobbyMem")
                    {
                        listView2.Clear();
                        foreach (string member in chInfo.idList)
                        {
                            listView2.Items.Add(member);
                        }
                    }
                }
            }
        }
        */
    }
     
    /*
    public class chatInfo
    {
        public string task;
        public string id;
        public List<string> idList = new List<string>();
        public string msg;
    }
     */
}
