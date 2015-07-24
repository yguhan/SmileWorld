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
        Socket lobbySocket;
        Form2 form;
        LobbyClientInformation lobbyClientInfo;
        string readData = null;

        public LobbyClient()
        {

        }

        public LobbyClient(Form2 form2, LobbyClientInformation userInfo)
        {
            // LobbyClient 멤버변수 할당
            lobbySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            form = form2;
            lobbyClientInfo = new LobbyClientInformation();
            lobbyClientInfo = userInfo;

            //socket을 로비 서버에 연결, getMessage 스레드 시작
            lobbySocket.Connect("127.0.0.1", 8001);
            Thread ctThread = new Thread(getMessage);
            ctThread.Start();
        }

        // 현재 로그인한 클라이언트의 정보를 로비 서버에 전송해 주는 메소드
        public void sendMessage(LobbyClientInformation lobbyClientInfoFromForm2)
        {
            User tempUser = new User(lobbyClientInfoFromForm2.status, lobbyClientInfoFromForm2.user_id);

            // 폼으로부터 전달받은 정보를 클라이언트에 저장
            lobbyClientInfo = lobbyClientInfoFromForm2;
            
            // 데이터를 Json 형식으로 변환하여 소켓을 통해 로비 서버로 전송
            string output = JsonConvert.SerializeObject(tempUser);
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(output);

            lobbySocket.Send(outStream, SocketFlags.None);
        }

        // 다른 유저가 로비 서버에 접속했을 때 서버가 보내는 메세지를 받는 메소드
        // 스레드로 만들어 실행
        private void getMessage()
        {
            while (true)
            {
                // 로비 서버로부터 메세지가 오면 byte 형식으로 읽음
                int buffSize = 0;
                byte[] inStream = new byte[LENGTH.MAX_PACKEN_LEN];
                buffSize = lobbySocket.ReceiveBufferSize;
                lobbySocket.Receive(inStream);

                string returndata = null;
                returndata = System.Text.Encoding.ASCII.GetString(inStream);
                readData = returndata;

                // byte 형식으로 받은 데이터를 Json 형식으로 변환
                LobbyClientInformation getNewClientInfo = JsonConvert.DeserializeObject<LobbyClientInformation>(returndata);

                User user = new User(getNewClientInfo.status, getNewClientInfo.user_id);

                // 새로 온 사람을 로비 접속 유저 리스트에 삽입
                // !!!!!!!!!!!!!!!!!!!!!!!나중에 경우에 따라 수정 필요!!!!!!!!!!!!!!!!!!!!
                lobbyClientInfo.lobbyList.Add(user);

                //if (getNewClientInfo != null)
                //{
                //    // 새 유저가 LOBBY_IN 의 상태로 로비에 접속했으면
                //    if (getNewClientInfo.status == LOBBY_STATUS.LOBBY_IN)
                //    {
                //        readData = "--------------------" + getNewClientInfo.user_id + " Joined ! --------------------";
                //        msg();
                //        form.listV();
                //    }

                //    // 유저가 로비를 나갔으면 (추후 구현)
                //    else if (getNewClientInfo.status == LOBBY_STATUS.LOBBY_OUT)
                //    {

                //    }
                //}
            }
        }

        // 받은 메시지를 로비 폼의 채팅방에 뿌려주는 메소드
        private void msg()
        {
            if (form.InvokeRequired)
                form.Invoke(new MethodInvoker(msg));
            else
                form.chatLog.Text = form.chatLog.Text + Environment.NewLine + " >> " + readData;
        }

        public LobbyClientInformation lobbyClientInfoFromLobby()
        {
            return lobbyClientInfo;
        }
    }
}
