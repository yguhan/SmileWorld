using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    static class LENGTH
    {
        public const int MAX_USERID_LEN = 50;
        public const int MAX_PASSWD_LEN = 32;
        public const int MAX_CHATMESSAGE_LEN = 512;
        public const int MAX_PACKET_LEN = 8192;
    }
    
    static class CHAT_TARGET
    {
        public const int CHAT_DEFAULT = 0;
        public const int CHAT_ALL = 1;
        public const int CHAT_WHISPER = 2;
        public const int CHAT_GAMEROOM = 3;
    }

    static class LOBBY_STATUS
    {
        public const int LOBBY_DEFAULT = 0;
        public const int LOBBY_IN = 1;
        public const int ROOM_IN = 2;
        public const int GAME_IN = 3;

        public const int LOBBY_OUT = -1;
    }

    public class ChatProtocol
    {
        public int chat_target;
        public string sender_id;
        public List<string> targetUserList;
        public string message;

        public ChatProtocol()
        {
            this.chat_target = CHAT_TARGET.CHAT_DEFAULT;
            this.sender_id = "A";
            this.message = "A";
            this.targetUserList = new List<string>();
        }
    }
}
