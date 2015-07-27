using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace simpleLobbyServer
{
    public class User
    {
        public int status;
        public string user_id;

        public User()
        {
            this.status = LOBBY_STATUS.LOBBY_DEFAULT;
            this.user_id = "A";
        }

        public User(int status, string user_id)
        {
            this.status = status;
            this.user_id = user_id;
        }
    }

    public class UserWithSocket
    {
        public User user;
        public TcpClient clientSocket;

        public UserWithSocket(User user, TcpClient clientSocket)
        {
            this.user = user;
            this.clientSocket = clientSocket;
        }
    }

    public class UserInformation
    {
        public List<UserWithSocket> userlist;
        public List<User> userlistwithoutsocket;
        public int userCount;
        public User myInformation;

        public UserInformation()
        {
            userlist = new List<UserWithSocket>();
            userCount = 0;
            myInformation = new User();
            userlistwithoutsocket = new List<User>();
        }

        public bool getNewUser(int status, string user_id, TcpClient clientSocket)
        {
            try
            {
                User user = new User(status, user_id);
                UserWithSocket UserWSocket = new UserWithSocket(user, clientSocket);

                this.userlistwithoutsocket.Add(user);
                this.userlist.Add(UserWSocket);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool removeUser()
        {
            return false;
        }
    }


}
