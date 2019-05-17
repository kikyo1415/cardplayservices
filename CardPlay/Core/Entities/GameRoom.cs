using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerCardPlay.Core
{
    /// <summary>
    /// 游戏房间类
    /// </summary>
    public class GameRoom
    {
        private PokerGameMatch match;
        public List<IPlayer> Players;
        private Dictionary<Guid, bool> playerReadyDic;
        private Action<Guid, string> _sendMsgCallback;
        public int RoomNum { get; set; }

        public GameRoom(int RoomNum, Action<Guid, string> sendMsgCallback)
        {
            playerReadyDic = new Dictionary<Guid, bool>();
            Players = new List<IPlayer>();
            _sendMsgCallback = sendMsgCallback;
        }

        /// <summary>
        /// Get join.
        /// </summary>
        /// <param name="playerId">playerId</param>
        /// <returns>Is players full</returns>
        public string PlayerGetJoin(Guid playerId)
        {

            if (playerReadyDic.Any(d => d.Key.Equals(playerId)) || Players.Any(d => d.ID.Equals(playerId)))//若玩家已加入则返回false
                throw new ArgumentException("玩家已加入");
            else
            {
                var player = GameUserManager.GetInstance().GetPlayerInfo(playerId);
                string HasJoinPlayers = string.Empty;
                if (Players.Count > 0)
                {
                    foreach (var item in Players)
                        if (item != null)
                            HasJoinPlayers += item.ID + ",";
                    HasJoinPlayers = HasJoinPlayers.Substring(0, HasJoinPlayers.Length - 1);
                }

                Players.ForEach(d => playerReadyDic[d.ID] = false);
                Players.Add(player);
                playerReadyDic.Add(playerId, false);



                Players.ForEach(d => { _sendMsgCallback(d.ID, "{'ActionName':'GetJoin','joinplayer':'" + playerId + "'}"); });
                if (Players.Count >= 5)
                    Players.ForEach(d => { _sendMsgCallback(d.ID, "{'ActionName':'Notice','gamestate':'ready'}"); });
                return HasJoinPlayers;
            }
        }


        public List<IPlayer> GetRoomPlayers()
        {
            return this.Players;
        }

        /// <summary>
        /// Get ready
        /// </summary>
        /// <param name="playerId">playerId</param>
        /// <returns>Is all ready</returns>
        public void PlayerGetReady(Guid playerId)
        {
            if (!playerReadyDic.Any(d => d.Key.Equals(playerId)))
                throw new ArgumentException("未加入房间，无法准备");
            playerReadyDic[playerId] = true;
            Players.ForEach(d => { _sendMsgCallback(d.ID, "{'ActionName':'GetReady','readyplayer':'" + playerId + "'}"); });
            if (Players.Count == 5 && !playerReadyDic.Any(d => !d.Value))//没有还未准备的
            {
                Players.ForEach(d => { _sendMsgCallback(d.ID, "{'ActionName':'Notice','gamestate':'startgame'}"); });
                StartGame();

                string playerInfoJson = string.Empty;
                foreach (var item in Players)
                    if (item != null)
                        playerInfoJson += item.ID + ",";
                playerInfoJson = playerInfoJson.Substring(0, playerInfoJson.Length - 1);
                Players.ForEach(d => { _sendMsgCallback(d.ID, "{'ActionName':'GetPlayerInfo','Players':'" + playerInfoJson + "'}"); });


                Players.ForEach(d =>
                {
                    var cards = d.GetCards();
                    _sendMsgCallback(d.ID, "{'ActionName':'GetCards','cards':{'card1':'" + cards[0].ToFileName() + "','card2':'" + cards[1].ToFileName() + "'}}");
                });
            }
        }

        public void StartGame()
        {
            Players = Players.OrderBy(d => Guid.NewGuid()).ToList();
            match = new PokerGameMatch(Players, 100, 400);
            match.StartNewRound();
        }

        public int GetBankerIndex()
        {
            return match.GetBankerIndex();
        }

        public PokerCard[] GetPlayerCards(Guid playerId)
        {
            var player = Players.FirstOrDefault(d => d.ID.Equals(playerId));
            return player.GetCards();
        }

        public List<BetStyle> GetPlayerBetStyles(Guid playId)
        {
            return match.GetPlayerBetStyles(Players.FirstOrDefault(d => d.ID.Equals(playId)));
        }



        public void PlayerBetOn(Guid playerId, Betting betting)
        {
            var player = Players.FirstOrDefault(d => d.ID.Equals(playerId));
            match.PlayerBetOn(player, betting);
        }


        public void RemovePlayer(Guid playerId)
        {
            var player = Players.FirstOrDefault(d => d.ID.Equals(playerId));
            if (player != null)
                Players.Remove(player);
            playerReadyDic.Remove(playerId);
        }
    }
}
