using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace gameServer
{
    class Program
    {
        public Socket m_ServerSocket;
        public Socket m_clientSocket;
        public byte[] szData;

        static void Main(string[] args)
        {
            Console.WriteLine("Game Server Started ....");
            Program pg = new Program();
            pg.start();
            
        }

        private void start()
        {
            m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9000);

            m_ServerSocket.Bind(ipep);
            m_ServerSocket.Listen(20);
            m_clientSocket = m_ServerSocket.Accept();
            Console.WriteLine("Attached to Lobby Server ....");

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            szData = new byte[1024];
            args.SetBuffer(szData, 0, 1024);
            args.Completed += new EventHandler<SocketAsyncEventArgs>(Receive_Completed);
            m_ServerSocket.ReceiveAsync(args);

            while (true)
            {
            }
        }

        private void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket ClientSocket = (Socket)sender;
            Console.WriteLine("Receiving data");
            if (ClientSocket.Connected && e.BytesTransferred > 0)
            {
                byte[] bytesFrom = e.Buffer;    // 데이터 수신
                String dataFromLobby = System.Text.Encoding.UTF8.GetString(bytesFrom);
                gameRoomInfo gameroominfo = JsonConvert.DeserializeObject<gameRoomInfo>(dataFromLobby);
                if (gameroominfo != null)
                {
                    GameRoom gameRoom = new GameRoom(gameroominfo);
                    Console.WriteLine("Game Created");

                    //string Test = sData.Replace("\0", "").Trim();
                    for (int i = 0; i < szData.Length; i++)
                    {
                        szData[i] = 0;
                    }
                    e.SetBuffer(szData, 0, 1024);
                    ClientSocket.ReceiveAsync(e);
                }
            }
            else
            {
                Console.WriteLine("Lobby Server Disconnected");
                ClientSocket.Disconnect(false);
                ClientSocket.Dispose();
            }
        }
    }

    class gameRoomInfo
    {
        public string HeadUser;
        public List<string> UserList;
    }
}
