using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net.Sockets;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Collections;
using Newtonsoft.Json;


namespace simpleLobbyServer
{
    class Program
    {
        public static Hashtable clientsList = new Hashtable();


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

                byte[] bytesFrom = new byte[10025];
                byte[] bytesFrom2 = new byte[10025];
           
                Byte[] bytesTo = null;
                string dataFromClient = null;

                NetworkStream networkStream = clientSocket.GetStream();
                networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                lobbyInfo userinfo = JsonConvert.DeserializeObject<lobbyInfo>(dataFromClient);

                Console.WriteLine(userinfo);

                lobbyInfo chInfo = new lobbyInfo();
                chInfo = userinfo;

                clientsList.Add(chInfo.id, clientSocket);
                dataFromClient = chInfo.id;
                foreach (DictionaryEntry LobbyMem in clientsList)
                {
                    chInfo.lobbyList.Add(LobbyMem.Key.ToString());
                }

                string output = JsonConvert.SerializeObject(chInfo);
                bytesFrom = Encoding.ASCII.GetBytes(output);
                broadcast(bytesFrom);
                Console.WriteLine("Joined ID: " + chInfo.id);

                /*
                chInfo.task = "lobbyMem";
                int i = 0;
                string  output2 = JsonConvert.SerializeObject(chInfo);
                bytesFrom2 = Encoding.ASCII.GetBytes(output2);
                broadcast(bytesFrom2);
                */

                //broadcast(ID + " Joined ", ID, false);

                handleClinet client = new handleClinet();
                client.startClient(clientSocket, dataFromClient, clientsList);
            }


            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine("exit");
            Console.ReadLine();
        }
        public static void broadcast(byte[] byteForm)
        {

            foreach (DictionaryEntry Item in clientsList)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)Item.Value;
                NetworkStream broadcastStream = broadcastSocket.GetStream();          
                byte[] broadcastBytes = byteForm;
                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
        }  //end broadcast function
/*
        public static void multicast(byte[] byteForm)
        {
            string dataFromClient = System.Text.Encoding.ASCII.GetString(byteForm);
            chatInfo chInfo = JsonConvert.DeserializeObject<chatInfo>(dataFromClient);
            foreach (string targetName in chInfo.chatList) 
            {
                TcpClient multicastSocket;
                multicastSocket = (TcpClient)clientsList[targetName];
                NetworkStream multicastStream = multicastSocket.GetStream();     
                byte[] multicastBytes = byteForm;
                multicastStream.Write(multicastBytes, 0, multicastBytes.Length);
                multicastStream.Flush();
            }
        }  //end multicast function   
               
        */


        public class handleClinet
        {
            TcpClient clientSocket;
            string clNo;
            Hashtable clientsList;

            public void startClient(TcpClient inClientSocket, string clineNo, Hashtable cList)
            {
                this.clientSocket = inClientSocket;
                this.clNo = clineNo;
                this.clientsList = cList;
                Thread ctThread = new Thread(doChat);
                ctThread.Start();
            }

            private void doChat()
            {
                int requestCount = 0;
                byte[] bytesFrom = new byte[10025];
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
                        //////////////////////
                        dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                        lobbyInfo chInfo = JsonConvert.DeserializeObject<lobbyInfo>(dataFromClient);
                        if (chInfo.task == "lobbyIn")
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
                    }
                }//end while
            }//end doChat
        } //end class handleClinet

        public class UserInfo
        {
            public string task;
            public string id;
            public string passwd;
        }

        public class lobbyInfo
        {
            public string task;
            public string id;
            public List<string> lobbyList = new List<string>();
            public string msg;
        }

    }//end namespace

}