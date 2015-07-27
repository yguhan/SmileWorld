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

namespace ChatServer
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


            byte[] bytesFrom = new byte[LENGTH.MAX_PACKET_LEN];
            string dataFromClient = null;
            ClientSocket.Receive(bytesFrom);
            dataFromClient = System.Text.Encoding.UTF8.GetString(bytesFrom);
            ChatProtocol chatProtocol = JsonConvert.DeserializeObject<ChatProtocol>(dataFromClient);

            socketToClient.Add(ClientSocket, chatProtocol.sender_id);
            clientToSocket.Add(chatProtocol.sender_id, ClientSocket);

            Console.WriteLine(chatProtocol.sender_id + " Joined ");

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
                string dataFromClient = Encoding.UTF8.GetString(bytesFrom);
                ChatProtocol chatProtocol = JsonConvert.DeserializeObject<ChatProtocol>(dataFromClient);
                if (chatProtocol != null)
                {
                    broadcast(chatProtocol);
                    Console.WriteLine("Message Received: " + chatProtocol.message);

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


        public static void broadcast(ChatProtocol chatProtocol)
        {
            string output = JsonConvert.SerializeObject(chatProtocol);
            Byte[] broadcastBytes = null;
            broadcastBytes = Encoding.UTF8.GetBytes(output);

            foreach (DictionaryEntry Item in clientToSocket)
            {
                Socket broadcastSocket;
                broadcastSocket = (Socket)Item.Value;
                broadcastSocket.Send(broadcastBytes, SocketFlags.None);
            }
        }  //end broadcast function

        public static void multicast(ChatProtocol chatProtocol)
        {
            string output = JsonConvert.SerializeObject(chatProtocol);
            Byte[] multicastBytes = null;

            foreach (string targetName in chatProtocol.targetUserList)
            {
                Socket multicastSocket;
                multicastSocket = (Socket)clientToSocket[targetName];
                multicastSocket.Send(multicastBytes, SocketFlags.None);
            }
        }  //end multicast function   
    }
}
