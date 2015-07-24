using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace gameServer
{
    class Player
    {
        string id;
        Socket socket;
        int x_coordinate;
        int y_coordinate;

        public Player()
        {
        }

        public Player(string id, Socket socket)
        {
            this.id = id;
            this.socket = socket;
            this.x_coordinate = 0;
            this.y_coordinate = 0;
        }

    }
}
