using PokerCardPlay.Core;
using PokerCardPlay.Net;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace PokerCardPlay.Service
{
    /// <summary>
    /// 服务基类。
    /// </summary>
    public abstract class PokerGameServiceBase
    {
        protected IServerNetWork netWork;
        #region Ctor

        public PokerGameServiceBase(IServerNetWork netWork)
        {
            this.netWork = netWork;
            netWork.RegistReceiveCallBack(OnRecvMsg);

        }
        #endregion

        public virtual void Open()
        {
            netWork.StartListen();
        }

        protected virtual void OnRecvMsg(Object sender, string contentMsg)
        {
            try
            {
                var contentData = JSONSerilizer.GetJsonNode(contentMsg, "NetWorkDataContent");
                var name = JSONSerilizer.GetJsonNode(contentData, "ActionName");
                var args = JSONSerilizer.GetJsonNode(contentData, "ActionPara");

                Type type = GetType();
                var methodsAll = type.GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                var targetMethod = methodsAll.FirstOrDefault(d =>
                {
                    var actionAtt = d.GetCustomAttributes(typeof(NetActionAttribute), true);
                    if (actionAtt.Length > 0)
                    {
                        var actionname = ((NetActionAttribute)actionAtt[0]).ActionName;
                        return actionname.Equals(name);
                    }
                    else
                    {
                        return false;
                    }
                });
                //TODO:将方法返回值输出发送到客户端
                if (targetMethod != null)
                {
                    if (name.Equals("HeartBeat"))//心跳包单独处理。
                        targetMethod.Invoke(this, new object[] { sender, args });
                    else
                    {
                        Object resultJson = targetMethod.Invoke(this, new object[] { args });
                        netWork.SendMsg(sender, resultJson.ToString());//向客户端返回结果。
                    }
                }

            }
            catch (Exception ex)
            {
               // throw ex;
            }

        }
    }

    /// <summary>
    /// 长连接服务基类。
    /// </summary>
    public abstract class PokerGameLongConService : PokerGameServiceBase
    {
        //protected ConcurrentDictionary<Guid, OnlineStateObject> ConnectingClientDic
        //{
        //    get
        //    {
        //        return _connectingClientDic;
        //    }
        //}
        protected ConcurrentDictionary<Guid, OnlineStateObject> _connectingClientDic;
        /// <summary>
        /// 检查客户端在线状态线程。
        /// </summary>
        private System.Threading.Thread thr;

        public PokerGameLongConService(IServerNetWork network)
            : base(network)
        {
            _connectingClientDic = new ConcurrentDictionary<Guid, OnlineStateObject>();
            Init();
        }

        public override void Open()
        {
            thr.Start();
            base.Open();
        }

        /// <summary>
        /// 获取所有已连接本服务的客户端列表。
        /// </summary>
        /// <returns></returns>
        public List<Guid> GetConnectingPlayerClients()
        {
            return _connectingClientDic.Keys.ToList();
        }

        /// <summary>
        /// 接收心跳包。
        /// </summary>
        /// <param name="paraJson"></param>
        [NetAction("HeartBeat")]
        protected virtual void HeartBeat(Object sender, string argJson)
        {
            var playerIdStr = JSONSerilizer.GetJsonNode(argJson, "playerId");
            Guid playerId = Guid.Empty;
            if (Guid.TryParse(playerIdStr, out playerId))
            {
                OnlineStateObject obj = new OnlineStateObject { LastHeartBeatTime = DateTime.Now, OnlineWorkObject = sender };
                _connectingClientDic.AddOrUpdate(playerId, obj, (key, value) => obj);
            }
        }

        protected virtual void SendToPlayer(Guid playerId, string msg)
        {
            try
            {
                OnlineStateObject stateObj = null;
                if (_connectingClientDic.TryGetValue(playerId, out stateObj))
                    if (stateObj.OnlineWorkObject != null)
                        netWork.SendMsg(stateObj.OnlineWorkObject, msg);
            }
            catch (Exception)
            {
                throw;
            }

        }

        protected virtual void SendToAllPlayer(string msg)
        {
            try
            {
                foreach (var item in _connectingClientDic)
                {
                    if (item.Value != null)
                        if (item.Value.OnlineWorkObject != null)
                            netWork.SendMsg(item.Value.OnlineWorkObject, msg);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        protected abstract void OnClientDisConnectService(Guid playerId);

        private void Init()
        {
            thr = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                while (true)
                {
                    try
                    {
                        DateTime now = DateTime.Now;
                        var outOfTimeObjKeys = _connectingClientDic.Where(d =>
                        {
                            return (DateTime.Now - d.Value.LastHeartBeatTime).TotalSeconds > 10;
                        }).Select(d => d.Key).ToList();

                        foreach (var key in outOfTimeObjKeys)
                        {
                            OnlineStateObject removeObj = null;
                            _connectingClientDic.TryRemove(key, out removeObj);
                            OnClientDisConnectService(key);
                        }

                        System.Threading.Thread.Sleep(10000);
                    }
                    catch (Exception ex)
                    {

                        //throw;
                    }


                }
            }));
        }

        protected class OnlineStateObject
        {
            public DateTime LastHeartBeatTime { get; set; }

            public Object OnlineWorkObject { get; set; }
        }
    }
}
