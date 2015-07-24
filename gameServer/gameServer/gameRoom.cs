using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace gameServer
{

    class GameRoom
    {
        public Socket m_ServerSocket;
        public Hashtable idToPlayer = new Hashtable();
        public gameRoomInfo gameroominfo;

        public GameRoom(gameRoomInfo gameroominfo)
        {
            Console.WriteLine("Game Room open...");
            this.gameroominfo = gameroominfo;
            start();
        }

        private void start()
        {
            m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9001);
            m_ServerSocket.Bind(ipep);
            m_ServerSocket.Listen(20);
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_Completed);
            m_ServerSocket.AcceptAsync(args);
            while (true)
            {
            }
        }

        private void Accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket ClientSocket = e.AcceptSocket;

            byte[] bytesFrom = new byte[10025];
            string dataFromClient = null;
            ClientSocket.Receive(bytesFrom);
            dataFromClient = System.Text.Encoding.UTF8.GetString(bytesFrom);
            gameJoinProtocol gamejoinprotocol = JsonConvert.DeserializeObject<gameJoinProtocol>(dataFromClient);
            Player player = new Player(gamejoinprotocol.userId,ClientSocket);
            idToPlayer.Add(gamejoinprotocol.userId,player);
            Console.WriteLine("User Joined: "+gamejoinprotocol.userId);

        }
    }

    class gameJoinProtocol()
    {
        public string userId;
    }

}
