using PokerCardPlay.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerCardPlay.GameService
{
    public class GameFlowController
    {
        public GameFlowController()
        {

        }

        public void StartMatch()
        {
            //实例化玩家实体players
            List<IPlayer> players = new List<IPlayer>
        {
            new Player() { Name = "ch" },
            new Player() { Name = "zjm" },
            new Player() { Name = "lyy" },
            new Player() { Name = "rxj" },
            new Player() { Name = "spx" },
        };
            PokerGameMatch match = new PokerGameMatch(players, 400, 100);

        }
    }
}
