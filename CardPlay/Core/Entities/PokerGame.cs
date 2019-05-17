using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerCardPlay.Core
{
    /// <summary>
    /// 牌局一次比赛。
    /// </summary>
    public class PokerGameMatch
    {
        #region Properties
        #endregion

        #region Fields
        private List<IPlayer> players;

        private int bankerIndex;
        private int firstBetPlayerIndex;
        private int currentBetPlayerIndex;
        private int roundIndex;

        private GameRound gameRound;
        private GameScoreBoard scoreBoard;

        private Dictionary<Guid, Betting> playerBetDic;

        private int cardinal, newpot;
        #endregion

        #region Ctor
        public PokerGameMatch(List<IPlayer> players, int cardinal, int newpot)
        {
            bankerIndex = 0;
            roundIndex = 0;
            this.players = players;
            this.cardinal = cardinal;
            this.newpot = newpot;
            scoreBoard = new GameScoreBoard(players.Select(d => d.ID).ToArray());
        }
        #endregion

        #region Methods

        public void StartNewRound()
        {
            gameRound = new GameRound();
            InitRound();
        }

        private void InitRound()
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (i.Equals(bankerIndex))
                    players[i].ConvertToBanker();
                else
                    players[i].ConvertToPlayer();
            }
            //playerTotalBetJettons = 0;
            scoreBoard.OnBettingPotJettons = scoreBoard.RealTimePotJettons;
            playerBetDic = new Dictionary<Guid, Betting>();
            currentBetPlayerIndex = firstBetPlayerIndex = (bankerIndex + 1) % 5;

            gameRound.Deal(players);//发牌。
            scoreBoard.BankerBetOn(players[bankerIndex].ID, newpot);

        }

        public List<BetStyle> GetPlayerBetStyles(IPlayer player)
        {
            var playerIndex = players.IndexOf(player);
            List<BetStyle> result = new List<BetStyle>();
            if (firstBetPlayerIndex - bankerIndex == 1)//第一圈
            {
                if (playerIndex - firstBetPlayerIndex == 0)
                    result.Add(BetStyle.HalfOfCurrentPot);
                else if (playerIndex - firstBetPlayerIndex == 1)
                    result.Add(BetStyle.LeaveOfCurrentPot);
                else if (playerIndex - firstBetPlayerIndex == 2)
                    result.Add(BetStyle.AllOfCurrentPot);
                else if (playerIndex - firstBetPlayerIndex == 3)
                    result.Add(BetStyle.Cardinal);
                else
                {
                    result.Add(BetStyle.LeaveOfCurrentPot);
                    result.Add(BetStyle.AllOfCurrentPot);
                    result.Add(BetStyle.Cardinal);
                    result.Add(BetStyle.Custom);
                }
            }
            else //第二圈起
            {
                if (playerIndex - firstBetPlayerIndex == 0)
                {
                    result.Add(BetStyle.HalfOfCurrentPot);
                    result.Add(BetStyle.AllOfCurrentPot);
                    result.Add(BetStyle.Custom);
                }
                else
                {
                    bool flag = playerBetDic.Values.Any(d => d.IsLazy);//是否有延迟计算的，有则无法再P。
                    if (!flag)
                    {
                        var totalPlayerBet = playerBetDic.Values.Sum(d => d.BetValue);
                        if (totalPlayerBet < scoreBoard.OnBettingPotJettons)
                            result.Add(BetStyle.LeaveOfCurrentPot);
                    }
                    result.Add(BetStyle.HalfOfTotalPot);
                    result.Add(BetStyle.AllOfCurrentPot);
                    result.Add(BetStyle.AllOfTotalPot);
                    result.Add(BetStyle.Custom);
                    result.Add(BetStyle.Cardinal);
                }
            }

            return result;
        }

        public GameScoreBoard GetScoreBorad()
        {
            return scoreBoard;
        }

        public int GetBankerIndex()
        {
            return bankerIndex;
        }

        public IPlayer GetOnBetPlayer()
        {
            return players[currentBetPlayerIndex];
        }

        public void PlayerBetOn(IPlayer player, Betting betting)
        {
            //playerTotalBetJettons += betting.BetValues;
            playerBetDic.Add(player.ID, betting);

            //下注后currentBetPlayerIndex自增1并对5取余，计算的结果如果是bankerIndex则再自增1。
            currentBetPlayerIndex++;
            currentBetPlayerIndex = (currentBetPlayerIndex % 5);
            currentBetPlayerIndex = currentBetPlayerIndex == bankerIndex ? currentBetPlayerIndex + 1 : currentBetPlayerIndex;

        }

        /// <summary>
        /// 结算
        /// </summary>
        /// <param name="player"></param>
        public void Account(IPlayer player)
        {
            if (player.IsBanker)
                return;
            //比牌
            var banker = players.FirstOrDefault(d => d.IsBanker);
            //计算实时注。
            var bettingEnt = playerBetDic[player.ID];
            int realTimeBetValue = 0;
            if (bettingEnt.IsLazy)
            {
                switch (bettingEnt.Style)
                {
                    case BetStyle.HalfOfTotalPot:
                        realTimeBetValue = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(scoreBoard.RealTimePotJettons) / 200) * 100);
                        break;
                    case BetStyle.AllOfTotalPot:
                        realTimeBetValue = Convert.ToInt32(scoreBoard.RealTimePotJettons);
                        break;
                    default:
                        realTimeBetValue = cardinal;
                        break;
                }
            }
            else
            {
                realTimeBetValue = bettingEnt.BetValue;
            }

            bool isdouble;
            bool isbankerWin = gameRound.HeadsUp(banker, player, out isdouble);
            if (isbankerWin)
            {
                int gaisJettons = scoreBoard.RealTimePotJettons < realTimeBetValue ? scoreBoard.RealTimePotJettons : realTimeBetValue;
                scoreBoard.AccoutScore(player.ID, -gaisJettons);
            }
            else
            {
                int tempJettons = isdouble ? realTimeBetValue * 2 : realTimeBetValue;
                int gainsJettons = scoreBoard.RealTimePotJettons < tempJettons ? scoreBoard.RealTimePotJettons : tempJettons;

                scoreBoard.AccoutScore(player.ID, gainsJettons);
            }
        }

        public void NextRound()
        {
            StartNewRound();
            roundIndex++;
        }

        public void GotoNextBanker()
        {
            bankerIndex++;
            bankerIndex = bankerIndex > 4 ? bankerIndex - 5 : bankerIndex;
        }
        #endregion

    }

    public class GameScoreBoard
    {
        private List<Dictionary<Guid, int>> accountRecords;
        private Dictionary<Guid, int> playerCurrentScore;

        /// <summary>
        /// 下注时奖池（仅每局变化）。
        /// </summary>
        public int OnBettingPotJettons { get; set; }

        public int RealTimePotJettons
        {
            get { return potJettons; }
        }

        /// <summary>
        /// 实时奖池（每次结算变化）。
        /// </summary>
        private int potJettons;

        public GameScoreBoard(Guid[] playerIds)
        {
            accountRecords = new List<Dictionary<Guid, int>>();
            playerCurrentScore = new Dictionary<Guid, int>();
            foreach (var id in playerIds)
                playerCurrentScore.Add(id, 0);
        }

        public void BankerBetOn(Guid bankerId, int betJettons)
        {
            playerCurrentScore[bankerId] -= betJettons;
            potJettons += betJettons;
        }

        public void AccoutScore(Guid playerId, int jettons)
        {
            playerCurrentScore[playerId] += jettons;
            potJettons -= jettons;
        }
    }

    internal class GameRound
    {
        private PokerCardTable pct;
        #region Pirvate Methods

        public GameRound()
        {
            InitGameRound();
        }

        private void InitGameRound()
        {
            CreateCardTable();
            pct.Shuffle();
        }

        private void CreateCardTable()
        {
            pct = new PokerCardTable();
        }

        /// <summary>
        /// 向玩家发牌。
        /// </summary>
        public void Deal(List<IPlayer> players)
        {
            foreach (var player in players)
            {
                player.GiveCard(pct.DealCard());
                player.GiveCard(pct.DealCard());
            }
        }

        /// <summary>
        /// 庄家与闲家对决。
        /// </summary>
        /// <returns>true：庄家赢，false：闲家赢。</returns>
        public bool HeadsUp(IPlayer banker, IPlayer player, out bool isDouble)
        {
            PokerCard[] cards_banker = banker.GetCards();
            PokerCard[] cards_player = player.GetCards();
            bool isDouble_banker = cards_banker[0].Point == cards_banker[1].Point;
            bool isDouble_player = cards_player[0].Point == cards_player[1].Point;
            bool isBankerPointWin =
                (cards_banker.Sum(d => d.Point) >= 10 ? cards_banker.Sum(d => d.Point) - 10 : cards_banker.Sum(d => d.Point))
                >=
                    (cards_player.Sum(d => d.Point) >= 10 ? cards_player.Sum(d => d.Point) - 10 : cards_player.Sum(d => d.Point));
            if (!isDouble_banker && !isDouble_player)
            {
                isDouble = false;
                return isBankerPointWin;
            }
            else if (isDouble_banker && !isDouble_player)
            {
                isDouble = false;
                return true;
            }
            else if (!isDouble_banker && isDouble_player)
            {
                isDouble = true;
                return false;
            }
            else if (isDouble_banker && isDouble_player)
            {
                isDouble = true;
                return isBankerPointWin;
            }
            else
            {
                isDouble = false;
                return false;
            }

        }


        #endregion
    }


    /// <summary>
    /// 牌局一回合。
    /// </summary>
    public class PokerGameRound
    {
        #region Properties
        public PokerGameRoundState State
        {
            get
            {
                return state;
            }
        }
        public long PotJettons { get { return potJettons; } set { this.potJettons = value; } }
        #endregion

        #region Fields
        private PokerGameRoundState state;
        private PokerCardTable pct;
        private IPokerBanker gamebanker;
        private List<IPokerPlayer> gameplayers;

        private List<IPlayer> players;
        private long potJettons;
        #endregion

        #region Ctor
        public PokerGameRound()
        {
            InitGame();
        }

        public PokerGameRound(List<IPlayer> players)
        {
            this.players = players;
            InitGame();
        }

        public PokerGameRound(IPokerBanker banker, List<IPokerPlayer> players)
        {
            this.gamebanker = banker;
            this.gameplayers = players;
            InitGame();
        }
        #endregion

        #region Methods
        /// <summary>
        /// 发牌。
        /// </summary>
        public void Deal()
        {
            gamebanker.GiveCard(pct.DealCard());
            gamebanker.GiveCard(pct.DealCard());
            for (int i = 0; i < 4; i++)
            {
                gameplayers[i].GiveCard(pct.DealCard());
                gameplayers[i].GiveCard(pct.DealCard());
            }
        }

        public PokerCard[] ScriptedDeal(int value)
        {
            return pct.DealCardWithScript(value);
        }

        public void ScriptedDeal(PokerCard referCard) { }

        /// <summary>
        /// 闲家下注。
        /// </summary>
        /// <param name="player"></param>
        /// <param name="betJettons"></param>
        public void PlayerBet(IPokerPlayer player, int betJettons)
        {
            player.Bet(betJettons);
        }

        /// <summary>
        /// 庄家与闲家对决。
        /// </summary>
        /// <returns>true：庄家赢，false：闲家赢。</returns>
        public bool HeadsUp(IPokerPlayer player, out bool isDouble)
        {
            PokerCard[] cards_banker = gamebanker.GetCards();
            PokerCard[] cards_player = player.GetCards();
            bool isDouble_banker = cards_banker[0].Point == cards_banker[1].Point;
            bool isDouble_player = cards_player[0].Point == cards_player[1].Point;
            bool isBankerPointWin =
                (cards_banker.Sum(d => d.Point) >= 10 ? cards_banker.Sum(d => d.Point) - 10 : cards_banker.Sum(d => d.Point))
                >=
                    (cards_player.Sum(d => d.Point) >= 10 ? cards_player.Sum(d => d.Point) - 10 : cards_player.Sum(d => d.Point));
            if (!isDouble_banker && !isDouble_player)
            {
                isDouble = false;
                return isBankerPointWin;
            }
            else if (isDouble_banker && !isDouble_player)
            {
                isDouble = false;
                return true;
            }
            else if (!isDouble_banker && isDouble_player)
            {
                isDouble = true;
                return false;
            }
            else if (isDouble_banker && isDouble_player)
            {
                isDouble = true;
                return isBankerPointWin;
            }
            else
            {
                isDouble = false;
                return false;
            }

        }

        public PokerCard DealACardToPlayer()
        {
            return pct.DealCard();
        }
        #endregion

        #region Pirvate Methods
        private void InitGame()
        {
            CreateCardTable();
            pct.Shuffle();
            potJettons = 400;
        }

        private void CreateCardTable()
        {
            pct = new PokerCardTable();
        }
        #endregion

    }

    /// <summary>
    /// 一圈牌的状态。
    /// </summary>
    public enum PokerGameRoundState
    {
        /// <summary>
        /// 发牌阶段。
        /// </summary>
        Dealing,

        /// <summary>
        /// 下注阶段。
        /// </summary>
        Betting,

        /// <summary>
        /// 结算阶段。
        /// </summary>
        Accounting,

        /// <summary>
        /// 结束。
        /// </summary>
        End
    }

    public class PokerGameBetContext
    {
        public BetStyle Style { get; set; }
        public int BetJettons { get; set; }

        public PokerGameBetContext()
        {

        }

        public PokerGameBetContext(BetStyle style, int betJettons)
        {

        }
    }
}
