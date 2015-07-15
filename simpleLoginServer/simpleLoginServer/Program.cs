using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net.Sockets;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace simpleLoginServer
{
    class Program
    {
        static void Main(string[] args)
        {
            String strConn = "Server=192.168.0.45;Database=game;Uid=kimhoon;Pwd=1234;";
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(strConn);
                conn.Open();
                Console.WriteLine("MySQL version : {0}", conn.ServerVersion);

            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: {0}", ex.ToString());

            }

            TcpListener serverSocket = new TcpListener(8000);
            TcpClient clientSocket = default(TcpClient);
            serverSocket.Start();
            Console.WriteLine("Log In Server Started..");

            while (true)
            {
                clientSocket = serverSocket.AcceptTcpClient();
                byte[] bytesFrom = new byte[10025];
                Byte[] bytesTo = null;
                string dataFromClient = null;

                NetworkStream networkStream = clientSocket.GetStream();
                networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                UserInfo userinfo = JsonConvert.DeserializeObject<UserInfo>(dataFromClient);

                string[] spstring = dataFromClient.Split('$');
                string task = spstring[0];
                string ID = spstring[1];
                string passWD = spstring[2];
                Console.WriteLine("Task: " +  task);
                Console.WriteLine("ID: " + ID);
                Console.WriteLine("passWD: " + passWD);

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "SELECT * FROM `game`.`userdb` WHERE userid = '" + ID + "'";
                cmd.Connection = conn;
              
                MySqlDataReader rdr = cmd.ExecuteReader();
            
                while (rdr.Read())
                {
                    Console.WriteLine(rdr);
                    Console.WriteLine("mysql key = " + rdr["serial_no"]);
                    Console.WriteLine("mysql id = " + rdr["userid"]);
                    Console.WriteLine("mysql pwd = " + rdr["pwd"]);
                    Console.WriteLine("mysql games = " + rdr["games"]);
                    Console.WriteLine("mysql win = " + rdr["win"]);
                    Console.WriteLine("mysql lose = " + rdr["lose"]);

                }

                if (task == "login")
                {                   
                    if (rdr.HasRows)
                    {
                        if (String.Compare((string)rdr["pwd"],passWD) == 0)
                        {
                            Console.WriteLine("logged in!");
                            bytesTo = Encoding.ASCII.GetBytes("logged in!");                                                     
                        }
                        else
                        {
                            Console.WriteLine("wrong passwd! Right passwd = " + rdr["pwd"]);
                            bytesTo = Encoding.ASCII.GetBytes("wrong passwd!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("no such ID!");
                        bytesTo = Encoding.ASCII.GetBytes("no such ID!");
                    }
                }
                else
                {
                    if (!rdr.HasRows)
                    {
                        Console.WriteLine("Signed in!");
                        bytesTo = Encoding.ASCII.GetBytes("Signed in!");
                        
                        rdr.Close();
                        MySqlCommand insert_cmd = new MySqlCommand();
                        insert_cmd.Connection = conn;
                        insert_cmd.CommandText = "INSERT INTO `game`.`userdb` (userid,pwd) VALUES ('" + ID + "','" + passWD + "')";
                        insert_cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        Console.WriteLine("ID already exists!");
                        bytesTo = Encoding.ASCII.GetBytes("ID already exists!");
                    }
                }
                networkStream.Write(bytesTo, 0, bytesTo.Length);
                networkStream.Flush();
                Console.WriteLine("Message Sent!");
                rdr.Close();
            }
            if (conn != null)
            {
                conn.Close();
            }
        }
    }
}
