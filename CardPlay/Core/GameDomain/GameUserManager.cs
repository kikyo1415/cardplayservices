using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace PokerCardPlay.Core
{
    public class GameUserManager
    {
        private ConcurrentDictionary<Guid, GameUser> onlineGameUserDic;
        private static GameUserManager instance;
        private static Object locker = new object();
        private GameUserManager()
        {

        }

        public static GameUserManager GetInstance()
        {
            lock (locker)
                if (instance == null)
                    lock (locker)
                        instance = new GameUserManager();
            return instance;
        }


        public IPlayer GetPlayerInfo(Guid playerId)
        {

            return new Player { ID = playerId };
        }
    }
}
