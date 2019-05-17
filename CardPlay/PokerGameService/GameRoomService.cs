using PokerCardPlay.Core;
using PokerCardPlay.Net;
using PokerCardPlay.Service;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace PokerCardPlay.GameService
{

    /// <summary>
    /// 房间服务，对应一个游戏房间。
    /// </summary>
    public sealed class GameRoomService : PokerGameLongConService
    {
        private GameRoomManager roomManager;

        public GameRoomService(IServerNetWork network)
            : base(network)
        {
            roomManager = GameRoomManager.GetInstance();
        }

        [NetAction("CreateGameRoom")]
        public string CreateGameRoom(string args)
        {

            try
            {
                roomManager.CreateRoom(SendToPlayer);
                return "{ 'IsSuccess':1}";
            }
            catch (Exception ex)
            {
                return "{ 'IsSuccess':0,'msg':'" + ex.Message + "'}";
            }

        }


        [NetAction("JoinGameRoom")]
        public string JoinGameRoom(string args)
        {
            try
            {
                var playerIdStr = JSONSerilizer.GetJsonNode(args, "playerId");
                var roomNum = JSONSerilizer.GetJsonNode(args, "roomNum");
                string hasJoinPlayers = roomManager.JoinRoom(Guid.Parse(playerIdStr), Convert.ToInt32(roomNum));
                //SendJoin();
                return "{ 'IsSuccess':1,'HasJoinPlayer':'" + hasJoinPlayers + "'}";
            }
            catch (Exception ex)
            {
                return "{ 'IsSuccess':0,'msg':'" + ex.Message + "'}";
            }

        }

        [NetAction("Ready")]
        public string PlayerReady(string args)
        {

            try
            {
                var playerIdStr = JSONSerilizer.GetJsonNode(args, "playerId");
                var room = roomManager.GetRoomByPlayerId(Guid.Parse(playerIdStr));
                room.PlayerGetReady(Guid.Parse(playerIdStr));
                return "{ 'IsSuccess':1}";
            }
            catch (Exception ex)
            {
                return "{ 'IsSuccess':0,'msg':'" + ex.Message + "'}";
            }

        }

        [NetAction("BetOn")]
        public string PlayerBetOn(string args)
        {
            try
            {
                var playerIdStr = JSONSerilizer.GetJsonNode(args, "playerId");
                var betStyle = (BetStyle)Convert.ToInt32(JSONSerilizer.GetJsonNode(args, "betStyleInt"));
                var betValue = Convert.ToInt32(JSONSerilizer.GetJsonNode(args, "betValueInt"));
                Betting betting = new Betting() { BetValue = betValue, Style = betStyle };
                var room = roomManager.GetRoomByPlayerId(Guid.Parse(playerIdStr));
                room.PlayerBetOn(Guid.Parse(playerIdStr), betting);
                return "{ 'IsSuccess':1}";
            }
            catch (Exception ex)
            {
                return "{ 'IsSuccess':0,'msg':'" + ex.Message + "'}";
            }
        }

        [NetAction("GetBankerIndex")]
        public string GetBankerIndex(string args)
        {
            try
            {
                var playerIdStr = JSONSerilizer.GetJsonNode(args, "playerId");

                var room = roomManager.GetRoomByPlayerId(Guid.Parse(playerIdStr));

                int bankerIndex = room.GetBankerIndex();
                return "{ 'IsSuccess':1,'BankerIndex':" + bankerIndex + "}";
            }
            catch (Exception ex)
            {
                return "{ 'IsSuccess':0,'msg':'" + ex.Message + "'}";
            }
        }

        public string GetPlayersCards(string args)
        {
            try
            {
                var playerIdStr = JSONSerilizer.GetJsonNode(args, "playerId");

                var room = roomManager.GetRoomByPlayerId(Guid.Parse(playerIdStr));

                var cards = room.GetPlayerCards(Guid.Parse(playerIdStr));
                return "{'cards':{'ActionName':'GetCards','card1':'" + cards[0] + "','':'" + cards[1] + "'}}";
            }
            catch (Exception ex)
            {
                return "{ 'IsSuccess':0,'msg':'" + ex.Message + "'}";
            }
        }

        protected override void OnClientDisConnectService(Guid playerId)
        {
            try
            {
                //find player's room then clear him out to room
                var room = roomManager.GetRoomByPlayerId(playerId);
                //var player = room.Players.FirstOrDefault(d => d.ID.Equals(playerId));
                room.RemovePlayer(playerId);
            }
            catch (Exception)
            {
                throw;
            }

        }
    }


}
