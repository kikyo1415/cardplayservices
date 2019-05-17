using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PokerCardPlay.Core
{
    internal interface ICardTable
    {
        PokerCard DealCard();

        void Shuffle();
    }


    internal class PokerCardTable : ICardTable
    {
        public List<PokerCard> Cards
        {
            get
            {
                return cards;
            }
        }
        private List<PokerCard> cards;
        public CardScript scriptObj;

        public PokerCardTable()
        {
            cards = new List<PokerCard>();
            InitCards();
            InitScript();
        }

        private void InitCards()
        {
            for (int i = 1; i < 5; i++)
            {
                CardSuit cs = (CardSuit)Enum.Parse(typeof(CardSuit), i.ToString());
                for (int j = 1; j < 11; j++)
                {
                    cards.Add(new PokerCard(cs, j));
                }
            }
        }

        private void InitScript()
        {
            //scriptObj = new CardScript();
            //XmlDocument doc = new XmlDocument();
            //doc.Load("CardCharge.d0");
            //scriptObj = XmlSerilzerTool.Deserialize<CardScript>(doc.OuterXml);

            //scriptObj.PlayerCardScripts.Add(scriptObj.PlayerCardScripts[3]);
        }

        public PokerCard DealCard()
        {
            return DealCardRandom();
        }

        private PokerCard DealCardRandom()
        {
            Random rd = new Random();
            int index = rd.Next(0, cards.Count);
            var card = cards[index];
            card.IsHandle = true;
            cards.Remove(card);
            return card;
        }

        public PokerCard[] DealCardWithScript(int point)
        {
            Random rd = new Random();
            PokerCard firstCard = null;
            PokerCard secCard = null;
            if (point == 0)
            {
                int a = 0;
                while (true)
                {
                    firstCard = Cards[a];
                    if (Cards.Any(d => d.Point == 10 - point))
                    {
                        secCard = Cards.FirstOrDefault(d => d.Point == 10 - firstCard.Point);
                        Cards.Remove(firstCard);
                        Cards.Remove(secCard);
                        break;
                    }
                    else
                        a++;
                    if (a > Cards.Count)
                    {
                        firstCard = DealCardRandom();
                        secCard = DealCardRandom();
                    }
                }
            }
            else if (point < 10)
            {
                int a = 0;
                while (true)
                {
                    firstCard = Cards[a];
                    int secPoint = point < firstCard.Point ? point + 10 - firstCard.Point : point - firstCard.Point;
                    if (Cards.Any(d => d.Point == secPoint) && (point % 2 != 0 || firstCard.Point != point / 2))
                    {
                        secCard = Cards.FirstOrDefault(d => d.Point == secPoint);
                        Cards.Remove(firstCard);
                        Cards.Remove(secCard);
                        break;
                    }
                    else
                        a++;

                    if (a > Cards.Count)
                    {
                        firstCard = DealCardRandom();
                        secCard = DealCardRandom();
                    }
                }
            }
            else
            {
                int cardPoint = point - 10;
                if (Cards.Count(d => d.Point == cardPoint) >= 2)
                {
                    var currentCards = Cards.Where(d => d.Point == cardPoint).ToList();
                    firstCard = currentCards[0];
                    secCard = currentCards[1];
                    Cards.RemoveAll(d => d.ToFileName().Equals(firstCard.ToFileName()) || d.ToFileName().Equals(secCard.ToFileName()));
                }
                else
                {
                    firstCard = DealCardRandom();
                    secCard = DealCardRandom();
                }
            }

            //cards.Remove(cards.FirstOrDefault(d => d.SuitKind.Equals(firstCard.SuitKind) && d.Point.Equals(firstCard.Point)));
            //cards.Remove(cards.FirstOrDefault(d => d.SuitKind.Equals(secCard.SuitKind) && d.Point.Equals(secCard.Point)));
            return new PokerCard[2] { firstCard, secCard };
        }

        public void Shuffle()
        {
            //Random random = new Random();
            //int tempIndex = 0;
            //PokerCard temp = null;
            //for (int i = 0; i < 40; i++)
            //{
            //    tempIndex = random.Next(40);
            //    temp = Cards[tempIndex];
            //    Cards[tempIndex] = Cards[i];
            //    Cards[i] = temp;
            //}
            cards = cards.OrderBy(d => Guid.NewGuid()).ToList();
        }

        private int getNum(int[] arrNum, int tmp, int minValue, int maxValue, Random ra)
        {
            int n = 0;
            while (n <= arrNum.Length - 1)
            {
                if (arrNum[n] == tmp) //利用循环判断是否有重复 
                {
                    tmp = ra.Next(minValue, maxValue); //重新随机获取。 
                    getNum(arrNum, tmp, minValue, maxValue, ra);//递归:如果取出来的数字和已取得的数字有重复就重新随机获取。 
                }
                arrNum[n] = tmp;
                n++;
            }
            return tmp;
        }
    }




}
