using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PokerCardPlay.Core
{

    public interface IPlayer
    {
        /// <summary>
        /// ID。
        /// </summary>
        Guid ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="card"></param>
        void GiveCard(PokerCard card);

        /// <summary>
        /// 获取牌手的所有手牌。
        /// </summary>
        /// <returns></returns>
        PokerCard[] GetCards();


        void ClearCards();

        /// <summary>
        /// 获取牌手的筹码数。
        /// </summary>
        /// <returns>筹码数。</returns>
        long GetJettonsCount();

        /// <summary>
        /// 赢得筹码。
        /// </summary>
        /// <param name="jettons">筹码数。</param>
        void GainJettons(int jettons);

        /// <summary>
        /// 输掉筹码。
        /// </summary>
        /// <returns>筹码数。</returns>
        void LostJettons(int jettons);

        /// <summary>
        /// 转换为庄家。
        /// </summary>
        void ConvertToBanker();

        /// <summary>
        /// 转换为闲家。
        /// </summary>
        void ConvertToPlayer();

        bool IsBanker { get; }
    }

    /// <summary>
    /// 闲家接口。
    /// </summary>
    public interface IPokerPlayer : IPlayer
    {
        /// <summary>
        /// 下注。
        /// </summary>
        /// <param name="jettons">筹码数。</param>
        void Bet(long jettons);
    }

    public interface IPokerBanker : IPlayer
    {

    }

    public class Player : IPlayer
    {
        private List<PokerCard> Cards;
        private long playerJettons;
        private Guid _id;
        private bool isBanker;

        public bool IsBanker
        {
            get { return isBanker; }
        }

        public Guid ID { get { return _id; } set { _id = value; } }

        public string Name { get; set; }

        public Player()
        {
            _id = Guid.NewGuid();
            Cards = new List<PokerCard>();
        }

        public void GiveCard(PokerCard card)
        {
            Cards.Add(card);
        }

        public PokerCard[] GetCards()
        {
            return Cards.ToArray();
        }

        public long GetJettonsCount()
        {
            return playerJettons;
        }

        public void GainJettons(int jettons)
        {
            jettons += jettons;
        }

        public void LostJettons(int jettons)
        {
            jettons -= jettons;
        }


        public void ConvertToBanker()
        {
            isBanker = true;
        }

        public void ConvertToPlayer()
        {
            isBanker = false;
        }


        public void ClearCards()
        {
            this.Cards.Clear();
        }
    }

    public class PokerBanker : Player, IPokerBanker { }

    public class PokerPlayer : Player, IPokerPlayer
    {
        public void Bet(long jettons)
        {
            throw new NotImplementedException();
        }
    }

    public enum BetStyle
    {
        /// <summary>
        /// 劈。
        /// </summary>
        [Description("劈")]
        HalfOfCurrentPot,
        /// <summary>
        /// 总劈。
        /// </summary>
        [Description("总劈")]
        HalfOfTotalPot,
        /// <summary>
        /// 余。
        /// </summary>
        [Description("余")]
        LeaveOfCurrentPot,
        /// <summary>
        /// 总余。
        /// </summary>
        [Description("总余")]
        LeaveOfTotalPot,
        /// <summary>
        /// 打坛底。
        /// </summary>
        [Description("打坛底")]
        AllOfCurrentPot,
        /// <summary>
        /// 总打。
        /// </summary>
        [Description("总打")]
        AllOfTotalPot,
        /// <summary>
        /// 基数。
        /// </summary>
        [Description("基数")]
        Cardinal,
        /// <summary>
        /// 活注。
        /// </summary>
        [Description("活注")]
        Custom,

    }

    public class Betting
    {
        public BetStyle Style { get; set; }
        public int BetValue { get; set; }
        public bool IsLazy
        {
            get
            {
                return Style == BetStyle.AllOfTotalPot || Style == BetStyle.HalfOfTotalPot;
            }
        }
    }
}
