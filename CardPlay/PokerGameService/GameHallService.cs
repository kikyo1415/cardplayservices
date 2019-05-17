using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerCardPlay.GameService
{


    public class GameHallService
    {
        private ConcurrentDictionary<int, GameRoomService> roomDic;

        public GameHallService()
        {
            roomDic = new ConcurrentDictionary<int, GameRoomService>();
        }

        public string CreateGameRoom()
        {
            throw new NotImplementedException();
        }

        public string GetPlayerNowRoomNum(Guid playerId)
        {
            throw new NotImplementedException();
        }

        public bool JoinGameRoom(string roomNumber)
        {
            throw new NotImplementedException();
        }

        public bool ExitGameRoom(string roomNumber)
        {
            throw new NotImplementedException();
        }

    }
}
