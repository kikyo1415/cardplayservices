using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PokerCardPlay.Core
{
    internal class PokerCardScript
    {
        private CardScript scriptObj;
        public PokerCardScript()
        {
            scriptObj = new CardScript();
            XmlDocument doc = new XmlDocument();
            doc.Load("CardCharge.d0");
            scriptObj = XmlSerilzerTool.Deserialize<CardScript>(doc.OuterXml);
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
}
