using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace PokerCardPlay.Net
{
    public class WcfService
    {
        public void StartUpNetTcpServer<TContract, TService>(string hostUrl) where TService : TContract
        {
            ServiceHost host = new ServiceHost(typeof(TService));
            //TCP方式  
            NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.None);
            host.AddServiceEndpoint(typeof(TContract), tcpBinding, hostUrl);
            // 打开服务  
            host.Open();
        }

        public void StartUpNetTcpServer<TContract>(object instance, string hostUrl)
        {
            ServiceHost host = new ServiceHost(instance);
            //TCP方式  
            NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.None);
            host.AddServiceEndpoint(typeof(TContract), tcpBinding, hostUrl);
            // 打开服务  
            host.Open();
        }

        public void StartUpWSHttpServer<TContract, TService>(string hostUrl)
        {
            ServiceHost host = new ServiceHost(typeof(TService));
            //Http方式  
            WSHttpBinding Binding = new WSHttpBinding(SecurityMode.None);
            host.AddServiceEndpoint(typeof(TContract), Binding, hostUrl);

            ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();

            behavior.HttpGetEnabled = true;

            behavior.HttpGetUrl = new Uri(hostUrl);

            host.Description.Behaviors.Add(behavior);   // myServiceHost是ServiceHost实例


            // 打开服务  
            host.Open();
        }
    }
}
