using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net.Sockets;
using MySql.Data.MySqlClient;
using System.Threading;
using Newtonsoft.Json;


namespace simpleLobbyServer
{
    class Program
    {
        public static RoomInformation roominfo = new RoomInformation();
        public static UserInformation userinfo = new UserInformation();

        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(8001);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;
            serverSocket.Start();
            Console.WriteLine("Lobby Server Started..");

            while (true)
            {
                counter += 1;
                clientSocket = serverSocket.AcceptTcpClient();
                Console.WriteLine("Lobby Success");

                byte[] bytesFrom = new byte[LENGTH.MAX_PACKET_LEN];
                byte[] bytesFrom2 = new byte[LENGTH.MAX_PACKET_LEN];
           
                Byte[] bytesTo = null;
                string dataFromClient = null;

                NetworkStream networkStream = clientSocket.GetStream();
                networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);

                User user = new User();
                user = JsonConvert.DeserializeObject<User>(dataFromClient);

                LobbyClientInformation lobbyClientInfo = new LobbyClientInformation();
                lobbyClientInfo.status = LOBBY_STATUS.LOBBY_IN;
                lobbyClientInfo.user_id = user.user_id;
                lobbyClientInfo.lobbyList = userinfo.userlistwithoutsocket;

                UserWithSocket userWithSocket = new UserWithSocket(user, clientSocket);
                
                Console.WriteLine(user);

                userinfo.myInformation = user;

                userinfo.userlist.Add(userWithSocket);

                string output = JsonConvert.SerializeObject(lobbyClientInfo);
                bytesFrom = Encoding.ASCII.GetBytes(output);
                broadcast(bytesFrom);
                Console.WriteLine("Joined ID: " + userinfo.myInformation.user_id);

                handleClinet client = new handleClinet();
                client.startClient(userWithSocket.clientSocket, dataFromClient, userinfo.userlist);
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine("exit");
            Console.ReadLine();
        }

        public static void broadcast(byte[] byteForm)
        {

            foreach (UserWithSocket userWithSocket in userinfo.userlist)
            {
                TcpClient broadcastSocket;
                broadcastSocket = userWithSocket.clientSocket;
                NetworkStream broadcastStream = broadcastSocket.GetStream();          
                byte[] broadcastBytes = byteForm;
                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
        }

        public class handleClinet
        {
            TcpClient clientSocket;
            string clNo;
            List<UserWithSocket> userlist;

            public void startClient(TcpClient inClientSocket, string clineNo, List<UserWithSocket> userlist)
            {
                this.clientSocket = inClientSocket;
                this.clNo = clineNo;
                this.userlist = userlist;
                Thread ctThread = new Thread(doChat);
                ctThread.Start();
            }

            private void doChat()
            {
                int requestCount = 0;
                byte[] bytesFrom = new byte[LENGTH.MAX_PACKET_LEN];
                string dataFromClient = null;
                Byte[] sendBytes = null;
                string serverResponse = null;
                string rCount = null;
                requestCount = 0;

                while ((true))
                {
                    try
                    {
                        requestCount = requestCount + 1;

                        NetworkStream networkStream = clientSocket.GetStream();
                        networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                        dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                        User user = JsonConvert.DeserializeObject<User>(dataFromClient);
                        if (user.status == LOBBY_STATUS.LOBBY_IN)
                        {
                            Program.broadcast(bytesFrom);
                        }
              
                        /*
                        NetworkStream networkStream = clientSocket.GetStream();
                        networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                        dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                        dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                        Console.WriteLine("From client - " + clNo + " : " + dataFromClient);  
                        */
                        rCount = Convert.ToString(requestCount);
                        //Program.broadcast(dataFromClient, clNo, true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        break;
                    }
                }//end while
            }//end doChat
        } //end class handleClinet
    }//end namespace
}