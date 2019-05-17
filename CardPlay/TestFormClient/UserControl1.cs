using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PokerCardPlay.Core;
using System.Threading;

namespace TestFormClient
{
    public partial class UserControl1 : UserControl
    {

        //public Button CofirmBtn { get { return this.button1; } }

        //public string TextInput { get { return this.textBox1.Text; } }

        public long PlayerJettons { get { return Convert.ToInt64(this.label2.Text); } set { this.label2.Text = value.ToString(); } }

        public string PlayerName { get { return this.label1.Text; } set { this.label1.Text = value; } }

        public int BetJettons
        {
            get {
                BetStyle style = ((ComBetItem)this.comboBox1.SelectedItem).Value;
                return ResolvBetAction(style); ; 
            }
            set
            {
                this.betJettons = value;
                //this.lb_info.Text = String.Format("下注:{0}", value);
            }
        }

        public IPlayer Player
        {
            get { return _player; }
            set
            {
                if (value is IPokerBanker)
                {
                    this.label3.Text = "庄家";
                    this.label3.ForeColor = Color.Red;
                    //PlayerJettons -= 400;
                }
                else
                {
                    this.label3.Text = "闲家";
                    this.label3.ForeColor = Color.Black;
                }
                _player = value;
            }
        }
        private IPlayer _player;
        private int betJettons;
        private Action<BetStyle,int> betCallBack;
        private long currentPot,bettedJettons;
        private int cardinal;

         private Pot totalPot;
	
        
        public UserControl1()
        {
            InitializeComponent();
            this.textBox1.Visible = false;
            this.button1.Enabled = false;
            this.comboBox1.Enabled = false;

            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

            PlayerJettons = 0;
            //this.comboBox1.Items.Clear();
            //this.comboBox1.Items.Add("活注");
            //this.comboBox1.SelectedIndex = 0;
            this.comboBox1.DisplayMember = "Text";
        }

        public void DrawPlayerHandCard()
        {
            var handcards = Player.GetCards();
            if (handcards != null && handcards.Length == 2)
            {
                string filename1 = handcards[0].ToFileName();
                string filename2 = handcards[1].ToFileName();

                this.pictureBox1.Load("img/" + filename1 + ".png");
                this.pictureBox2.Load("img/" + filename2 + ".png");
            }
            else
            {
                this.pictureBox2.Load("img/CardBackBlue.png");
                this.pictureBox1.Load("img/CardBackBlue.png");
            }
        }

        public void SetBetCallBack(Action<BetStyle,int> callBack)
        {
            betCallBack = callBack;
        }

        public void ClearCallBack() { this.betCallBack = null; }

        public void StartBet(long currentPot, Pot pot, long betted,int cardinal)
        {
            this.currentPot = currentPot;
            this.totalPot = pot;
            this.cardinal = cardinal;
            this.bettedJettons = betted;

            this.button1.Enabled = true;
            this.comboBox1.Enabled = true;
            this.comboBox1.SelectedIndex = 0;
            //this.lb_info.Text = "";
        }
        public void EndBet()
        {
            this.button1.Enabled = false;
            this.comboBox1.Enabled = false;
            //this.lb_info.Text = "";
        }

        public void AddComSelection(BetStyle betStyle)
        {
            string text = betStyle.GetDescription();
            this.comboBox1.Items.Add(new ComBetItem { Text = text, Value = betStyle });
        }

        public void ClearComSelection()
        {
            this.comboBox1.Items.Clear();

        }


        public void PlayerWin(long jettons)
        {
            Player.GainJettons(Convert.ToInt32(jettons));
            this.PlayerJettons += jettons;
            //ShowInfo(string.Format("+{0}", betJettons));
            //this.lb_info.Text = "+" + betJettons.ToString();
            betJettons = 0;
        }

        public void PlayerLose()
        {
            Player.LostJettons(BetJettons);
            this.PlayerJettons -= BetJettons;
            //this.lb_info.Text = "-" + betJettons.ToString();
            ShowInfo(string.Format("-{0}", BetJettons));
            betJettons = 0;
        }

        public void PlayerLose(long jettons)
        {


            Player.LostJettons((int)jettons);
            this.PlayerJettons -= jettons;
            //this.lb_info.Text = "-" + betJettons.ToString();
            ShowInfo(string.Format("-{0}", jettons));
            betJettons = 0;
        }

        public void RefreshScore()
        {
            PlayerJettons = Player.GetJettonsCount();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (betCallBack != null)
            {
                BetStyle style = ((ComBetItem)this.comboBox1.SelectedItem).Value;

                BetJettons = (style == BetStyle.HalfOfTotalPot || style == BetStyle.AllOfTotalPot) ? 0 : ResolvBetAction(style);
                //ShowInfo(String.Format("下注:{0}", BetJettons));
                betCallBack(style, BetJettons);
            }
        }

        public void ShowInfo(string text)
        {
            new Thread(new ParameterizedThreadStart(obj =>
            {
                string str = obj.ToString();

                this.Invoke(new Action(() =>
                {
                    this.lb_info.Text = str;
                    this.Update();
                }));

                Thread.Sleep(3000);
                this.Invoke(new Action(() =>
                {
                    this.lb_info.Text = "";
                    this.Update();
                }));
            })).Start(text);
        }


        private int ResolvBetAction(BetStyle style)
        {
            int result = 0;
            switch (style)
            {
                case BetStyle.HalfOfCurrentPot:
                    //result = Convert.ToInt32(currentPot / 2);
                    result = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(currentPot) / 200) * 100);
                    break;
                case BetStyle.HalfOfTotalPot:
                    result = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalPot.PotJettons) / 200) * 100);
                    break;
                case BetStyle.LeaveOfCurrentPot:
                    result = Convert.ToInt32(currentPot - bettedJettons);
                    break;
                //case BetStyle.LeaveOfTotalPot:
                //    result = Convert.ToInt32(totalPot - currentPot / 2);
                //    break;
                case BetStyle.AllOfCurrentPot:
                    result = Convert.ToInt32(currentPot);
                    break;
                case BetStyle.AllOfTotalPot:
                        result = Convert.ToInt32(totalPot.PotJettons);
                    break;
                case BetStyle.Cardinal:
                    result = cardinal;
                    break;
                case BetStyle.Custom:
                    result = Convert.ToInt32(this.textBox1.Text);
                    break;
                default:
                    result = 100;
                    break;
            }
            return result;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComBetItem)this.comboBox1.SelectedItem).Value.Equals(BetStyle.Custom))
                this.textBox1.Visible = true;
            else
                this.textBox1.Visible = false;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char result = e.KeyChar;
            if (string.IsNullOrWhiteSpace(textBox1.Text) && result == 48)
                e.Handled = true;
            else
            {
                if (char.IsDigit(result) || result == 8)
                {
                    e.Handled = false;
                    if (!textBox1.Text.EndsWith("00"))
                        textBox1.Text += "00";
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        private class ComBetItem
        {
            public string Text { get; set; }
            public BetStyle Value { get; set; }
        }
    }
}
