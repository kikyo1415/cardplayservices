using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerCardPlay.Core
{
    public class GameRoomManager
    {
        private static object locker = new object();
        private static GameRoomManager instance;
        private ConcurrentDictionary<int, GameRoom> roomDic;
        private GameRoomManager()
        {
            roomDic = new ConcurrentDictionary<int, GameRoom>();
        }

        public static GameRoomManager GetInstance()
        {
            lock (locker)
                if (instance == null)
                    lock (locker)
                        instance = new GameRoomManager();
            return instance;
        }

        public GameRoom CreateRoom(Action<Guid, string> sendMsgCallback)
        {
            GameRoom room = new GameRoom(10086, sendMsgCallback);
            return roomDic.GetOrAdd(10086, room);
        }

        public string JoinRoom(Guid playerId, int num)
        {
            GameRoom room = null;
            if (roomDic.TryGetValue(num, out room))
               return room.PlayerGetJoin(playerId);
            else
                throw new Exception("房间不存在");
        }

        /// <summary>
        /// 获取玩家所在房间。
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public GameRoom GetRoomByPlayerId(Guid playerId)
        {
            var room = roomDic.FirstOrDefault(d => d.Value.Players.Any(s => s.ID.Equals(playerId)));
            if (room.Value == null)
                throw new Exception("玩家不在任何房间");
            return room.Value;
        }
    }
}
