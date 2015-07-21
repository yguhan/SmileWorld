using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using Newtonsoft.Json;

namespace ConsoleApplication1
{
    class Program
    {
        public static Hashtable socketToClient = new Hashtable();
        public static Hashtable clientToSocket = new Hashtable();
        public static Socket m_ServerSocket;
        public byte[] szData;

        static void Main(string[] args)
        {
            Program pg = new Program();
            pg.start();
        }

        private void start()
        {
            m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 10000);
            m_ServerSocket.Bind(ipep);
            m_ServerSocket.Listen(20);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_Completed);
            m_ServerSocket.AcceptAsync(args);
            Console.WriteLine("Chat Server Started ....");

            while (true)
            {
            }
        }

        private void Accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket ClientSocket = e.AcceptSocket;
            Console.WriteLine("Accepting connection");


            byte[] bytesFrom = new byte[10025];
            string dataFromClient = null;
            ClientSocket.Receive(bytesFrom);
            dataFromClient = System.Text.Encoding.UTF8.GetString(bytesFrom);
            chatInfo chatInfo = JsonConvert.DeserializeObject<chatInfo>(dataFromClient);

            socketToClient.Add(ClientSocket, chatInfo.id);
            clientToSocket.Add(chatInfo.id, ClientSocket);
            if (chatInfo.task == "chatAll") {
                broadcast(chatInfo);
            }
            else if (chatInfo.task == "chatTarget")
            {
                multicast(chatInfo);
            }
            Console.WriteLine(chatInfo.id + " Joined ");

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            szData = new byte[1024];
            args.SetBuffer(szData, 0, 1024);
            args.Completed
                += new EventHandler<SocketAsyncEventArgs>(Receive_Completed);
            ClientSocket.ReceiveAsync(args);

            e.AcceptSocket = null;
            m_ServerSocket.AcceptAsync(e);
        }

        private void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket ClientSocket = (Socket)sender;
            Console.WriteLine("Receiving data");
            if (ClientSocket.Connected && e.BytesTransferred > 0)
            {
                byte[] bytesFrom = e.Buffer;    // 데이터 수신
                string dataFromClient = Encoding.UTF8.GetString(szData);
                chatInfo chatInfo = JsonConvert.DeserializeObject<chatInfo>(dataFromClient);
                if (chatInfo != null)
                {
                    broadcast(chatInfo);
                    Console.WriteLine("Message Received: " + chatInfo.msg);

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
                Console.WriteLine(socketToClient[ClientSocket] + "Disconnected");
                ClientSocket.Disconnect(false);
                ClientSocket.Dispose();
                clientToSocket.Remove(socketToClient[ClientSocket]);
                socketToClient.Remove(ClientSocket);
            }
        }


        public static void broadcast(chatInfo chInfo)
        {
            string output = JsonConvert.SerializeObject(chInfo);
            Byte[] broadcastBytes = null;
            broadcastBytes = Encoding.UTF8.GetBytes(output);

            foreach (DictionaryEntry Item in clientToSocket)
            {
                Socket broadcastSocket;
                broadcastSocket = (Socket)Item.Value;
                broadcastSocket.Send(broadcastBytes, SocketFlags.None);
            }
        }  //end broadcast function

        public static void multicast(chatInfo chInfo)
        {
            string output = JsonConvert.SerializeObject(chInfo);
            Byte[] multicastBytes = null;

            foreach (string targetName in chInfo.chatList)
            {
                Socket multicastSocket;
                multicastSocket = (Socket)clientToSocket[targetName];
                multicastSocket.Send(multicastBytes, SocketFlags.None);
            }
        }  //end multicast function   
    }

    public class chatInfo
    {
        public string task;
        public string id;
        public List<string> chatList = new List<string>();
        public string msg;
    }
}
