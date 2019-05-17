using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PokerCardPlay.Core;
using System.Threading;
using System.Xml;

namespace TestFormClient
{
    public partial class Form1 : Form
    {

        private long PotJetton
        {
            get
            {
                return Convert.ToInt64(lb_pot.Text);
            }
            set
            {
                pot.PotJettons = value;
                lb_pot.Text = value.ToString();
            }
        }
        private long RoundStartPot;
        private Pot pot = new Pot();
        private List<IPlayer> players;
        private IPokerBanker banker;
        private List<IPokerPlayer> pokerplayers;
        private List<UserControl1> ucList;
        private int firstBetIndex;
        private int currentBetIndex;
        private int betCount;
        private PokerGameRound pgr;
        private bool flag_LeaveOfCurrentPot = false;
        private bool flag_HalfOfTotalPot = false;
        private bool flag_All = false;
        private int totalBetJettons;

        CardScript scriptObj;
        int DealCount = 0;
        List<UserControl1> ucListScript;

        public Form1()
        {
            InitializeComponent();
            ucList = new List<UserControl1>();
            ucList.Add(userControl11);
            userControl11.PlayerName = "ch";
            ucList.Add(userControl12);
            userControl12.PlayerName = "zjm";
            ucList.Add(userControl13);
            userControl13.PlayerName = "rxj";
            ucList.Add(userControl14);
            userControl14.PlayerName = "lyy";
            ucList.Add(userControl15);
            userControl15.PlayerName = "spx";
            currentBetIndex = firstBetIndex = 0;
            ucList[0].PlayerJettons -= 400;
            RoundStartPot = PotJetton = 400;

            ucListScript = new List<UserControl1>();
            ucListScript.Add(userControl11);
            ucListScript.Add(userControl12);
            ucListScript.Add(userControl13);
            ucListScript.Add(userControl14);
            ucListScript.Add(userControl15);
        }

        private void LoadScript()
        {
            scriptObj = new CardScript();
            XmlDocument doc = new XmlDocument();
            doc.Load("CardCharge.d0");
            scriptObj = XmlSerilzerTool.Deserialize<CardScript>(doc.OuterXml);

            scriptObj.PlayerCardScripts.Add(scriptObj.PlayerCardScripts[3]);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartNewRound();
        }

        private void StartNewRound()
        {
            InitRound();
            NextPlayerBet();
            totalBetJettons = 0;
        }

        private void NextPlayerBet()
        {
            currentBetIndex = currentBetIndex % 4;
            IPokerPlayer betPlayer = pokerplayers[currentBetIndex];

            var uc = ucList.FirstOrDefault(d => d.Player.ID == betPlayer.ID);

            uc.ClearComSelection();
            if (firstBetIndex == 0)//第一圈
            {
                if (betCount == 0) //第一圈第一位
                    uc.AddComSelection(BetStyle.HalfOfCurrentPot);
                else if (betCount == 1) //第一圈第二位
                    uc.AddComSelection(BetStyle.LeaveOfCurrentPot);
                else if (betCount == 2)
                    uc.AddComSelection(BetStyle.AllOfCurrentPot);
                else if (betCount == 3)
                    uc.AddComSelection(BetStyle.Cardinal);
                else
                {
                    uc.AddComSelection(BetStyle.LeaveOfCurrentPot);
                    uc.AddComSelection(BetStyle.AllOfCurrentPot);
                    uc.AddComSelection(BetStyle.Cardinal);
                    uc.AddComSelection(BetStyle.Custom);
                }
            }
            else //第二圈起
            {
                if (betCount == 0)//第一位
                {
                    uc.AddComSelection(BetStyle.HalfOfCurrentPot);
                    //uc.AddComSelection(BetStyle.HalfOfCurrentPot);
                    uc.AddComSelection(BetStyle.AllOfCurrentPot);
                    uc.AddComSelection(BetStyle.Custom);
                }
                else
                {
                    if (!flag_LeaveOfCurrentPot && !flag_HalfOfTotalPot && !flag_All)
                    {
                        if (ucList.Sum(d => d.BetJettons) < PotJetton)
                            uc.AddComSelection(BetStyle.LeaveOfCurrentPot);
                        //uc.AddComSelection(BetStyle.LeaveOfTotalPot);
                    }

                    if (!flag_HalfOfTotalPot)
                        uc.AddComSelection(BetStyle.HalfOfTotalPot);
                    if (!flag_All)
                    {
                        uc.AddComSelection(BetStyle.AllOfCurrentPot);
                        uc.AddComSelection(BetStyle.AllOfTotalPot);
                    }
                    uc.AddComSelection(BetStyle.Custom);
                    uc.AddComSelection(BetStyle.Cardinal);
                }
            }

            uc.StartBet(RoundStartPot, pot, totalBetJettons, 100);

            betCount++;
            uc.SetBetCallBack(new Action<BetStyle, int>((style, jpoint) =>
            {
                if (style == BetStyle.LeaveOfCurrentPot)
                    flag_LeaveOfCurrentPot = true;
                else if (style == BetStyle.HalfOfTotalPot)
                    flag_HalfOfTotalPot = true;
                else if (style == BetStyle.AllOfCurrentPot || style == BetStyle.AllOfTotalPot)
                    flag_All = true;
                totalBetJettons += jpoint;
                uc.EndBet();
                if (betCount >= 4)
                {
                    betCount = 0;
                    OverBet();
                    return;
                }
                else
                {
                    currentBetIndex++;
                    NextPlayerBet();
                }
            }));
        }

        private void OverBet()
        {
            //pgr.Deal();
            ucList[0].DrawPlayerHandCard();
            for (int i = 0; i < 4; i++)
            {
                int index = (firstBetIndex + i) % 4;
                //if (index == 0)
                //    index++;
                var uc = ucList.FirstOrDefault(d => d.Player.ID == pokerplayers[index].ID);
                if (uc.Player is IPokerBanker)
                    index++;
                uc.DrawPlayerHandCard();
                bool isDouble;
                bool isbankerWin = pgr.HeadsUp(uc.Player as IPokerPlayer, out isDouble);
                //long pot = Convert.ToInt64(lb_pot.Text);
                if (isbankerWin)
                {
                    //  int loseJettons=0;
                    long gainsJettons = PotJetton < uc.BetJettons ? PotJetton : uc.BetJettons;
                    // lb_pot.Text = (PotJetton + uc.BetJettons).ToString();
                    uc.PlayerLose(gainsJettons);
                    PotJetton = PotJetton + gainsJettons;
                }
                else
                {
                    //  long gainsJettons = 0;
                    int tempJettons = isDouble ? uc.BetJettons * 2 : uc.BetJettons;
                    long gainsJettons = PotJetton < tempJettons ? PotJetton : tempJettons;

                    //lb_pot.Text = PotJetton.ToString();

                    uc.PlayerWin(gainsJettons);
                    PotJetton = PotJetton - gainsJettons;
                    if (PotJetton == 0)
                    {
                        //banker sleep down!
                        MessageBox.Show("换庄", "提示", MessageBoxButtons.OK);
                        GotoNextBanker();
                        return;
                    }
                }
                this.Update();
                Thread.Sleep(1000);
            }
            //banker alive
            MessageBox.Show("下一局", "提示", MessageBoxButtons.OK);

            firstBetIndex++;
            currentBetIndex = firstBetIndex;
            StartNewRound();
        }

        private void GotoNextBanker()
        {
            ucList.Add(ucList[0]);
            ucList.RemoveAt(0);
            currentBetIndex = firstBetIndex = 0;
            RoundStartPot = PotJetton = 400;
            StartNewRound();
            lb_pot.Text = "400";
            ucList[0].PlayerJettons -= 400;

        }


        PokerCard[] scriptCardList_First = new PokerCard[5];
        PokerCard[,] scriptCardList_Second = new PokerCard[5, 2];

        private void InitRound()
        {
            LoadScript();
            flag_LeaveOfCurrentPot = false;
            flag_HalfOfTotalPot = false;

            RoundStartPot = PotJetton;
            banker = new PokerBanker();
            pokerplayers = new List<IPokerPlayer>();
            for (int i = 0; i < 4; i++)
                pokerplayers.Add(new PokerPlayer());
            players = new List<IPlayer>();
            players.Add(banker as IPlayer);//设定第0个是庄家
            foreach (var item in pokerplayers)
                players.Add(item as IPlayer);//闲家依次入链表

            ucList[0].BetJettons = 0;
            pgr = new PokerGameRound(banker, pokerplayers);

            for (int i = 0; i < players.Count; i++)
            {
                ucList[i].Player = players[i];
                ucList[i].DrawPlayerHandCard();
                ucList[i].EndBet();
                ucList[i].ClearCallBack();
                ucList[i].BetJettons = 0;
            }

            //Script
            if (firstBetIndex == 0)
            {
                pgr.Deal();
                //Save First Card
                for (int i = 0; i < players.Count; i++)
                    if (ucListScript[i].Player != null)
                        scriptCardList_First[i] = ucListScript[i].Player.GetCards()[0];
            }
            else if (firstBetIndex == 1)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    //ScriptCardList[i] = ucListScript[i].Player.GetCards()[0];
                    var currentScript = scriptObj.PlayerCardScripts[i].ScriptItems.Where(d => d.ReferCard == scriptCardList_First[i].ToFileName()).OrderBy(d => Guid.NewGuid()).First();
                    var card1Str = currentScript.Card1;
                    var card2Str = currentScript.Card2;

                    //var watToRemoveCardList=
                    scriptObj.PlayerCardScripts.ForEach(pcs =>
                    {
                        pcs.ScriptItems = pcs.ScriptItems.Where(d =>
                            d.Card1 != card1Str &&
                            d.Card2 != card2Str &&
                            d.Card1 != card1Str &&
                            d.Card2 != card2Str).ToList();
                    });

                    PokerCard card1 = new PokerCard((CardSuit)Enum.Parse(typeof(CardSuit), card1Str.Substring(0, 3)), Convert.ToInt32(card1Str.Substring(3, 2)));
                    PokerCard card2 = new PokerCard((CardSuit)Enum.Parse(typeof(CardSuit), card2Str.Substring(0, 3)), Convert.ToInt32(card2Str.Substring(3, 2)));

                    //Save Second Card
                    scriptCardList_Second[i, 0] = card1;
                    scriptCardList_Second[i, 1] = card2;
                    ucListScript[i].Player.ClearCards();
                    ucListScript[i].Player.GiveCard(card1);
                    ucListScript[i].Player.GiveCard(card2);
                }
            }
            else if (firstBetIndex == 2)
            {
                //pgr.ScriptedDeal(6);
                for (int i = 0; i < players.Count; i++)
                {
                    var point = scriptObj.PlayerCardScripts[i].ScriptItems.FirstOrDefault(d => d.ReferCard == scriptCardList_First[i].ToFileName()
                        && d.Card1 == scriptCardList_Second[i, 0].ToFileName()
                        && d.Card2 == scriptCardList_Second[i, 1].ToFileName()).NextValue;

                    var cards = pgr.ScriptedDeal(Convert.ToInt32(point));

                    ucListScript[i].Player.ClearCards();
                    ucListScript[i].Player.GiveCard(cards[0]);
                    ucListScript[i].Player.GiveCard(cards[1]);
                }
            }
            else
            {
                pgr.Deal();
            }
            DealCount++;


            ucList.ForEach(uc =>
            {
                uc.DrawPlayerHandCard();
            });
        }
    }

    public class Pot
    {
        public long PotJettons { get; set; }
    }
}
