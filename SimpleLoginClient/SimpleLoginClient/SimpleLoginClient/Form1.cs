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
            // 소켓 생성하고 로그인 서버와 통신 준비
            System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
            NetworkStream serverStream = default(NetworkStream);
            clientSocket.Connect("127.0.0.1", 8000);
            serverStream = clientSocket.GetStream();

            // 로그인 폼에서 정보 받아와서, Json 형식으로 변환
            UserInfo userinfo = new UserInfo(LOGIN_RESULT.LOGIN, textBox1.Text, textBox2.Text);
            string output = JsonConvert.SerializeObject(userinfo);

            // Json 형식으로 변환된 정보를 byte 형식으로 전송
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(output);
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            // 로그인 리턴 정보를 받기 위해 새로운 버퍼 생성
            int buffSize = 0;
            byte[] inStream = new byte[LENGTH.MAX_PACKEN_LEN];
            buffSize = clientSocket.ReceiveBufferSize;

            // byte 형식으로 정보 가져와서 Json 형식으로 변환
            serverStream.Read(inStream, 0, buffSize);
            string returndata = System.Text.Encoding.ASCII.GetString(inStream);
            LoginReturn lg_return = JsonConvert.DeserializeObject<LoginReturn>(returndata);

            if (lg_return.task == LOGIN_RESULT.LOGIN)
            {
                // 받아온 로그인 리턴값이 성공이면 소켓을 닫고 로비 폼으로 넘어감
                if (lg_return.result == LOGIN_RESULT.LOGIN_SUCCESS)
                {
                    clientSocket.Close();
                    Form form2 = new Form2(this);
                    form2.ShowDialog();
                }

                // 실패했다면 오류 메세지를 폼의 결과창에 출력
                else
                {
                    if (lg_return.result == LOGIN_RESULT.LOGIN_FAIL_NO_ID)
                    {
                        textBox3.Text = "No such ID!";
                    }
                    else if (lg_return.result == LOGIN_RESULT.LOGIN_FAIL_PASSWORD)
                    {
                        textBox3.Text = "Wrong password!";
                    }
                }
            }

            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 소켓 생성하고 로그인 서버와 통신 준비
            System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
            NetworkStream serverStream = default(NetworkStream);
            clientSocket.Connect("192.168.0.45", 8000);
            serverStream = clientSocket.GetStream();

            // 로그인 폼에서 정보 받아와서, Json 형식으로 변환
            UserInfo userinfo = new UserInfo(LOGIN_RESULT.SIGNUP, textBox1.Text, textBox2.Text);
            string output = JsonConvert.SerializeObject(userinfo);

            // Json 형식으로 변환된 정보를 byte 형식으로 전송
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(output);
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            // 가입 리턴 정보를 받기 위해 새로운 버퍼 생성
            int buffSize = 0;
            byte[] inStream = new byte[LENGTH.MAX_PACKEN_LEN];
            buffSize = clientSocket.ReceiveBufferSize;

            // byte 형식으로 정보 가져와서 Json 형식으로 변환
            serverStream.Read(inStream, 0, buffSize);
            string returndata = System.Text.Encoding.ASCII.GetString(inStream);
            LoginReturn lg_return = JsonConvert.DeserializeObject<LoginReturn>(returndata);

            if (lg_return.task == LOGIN_RESULT.SIGNUP)
            {
                // 성공 여부에 따라 폼의 결과창에 결과 출력
                if (lg_return.result == LOGIN_RESULT.SIGNUP_SUCCESS)
                {
                    textBox3.Text = "Sign up success!";
                }
                else if (lg_return.result == LOGIN_RESULT.SIGNUP_FAIL_ID_EXISTS)
                {
                    textBox3.Text = "Same ID exists!";
                }
            }

            clientSocket.Close();
        }
    }
}
