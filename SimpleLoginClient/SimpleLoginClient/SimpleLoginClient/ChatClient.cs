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
        Socket chatSocket;
        Form2 form;
        ChatClientInformation chatClientInfo;
        string readData;

        public ChatClient()
        {

        }

        public ChatClient(Form2 form2, ChatClientInformation userInfo)
        {
            // ChatClient 멤버변수 할당
            chatSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            form = form2;
            chatClientInfo = new ChatClientInformation();
            chatClientInfo = userInfo;
            readData = null;

            // socket을 채팅 서버에 연결, getMessage 스레드 시작
            chatSocket.Connect("127.0.0.1", 10000);
            this.sendMessage(this.chatClientInfo);
            Thread ctThread = new Thread(getMessage);
            ctThread.Start();
        }

        // 새로운 채팅 메세지를 채팅 서버에 보내는 메소드
        public void sendMessage(ChatClientInformation chatClientInfoFromForm2)
        {
            // 새로운 채팅 프로토콜 object 할당
            ChatProtocol chatProtocol = new ChatProtocol();

            // 채팅 프로토콜 object에, 클라이언트가 가지고 있던 정보 삽입
            chatProtocol.chat_target = chatClientInfoFromForm2.chat_target;
            chatProtocol.message = form.msgInput.Text;
            chatProtocol.sender_id = chatClientInfoFromForm2.user_id;
            
            // 채팅 모드에 따라 target id 정함
            if (chatProtocol.chat_target == CHAT_TARGET.CHAT_WHISPER || chatProtocol.chat_target == CHAT_TARGET.CHAT_GAMEROOM)
            {
                // 귓속말 모드이거나 방에 소속된 경우. 자신까지 채팅의 타겟으로 삽입
                chatProtocol.targetUserList.Add(chatClientInfo.user_id);

                // 귓속말 모드일 경우 타겟 유저 아이디를 리스트에 삽입
                if (chatProtocol.chat_target == CHAT_TARGET.CHAT_WHISPER)
                {
                    chatProtocol.targetUserList.Add(chatClientInfo.target_id);
                }

                // 방 채팅 모드일 경우 방에 접속한 유저 아이디를 리스트에 삽입
                if (chatProtocol.chat_target == CHAT_TARGET.CHAT_GAMEROOM)
                {
                    foreach (string chatMem in chatClientInfo.roomUserList)
                    {
                        chatProtocol.targetUserList.Add(chatMem);
                    }
                }
            }

            // 채팅 프로토콜을 Json으로 변환하여 소켓을 통해 서버로 전송
            Console.WriteLine(chatProtocol.message);
            string output = JsonConvert.SerializeObject(chatProtocol);
            byte[] outStream = System.Text.Encoding.UTF8.GetBytes(output);
            chatSocket.Send(outStream, SocketFlags.None);
            changeChatTarget(CHAT_TARGET.CHAT_ALL);
        }

        // 채팅 메세지를 채팅 서버로부터 받아오는 메소드
        // 스레드로 만들어 실행
        private void getMessage()
        {
            while (true)
            {
                // 채팅 서버로부터 메세지가 오면 byte 형식으로 읽음
                int buffSize = 0;
                byte[] inStream = new byte[LENGTH.MAX_PACKET_LEN];
                buffSize = chatSocket.ReceiveBufferSize;
                chatSocket.Receive(inStream);

                string returndata = System.Text.Encoding.UTF8.GetString(inStream);
                readData = returndata;

                // byte 형식으로 받은 데이터를 Json 형식으로 변환
                ChatProtocol chatProtocol = JsonConvert.DeserializeObject<ChatProtocol>(returndata);

                if (chatProtocol != null)
                {
                    // 전체 채팅 모드
                    if (chatProtocol.chat_target == CHAT_TARGET.CHAT_ALL)
                    {
                        readData = chatProtocol.sender_id + " says : " + chatProtocol.message;
                        msg();
                    }
                    // 귓속말 모드
                    else if (chatProtocol.chat_target == CHAT_TARGET.CHAT_WHISPER)
                    {
                        readData = chatProtocol.targetUserList[0] + " says to " + chatProtocol.targetUserList[1] + " : " + chatProtocol.message;
                        msg();
                    }
                    // 방 채팅 모드
                    else if (chatProtocol.chat_target == CHAT_TARGET.CHAT_GAMEROOM)
                    {
                        readData = chatProtocol.targetUserList[0] + "says : " + chatProtocol.message;
                        msg();
                    }
                }
            }
        }

        // 받은 메시지를 로비 폼의 채팅방에 뿌려주는 메소드
        private void msg()
        {
            if (form.InvokeRequired)
                form.Invoke(new MethodInvoker(msg));
            else
                form.chatLog.AppendText(">> " + readData + "\n");
        }

        /*
        public ChatClientInformation chatInfoFromChat() 
        {
            return chatClientInfo;
        }
        */

        // 현재 클라이언트의 채팅 모드를 바꿔주는 메소드
        public void changeChatTarget(int chat_target)
        {
            chatClientInfo.chat_target = chat_target;
        }
    }
}
