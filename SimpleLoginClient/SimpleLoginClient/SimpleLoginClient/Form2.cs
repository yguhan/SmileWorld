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
        public Form2()
        {
            InitializeComponent();
            /*
            clientLobbySocket.Connect("127.0.0.1", 8001);
            serverStream = clientLobbySocket.GetStream();

            Thread ctThread = new Thread(getMessage);
            ctThread.Start();
        */
        }

        public Form2(Form1 _form) 
        {
            InitializeComponent();
            form1 = _form;
            clientLobbySocket.Connect("192.168.0.45", 8000);
            serverStream = clientLobbySocket.GetStream();

            byte[] outStream = System.Text.Encoding.ASCII.GetBytes("login$" + Form1.ActiveForm.Controls["textBox1"].Text.ToString() + "$" + Form1.ActiveForm.Controls["textBox2"].Text.ToString() + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
            /*
            int buffSize = 0;
            byte[] inStream = new byte[10025];
            buffSize = clientLobbySocket.ReceiveBufferSize;
            */
            Thread ctThread = new Thread(getMessage);
            ctThread.Start();
        }

    
        
        private void button1_Click(object sender, EventArgs e)
        {
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(textBox5.Text + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
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
                readData = "" + returndata;
                msg();
            }
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
