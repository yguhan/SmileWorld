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
        // 클라이언트에서 채팅 / 로비 각 서버에 대한 정보를 가지고 있게 함
        Form1 form1;
        ChatClientInformation chatClientInfo = new ChatClientInformation();
        LobbyClientInformation lobbyClientInfo = new LobbyClientInformation();
        
        static ChatClient chat;
        static LobbyClient lobby;
        
        public Form2()
        {
            InitializeComponent();
        }
             
        public Form2(Form1 _form)
        {
            InitializeComponent();

            form1 = _form;

            // 로그인 폼에서 넘겨받은 id를 클라이언트의 id 정보에 적용
            chatClientInfo.user_id = Form1.ActiveForm.Controls["textBox1"].Text.ToString();
            lobbyClientInfo.user_id = Form1.ActiveForm.Controls["textBox1"].Text.ToString();

            UserData.Text = chatClientInfo.user_id;
            
            // lobby status 변환
            lobbyClientInfo.status = LOBBY_STATUS.LOBBY_IN;
            
            // 새로운 클라이언트를 할당받고 로비 서버에 메시지를 보내 접속을 알림
            lobby = new LobbyClient(this, lobbyClientInfo);
            lobby.sendMessage(lobbyClientInfo);

            // 채팅 모드를 default인 전체 채팅 모드로 하며 새로운 채팅 클라이언트를 할당
            chatClientInfo.chat_target = CHAT_TARGET.CHAT_ALL;
            chat = new ChatClient(this, chatClientInfo);

            // 접속자 리스트 갱신
            listView2.View = View.Details;
            listView2.BeginUpdate();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {

        }
        
        // 게임 종료 버튼 클릭시 호출되는 메소드
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 메시지 전송 버튼 클릭시 호출되는 메소드
        private void sendMsg_Click(object sender, EventArgs e)
        {
            chat.sendMessage(chatClientInfo);
            chatClientInfo.chat_target = CHAT_TARGET.CHAT_ALL;
        }

        // 접속자 리스트 갱신하는 메소드
        public void listV()
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(listV));
            else
            {
                // 전체 리스트 지움
                listView2.Items.Clear();
                   
                // 새로운 접속자 목록을 넣어 리스트 갱신
                foreach (User mem in lobby.lobbyClientInfoFromLobby().lobbyList)
                {
                    ListViewItem lvi = new ListViewItem(mem.user_id);
                    listView2.Items.Add(lvi);
                    Console.WriteLine(mem);
                }
            }
        }

        // 접속자 목록에서 특정한 접속자를 선택(클릭)시 호출되는 메소드
        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 접속자를 선택하면 해당 접속자의 정보를 받아옴
            int indexNum = listView2.FocusedItem.Index;
            string selectedUserName = listView2.Items[indexNum].SubItems[0].Text;

            // 채팅 모드를 귓속말 모드로 바꾸고, 메시지 받는 유저의 목록에 선택된 접속자를 추가 (0번: 본인, 1번: 선택 접속자)
            chatClientInfo.chat_target = CHAT_TARGET.CHAT_WHISPER;
            chatClientInfo.roomUserList.Add(chatClientInfo.user_id);
            chatClientInfo.roomUserList.Add(selectedUserName);
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
