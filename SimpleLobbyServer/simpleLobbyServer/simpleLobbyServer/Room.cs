using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simpleLobbyServer
{
    public class Room
    {
        public List<int> Users;
        public int map;
        public int max;
        public int passwd;
        public bool usePasswd;
        public int roomNumber;

        public Room()
        {
            Users = new List<int>();
            map = 0;
            max = 0;
            passwd = 0;
            usePasswd = false;
            roomNumber = 0;
        }

        public Room(int firstUser, int map, int max, int passwd, bool usePasswd)
        {
            Room room = new Room();
            room.Users.Add(firstUser);
            room.map = map;
            room.max = max;
            room.passwd = passwd;
            room.usePasswd = usePasswd;
        }
    }

    public class RoomInformation
    {
        public List<Room> roomlist;
        public int roomCount;

        public RoomInformation()
        {
            roomlist = new List<Room>();
            roomCount = 1;
        }

        public bool createRoom(int firstUser, int map, int max, int passwd, bool usePasswd)
        {
            try
            {
                Room room = new Room(firstUser, map, max, passwd, usePasswd);
                room.roomNumber = roomCount++;  // 중간에 비는 번호 채우지 않음.

                this.roomlist.Add(room);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool removeRoom(int roomNumber)
        {
            try
            {
                roomlist.Remove(roomlist.Find(x => x.roomNumber == roomNumber));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
