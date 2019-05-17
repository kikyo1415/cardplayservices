using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace PokerCardPlay.Net
{
    public class WcfClient<T> where T : class
    {
        public TContract ConnetNetTcpServer<TContract>(T instance, string url)
        {
            NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.None);
            var channel = new DuplexChannelFactory<TContract>(instance, tcpBinding, url);
            var service = channel.CreateChannel();
            return service;
        }

        public TContract ConnetWSHttpServer<TContract>(T instance, string url)
        {
            WSHttpBinding Binding = new WSHttpBinding(SecurityMode.None);
            var channel = new ChannelFactory<TContract>(Binding, url);
            var service = channel.CreateChannel();
            return service;
        }
    }
}
