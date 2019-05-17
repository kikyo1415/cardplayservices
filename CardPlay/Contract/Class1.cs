using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PokerCardPlay.Contract
{

    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        string SayHello();
    }

    [ServiceBehavior]
    public class ServiceTest : IService
    {
        public string SayHello()
        {
            return "1231231313123";
        }
    }


    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface ICallBackService
    {
        [OperationContract]
        void Login(string name);


        [OperationContract]
        string SayHello();
    }



    public interface ICallback  //回调接口客服端实现
    {

        [OperationContract(IsOneWay = true)]//单向调用,不需要返回值
        void TestCallBack(string hello);
    }
}

