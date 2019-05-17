using PokerCardPlay.Contract;
using PokerCardPlay.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Common;

namespace TestConsoleClient
{
    class Program
    {

        static TcpClient client;
        static string playerId = Guid.NewGuid().ToString();
        static void Main(string[] args)
        {

            string rzdata = new Excel_Act().GetJson();
            //WcfConnet();
            int a = 1000;
            var bytessss = Encoding.Default.GetBytes(a.ToString());

            SocketConnet();

            Thread thrHeart = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    SendHeartBeat();
                    Thread.Sleep(2000);
                }
            }));
            thrHeart.Start();

            Thread thrRecv = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    byte[] header = new byte[8];
                    client.GetStream().Read(header, 0, 8);

                    int contentLength = Convert.ToInt32(Encoding.UTF8.GetString(header, 0, 8));
                    byte[] content = new byte[contentLength];
                    client.GetStream().Read(content, 0, contentLength);

                    string str = Encoding.UTF8.GetString(content, 0, contentLength);

                    Console.WriteLine("socket-quanju receive:" + str);
                }
            }));
            thrRecv.Start();


            while (true)
            {
                Console.WriteLine("1.create room;2.join room;3.getready");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        InvokeNetAction("CreateGameRoom", "{'playerId':'" + playerId + "','roomNum':'10086'}");
                        break;
                    case "2":
                        InvokeNetAction("JoinGameRoom", "{'playerId':'" + playerId + "','roomNum':'10086'}");
                        break;
                    case "3":
                        InvokeNetAction("Ready", "{'playerId':'" + playerId + "','roomNum':'10086'}");
                        break;
                    default:
                        break;
                }
            }

        }

        private static void InvokeNetAction(string actionName, string actionArg)
        {
            using (TcpClient localClient = new TcpClient())
            {
                string content = "{'NetWorkDataContent':{'ActionName':'" + actionName + "','ActionPara':" + actionArg + "}}";
                SocketConnet(localClient);
                SendMsgToServer(localClient, content);


                byte[] header = new byte[8];
                localClient.Client.Receive(header);

                int contentLength = Convert.ToInt32(Encoding.UTF8.GetString(header, 0, 8));
                byte[] reccontent = new byte[contentLength];
                localClient.Client.Receive(reccontent);

                string str = Encoding.UTF8.GetString(reccontent, 0, contentLength);

                Console.WriteLine("socket-jubu receive:" + str);
            }
        }

        private static void SocketConnet()
        {
            client = new TcpClient();
            client.Connect(IPAddress.Parse("192.168.0.103"), 12456);
        }

        private static void SocketConnet(TcpClient client)
        {
            client.Connect(IPAddress.Parse("192.168.0.103"), 12456);
        }

        private static void SendHeartBeat()
        {
            //string content = "<NetWorkDataContent><ActionName>GetInRoom</ActionName><ActionPara><aaa>12313</aaa><bbb>345345</bbb></ActionPara></NetWorkDataContent>";
            //string content = "{'NetWorkDataContent':{'ActionName':'GetInRoom','ActionPara':{'aaa':'111','bbb':'222'}}}";
            string content = "{'NetWorkDataContent':{'ActionName':'HeartBeat','ActionPara':{'playerId':'" + playerId + "'}}}";
            SendMsgToServer(client, content);
        }

        private static void SendMsgToServer(TcpClient client, string content)
        {
            try
            {
                int contentLength = content.Length;
                byte[] bytesHeader = new byte[8];
                int result = Encoding.UTF8.GetBytes(contentLength.ToString(), 0, contentLength.ToString().Length, bytesHeader, 0);
                byte[] bytesContent = Encoding.UTF8.GetBytes(content);
                byte[] bytesAll = new byte[8 + contentLength];
                bytesHeader.CopyTo(bytesAll, 0);
                bytesContent.CopyTo(bytesAll, 8);
                client.Client.Send(bytesAll);
            }
            catch (Exception)
            {

            }

        }

    }
}
