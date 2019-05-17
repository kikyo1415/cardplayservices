using PokerCardPlay.Core;
using PokerCardPlay.GameService;
using PokerCardPlay.Net;
using PokerCardPlay.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketTestConsole
{
    class Program
    {

        static void Main(string[] args)
        {
            IServerNetWork network = new SocketServerNetWork();
            PokerGameServiceBase pgsb = new GameRoomService(network);
            pgsb.Open();
            Console.WriteLine("GameRoomService opened");
            Console.Read();
        }

    }

    public class TestService : PokerGameLongConService
    {

        public TestService(IServerNetWork network)
            : base(network)
        {

        }

        protected override void OnClientDisConnectService(Guid playerId)
        {
            throw new NotImplementedException();
        }
    }
}
