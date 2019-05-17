using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Data;
using System.Xml;
using PokerCardPlay.Net;
using PokerCardPlay.Contract;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //var str = new Excel_Act().GetJson();
            //XmlDocument doc = new XmlDocument();
            //doc.Load("CardCharge.d0");
            //var scriptObj = XmlSerilzerTool.Deserialize<CardScript>(doc.OuterXml);

            StartWcfServer();
            Console.ReadKey();
        }


        private static void StartWcfServer()
        {
            WcfService service = new WcfService();
            service.StartUpWSHttpServer<IService, PokerCardPlay.Contract.ServiceTest>("http://127.0.0.1:56789/MyServiceTest");
        }
        private static void ImportXls()
        {
            Excel_Act ea = new Excel_Act();
            var ds = ea.ExecleDs("SCRIPT.xlsx");
            CardScript cardScript = new CardScript();
            cardScript.PlayerCardScripts = new List<PlayerCardScript>();
            foreach (DataTable table in ds.Tables)
            {
                PlayerCardScript playerScript = new PlayerCardScript();
                playerScript.ScriptItems = new List<ScriptItem>();
                foreach (DataRow item in table.Rows)
                {
                    ScriptItem scriptItem = new ScriptItem();
                    scriptItem.ReferCard = item["ReferCard"].ToString();
                    scriptItem.Card1 = item["Card1"].ToString();
                    scriptItem.Card2 = item["Card2"].ToString();
                    scriptItem.CurrentValue = item["CurrentValue"].ToString();
                    scriptItem.NextValue = item["NextValue"].ToString();
                    playerScript.ScriptItems.Add(scriptItem);
                }
                cardScript.PlayerCardScripts.Add(playerScript);
            }
            string xml = XmlSerilzerTool.Serializer(cardScript);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            doc.Save("CardCharge.d0");
        }
    }





    [Serializable]
    public class CardScript
    {
        public List<PlayerCardScript> PlayerCardScripts { get; set; }
    }

    [Serializable]
    public class PlayerCardScript
    {
        public List<ScriptItem> ScriptItems { get; set; }
    }

    [Serializable]
    public class ScriptItem
    {
        public string ReferCard { get; set; }

        public string Card1 { get; set; }

        public string Card2 { get; set; }

        public string NextValue { get; set; }

        public string CurrentValue { get; set; }
    }


    public class RZMD_DATA
    {
        public List<RZMD> RZMDS { get; set; }
    }

    [Serializable]
    public class RZMD
    {
        public string name { get; set; }

        public string num { get; set; }

        public string address { get; set; }

        public string tel { get; set; }

        public string path { get; set; }
    }

}
