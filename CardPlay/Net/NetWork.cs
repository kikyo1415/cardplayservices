using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PokerCardPlay.Net
{
    /// <summary>
    /// 服务端网络通信接口。
    /// </summary>
    public interface IServerNetWork
    {
        void StartListen();

        void RegistReceiveCallBack(Action<Object, string> OnReceiveMsg);

        void SendMsg(Object client, string content);
    }

    public interface IDuplexServerNetWork : IServerNetWork
    {

    }

    public class SocketDuplexServerNetWork : SocketServerNetWork, IDuplexServerNetWork
    {

        public SocketDuplexServerNetWork()
            : base()
        {
        }

        public void SendMsg(Object client, string content)
        {
            throw new NotImplementedException();
        }
    }

    public class SocketServerNetWork : IServerNetWork
    {

        protected TcpListener listener;

        protected Action<Object, string> recvCallBack;

        public SocketServerNetWork()
        {

        }

        public virtual void StartListen()
        {
            listener = new TcpListener(IPAddress.Parse("192.168.0.103"), 12456);

            listener.Start();

            listener.BeginAcceptTcpClient(ConnectCallBack, listener);
        }


        public virtual void RegistReceiveCallBack(Action<Object, string> OnReceiveMsg)
        {
            recvCallBack = OnReceiveMsg;
        }

        protected virtual void ConnectCallBack(IAsyncResult ar)
        {
            var serverlistener = ar.AsyncState as TcpListener;
            TcpClient client = serverlistener.EndAcceptTcpClient(ar);
            //异步接收
            StateObject state = new StateObject();
            state.workSocket = client;
            // Begin receiving the data from the remote device.     
            client.Client.BeginReceive(state.headbuffer, 0, StateObject.HeadBufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            listener.BeginAcceptTcpClient(ConnectCallBack, serverlistener);
            Console.WriteLine("connneted");
        }

        protected virtual void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket     
                // from the asynchronous state object.     
                StateObject state = (StateObject)ar.AsyncState;
                TcpClient client = state.workSocket;
                var stream = client.GetStream();
                //Just read headdata from the remote device.     
                int headLength = client.Client.EndReceive(ar);
                //Check header length
                if (headLength == StateObject.HeadBufferSize)
                {
                    //Resolve headbuffer to get content length
                    int contentLength = Convert.ToInt32(Encoding.UTF8.GetString(state.headbuffer, 0, StateObject.HeadBufferSize));
                    //Read content
                    byte[] contentBuffer = new byte[contentLength];
                    int readContentLength = stream.Read(contentBuffer, 0, contentLength);
                    //Check content length
                    if (readContentLength == contentLength)
                    {
                        //Resolve content
                        string content = Encoding.UTF8.GetString(contentBuffer, 0, contentBuffer.Length);
                        recvCallBack(client, content);
                    }
                    // Get the rest of the data(Regist a new receive callback,wait for a new invoke from client-sender).
                    client.Client.BeginReceive(state.headbuffer, 0, StateObject.HeadBufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10054)
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        protected class StateObject
        {
            // Client socket.     
            public TcpClient workSocket = null;
            // Size of receive buffer.     
            public const int HeadBufferSize = 8;
            // Receive buffer.     
            public byte[] headbuffer = new byte[HeadBufferSize];
        }


        public void SendMsg(object client, string content)
        {
            TcpClient tcpclient = client as TcpClient;
            //content = content + "\n";//加入换行以适配Android端的readLine。
          
            byte[] bytesContent = Encoding.UTF8.GetBytes(content);
            int contentLength = bytesContent.Length;
            byte[] bytesHeader = new byte[8];
            int result = Encoding.UTF8.GetBytes(contentLength.ToString(), 0, contentLength.ToString().Length, bytesHeader, 0);
          
            byte[] bytesAll = new byte[8 + contentLength];
            bytesHeader.CopyTo(bytesAll, 0);
            bytesContent.CopyTo(bytesAll, 8);
            tcpclient.Client.Send(bytesAll);
        }
    }



    public class NetWorkDataContent
    {
        public string ActionName { get; set; }

        public string ActionPara { get; set; }
    }

    /// <summary>
    /// 指定方法的IServerNetWork方式远程调用特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class NetActionAttribute : Attribute
    {
        /// <summary>
        /// 初始化 <see cref="NetActionAttribute"/> 类的新实例并带有远程调用名称。
        /// </summary>
        /// <param name="name">远程调用名称</param>
        public NetActionAttribute(string name)
        {
            this.actionName = name;
        }

        private string actionName;

        /// <summary>
        ///获取或设置远程调用名称。
        /// </summary>
        public string ActionName
        {
            get { return actionName; }
            set { actionName = value; }
        }

    }

}
