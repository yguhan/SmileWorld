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
            
                Byte[] bytesTo = null;
                string dataFromClient = null;

                NetworkStream networkStream = clientSocket.GetStream();
                networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                UserInfo userinfo = JsonConvert.DeserializeObject<UserInfo>(dataFromClient);

                Console.WriteLine(userinfo);
                
                string ID = userinfo.id;
                clientsList.Add(ID, clientSocket);
                //
                dataFromClient = ID;
                chatInfo chInfo = new chatInfo();
                chInfo.id = ID;
                chInfo.msg = ID + "Joined";
                chInfo.task = "chatAll";
                string output = JsonConvert.SerializeObject(chInfo);
                //broadcast(output, chInfo.id, false);

                // byte[] bytesFrom2 = new byte[10025];
                bytesFrom = Encoding.ASCII.GetBytes(output);
                broadcast(bytesFrom);
                Console.WriteLine("Joined ID: " + ID);

                chInfo.task = "lobbyMem";
                int i = 0;
                foreach (string lobbyMemId in clientsList.Keys)
                {
                    chInfo.idList.Add(lobbyMemId);
                }

                string  output2 = JsonConvert.SerializeObject(chInfo);
                bytesFrom = Encoding.ASCII.GetBytes(output2);

                broadcast(bytesFrom);


                //broadcast(ID + " Joined ", ID, false);

                //

                //



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
                //broadcastBytes = Encoding.ASCII.GetBytes(chInfo);
                //broadcastBytes = byteForm;


                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
        }  //end broadcast function
        /*
                public static void broadcast(string msg, string uName, bool flag)
                {
                    foreach (DictionaryEntry Item in clientsList)
                    {
                        TcpClient broadcastSocket;
                        broadcastSocket = (TcpClient)Item.Value;
                        NetworkStream broadcastStream = broadcastSocket.GetStream();
                        Byte[] broadcastBytes = null;

                        if (flag == true)
                        {
                            broadcastBytes = Encoding.ASCII.GetBytes(uName + " says : " + msg);
                        }
                        else
                        {
                            broadcastBytes = Encoding.ASCII.GetBytes(msg);
                        }

                        broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                        broadcastStream.Flush();
                    }
                }  //end broadcast function
        
                public static void unicast(string msg, string uName, string rName, bool flag)
                {
                    //foreach (DictionaryEntry Item in clientsList)
            
                    TcpClient unicastSocket;
                    //unicastSocket = (TcpClient)Item.Value;
                    unicastSocket = (TcpClient)clientsList[rName];
                    NetworkStream unicastStream = unicastSocket.GetStream();
                    Byte[] unicastBytes = null;

                    if (flag == true)
                    {
                        unicastBytes = Encoding.ASCII.GetBytes(uName + " says : " + msg);
                    }
                    else
                    {
                        unicastBytes = Encoding.ASCII.GetBytes(msg);
                    }

                    unicastStream.Write(unicastBytes, 0, unicastBytes.Length);
                    unicastStream.Flush();
                }  //end unicast function   
            }//end Main class
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
                        chatInfo chInfo = JsonConvert.DeserializeObject<chatInfo>(dataFromClient);
                        if (chInfo.task == "chatAll")
                        {
                            Program.broadcast(bytesFrom);
                        }
                        //else if (chInfo.task == "chatTarget")
                        //{
                        //   Program.unicast(chInfo.msg, chInfo.id, chInfo.idList[0], true);
                        //}
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

        public class chatInfo
        {
            public string task;
            public string id;
            public List<string> idList = new List<string>();
            public string msg;
        }

    }//end namespace

}