using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace simpleLobbyServer
{
    class ClientManager
    {
        public static UserInformation userinfo = new UserInformation();

        public void test()
        {
            TcpListener serverSocket = new TcpListener(8001);
            TcpClient clientSocket = default(TcpClient);
            serverSocket.Start();
            Console.WriteLine("Lobby Server Started..");

            while (true)
            {
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

                UserWithSocket userWithSocket = new UserWithSocket(user, clientSocket);

                userinfo.myInformation = user;

                userinfo.userlist.Add(userWithSocket);
                userinfo.userlistwithoutsocket.Add(user);

                string output = JsonConvert.SerializeObject(userinfo.userlistwithoutsocket);
                bytesFrom = System.Text.Encoding.ASCII.GetBytes(output);

                networkStream.Write(bytesFrom, 0, bytesFrom.Length);
                networkStream.Flush();

                foreach (UserWithSocket uws in userinfo.userlist)
                {
                    NetworkStream nws = uws.clientSocket.GetStream();
                    nws.Write(bytesFrom, 0, bytesFrom.Length);
                    nws.Flush();
                }

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
                    rCount = Convert.ToString(requestCount);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    break;
                }
            }//end while
        }//end doChat
    } //end class handleClinet

}
