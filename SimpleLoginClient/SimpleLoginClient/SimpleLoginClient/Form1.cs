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
using Newtonsoft.Json;

namespace SimpleLoginClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
            NetworkStream serverStream = default(NetworkStream);
            clientSocket.Connect("127.0.0.1", 8000);
            serverStream = clientSocket.GetStream();

            UserInfo userinfo = new UserInfo();
            userinfo.task = "login";
            userinfo.id = textBox1.Text;
            userinfo.passwd = textBox2.Text;
            string output = JsonConvert.SerializeObject(userinfo);

            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(output);
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            int buffSize = 0;
            byte[] inStream = new byte[10025];
            buffSize = clientSocket.ReceiveBufferSize;
            serverStream.Read(inStream, 0, buffSize);
            string returndata = System.Text.Encoding.ASCII.GetString(inStream);
            textBox3.Text = returndata;

            if (String.Compare("logged in!", returndata) == 0)
            {
                clientSocket.Close();
                Form form2 = new Form2(this);
                form2.ShowDialog();
            }
            this.Close();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
            NetworkStream serverStream = default(NetworkStream);
            clientSocket.Connect("192.168.0.45", 8000);
            serverStream = clientSocket.GetStream();

            UserInfo userinfo = new UserInfo();
            userinfo.task = "signup";
            userinfo.id = textBox1.Text;
            userinfo.passwd = textBox2.Text;
            string output = JsonConvert.SerializeObject(userinfo);

            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(output);
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
            int buffSize = 0;
            byte[] inStream = new byte[10025];
            buffSize = clientSocket.ReceiveBufferSize;
            serverStream.Read(inStream, 0, buffSize);
            string returndata = System.Text.Encoding.ASCII.GetString(inStream);
            textBox3.Text = returndata;
            clientSocket.Close();
        }
    }

    public class UserInfo
    {
        public string task;
        public string id;
        public string passwd;
    }
   
    public class chatInfo
    {
        public string task;
        public string id;
        public List<string> chatList = new List<string>();
        public List<string> lobbyList = new List<string>();
        public string msg;
    }
  
}
