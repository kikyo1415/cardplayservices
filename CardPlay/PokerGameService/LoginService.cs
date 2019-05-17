using PokerCardPlay.Net;
using PokerCardPlay.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerCardPlay.GameService
{
    public class LoginService : PokerGameServiceBase
    {

        public LoginService(IServerNetWork network)
            : base(network)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="para"></param>
        [NetAction("Login")]
        protected void PlayerLogin(string para)
        {

        }
    }
}
