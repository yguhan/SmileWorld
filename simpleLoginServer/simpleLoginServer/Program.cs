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
                string dataFromClient = null;

                NetworkStream networkStream = clientSocket.GetStream();
                networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                UserInfo userinfo = JsonConvert.DeserializeObject<UserInfo>(dataFromClient);

                Console.WriteLine("Task: " + userinfo.task);
                Console.WriteLine("ID: " + userinfo.id);
                Console.WriteLine("passWD: " + userinfo.passwd);

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "SELECT * FROM `game`.`userdb` WHERE userid = '" + userinfo.id +"'";
                cmd.Connection = conn;

                MySqlDataReader rdr = cmd.ExecuteReader();

                LoginReturn lg_return = new LoginReturn(LOGIN_RESULT.TASK_DEFAULT, LOGIN_RESULT.RESULT_DEFAULT);

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

                if (userinfo.task == LOGIN_RESULT.LOGIN)
                {
                    lg_return.task = LOGIN_RESULT.LOGIN;

                    if (rdr.HasRows)
                    {
                        if (String.Compare((string)rdr["pwd"], userinfo.passwd) == 0)
                        {
                            Console.WriteLine("logged in!");
                            lg_return.result = LOGIN_RESULT.LOGIN_SUCCESS;
                        }
                        else
                        {
                            Console.WriteLine("wrong passwd! Right passwd = " + rdr["pwd"]);
                            lg_return.result = LOGIN_RESULT.LOGIN_FAIL_PASSWORD;
                        }
                    }
                    else
                    {
                        Console.WriteLine("no such ID!");
                        lg_return.result = LOGIN_RESULT.LOGIN_FAIL_NO_ID;
                    }
                }
                else
                {
                    lg_return.task = LOGIN_RESULT.SIGNUP;

                    if (!rdr.HasRows)
                    {
                        Console.WriteLine("Signed in!");
                        lg_return.result = LOGIN_RESULT.SIGNUP_SUCCESS;

                        rdr.Close();
                        MySqlCommand insert_cmd = new MySqlCommand();
                        insert_cmd.Connection = conn;
                        insert_cmd.CommandText = "INSERT INTO `game`.`userdb` (userid,pwd) VALUES ('" + userinfo.id + "','" + userinfo.passwd + "')";
                        insert_cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        Console.WriteLine("ID already exists!");
                        lg_return.result = LOGIN_RESULT.SIGNUP_FAIL_ID_EXISTS;
                    }
                }

                string output = JsonConvert.SerializeObject(lg_return);
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(output);
                networkStream.Write(outStream, 0, outStream.Length);
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
