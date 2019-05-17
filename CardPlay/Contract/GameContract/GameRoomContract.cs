using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace PokerCardPlay.Contract
{
    [ServiceContract(CallbackContract = typeof(IGameClientCallBackContract))]
    public interface IGameRoomContract
    {
        [OperationContract]
        bool PlayerJoin(Guid playerId);

        [OperationContract]
        bool PlayerGetReady();


        //GameSituation GetGameCurrentSituation();




    }

    [DataContract]
    public class GameSituation
    {

    }

    public interface IGameClientCallBackContract
    {
        /// <summary>
        /// 玩家到齐，提供准备操作。
        /// </summary>
        [OperationContract(IsOneWay = true)]//单向调用,不需要返回值
        void ConfirmReady();

        /// <summary>
        /// 用户消息提醒。
        /// </summary>
        [OperationContract(IsOneWay = true)]//单向调用,不需要返回值
        void MessageNotice(string message);

        /// <summary>
        /// 获取用户昵称。
        /// </summary>
        /// <returns></returns>
        [OperationContract(IsOneWay = false)]
        string GetPlayerName();
    }
}
