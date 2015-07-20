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
        Socket lobbySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        List<string> lobbyMem = new List<string>();

        Form1 form1;
        UserInfo userInfo = new UserInfo();
        UserInfo reInfo = new UserInfo();
        static ChatClient chat;
        
         public Form2()
         {
             InitializeComponent();

         }
             
        public Form2(Form1 _form) 
        {
            InitializeComponent();

            form1 = _form;
            lobbySocket.Connect("127.0.0.1", 8001);

            userInfo.id = Form1.ActiveForm.Controls["textBox1"].Text.ToString();
            userInfo.passwd = 0.ToString();
            userInfo.task = "login";
            string output = JsonConvert.SerializeObject(userInfo);
            byte[] outStream = System.Text.Encoding.UTF8.GetBytes(output);
            lobbySocket.Send(outStream, SocketFlags.None);
       
            listView2.View = View.Details;
            listView2.BeginUpdate();

            reInfo.task = "chatAll";
            chat = new ChatClient(this, userInfo.id);
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void sendMsg_Click(object sender, EventArgs e)
        {
            chat.sendMessage();
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

    }
}
