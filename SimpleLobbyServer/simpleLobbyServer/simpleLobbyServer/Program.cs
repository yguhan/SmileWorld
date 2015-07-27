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
        public static RoomInformation roominfo = new RoomInformation();
        public static UserInformation userinfo = new UserInformation();

        static ClientManager client;

        static void Main(string[] args)
        {
            client = new ClientManager();
            client.test();
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
    }//end namespace
}