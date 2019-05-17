using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace PokerCardPlay.Core
{
    public class PokerCard
    {
        #region Properties
        /// <summary>
        /// 牌面花色。
        /// </summary>
        public CardSuit SuitKind
        {
            get
            {
                return suitkind;
            }
        }

        public bool IsHandle { get; internal set; }
        /// <summary>
        /// 牌面点数。
        /// </summary>
        public int Point
        {
            get
            {
                return _point;
            }
        }
        #endregion

        #region Feilds
        private int _point;
        private CardSuit suitkind;
        #endregion

        #region Ctor
        public PokerCard(CardSuit suit, int num)
        {
            this.suitkind = suit;
            this._point = num;
        }
        #endregion

        #region Methods
        /// <summary>
        /// 返回牌面描述字符串。
        /// </summary>
        /// <returns>牌面描述字符串。</returns>
        public override string ToString()
        {
            return String.Format("the {0} of {1}", this.Point, this.SuitKind.ToString());
        }

        public string ToFileName()
        {
            return String.Format("{1}{0:D2}", this.Point, this.SuitKind.ToString());
        }
        #endregion
    }

    public enum CardSuit
    {
        [Description("无")]
        none = 0,

        [Description("方块")]
        DND = 1,

        [Description("梅花")]
        CLB = 2,

        [Description("红桃")]
        HRT = 3,

        [Description("黑桃")]
        SPD = 4
    }

}
