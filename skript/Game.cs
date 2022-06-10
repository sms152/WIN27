using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InterfaceGame;
using GambleRedBlack;
using UPC_Work;
using GambleDrowPoker;

namespace nmWins27
{


    public class Game : MonoBehaviour
    {

        [SerializeField]
        ReelCntrl m_ReelCntrl;

        [SerializeField]
        NewGame2.SymbolList m_SymbolList;

        [SerializeField]
        AnimSymbols m_AnimSymbols;

        [SerializeField]
        ManagerLanguage m_ManagerLanguage;

        [SerializeField]
        NovoInterface m_Interface;

        [SerializeField]
        SoundManager m_SoundManager;
        
        [SerializeField]
        Paytable m_Paytable;

        [SerializeField]
        GambleDP m_Gamble;

        [SerializeField]
        Scratch m_Scratc;

        [SerializeField]
        bonus m_bonus;

        GLanguage m_Language;
        Data m_Data;
        MenuButton m_menuButton;
        bool m_Autoplay;

        DataGambleDP m_DataGambleDP;
        CL_TMoney TMoney;

        VoidDelegate m_Update, l_Update;

        KeyCode[] keys = global::Data.Keys.key;

        public event VoidDelegate
            GetRoll,
            GetGambleDP,
            CreditUpdate,
            WaitingInput,
            WaitingInputGamle;

        // Use this for initialization
        private void Awake()
        {
                enabled = false;
        }
        void OnDestroy()
        {
            Data.Dispose();

            m_SymbolList.Dispose();
        }
        void Start()
        {
           // Initialized();
        }
        public void Initialize()
        {
            int i;
            m_Data = Data.Instance;
            m_menuButton = MenuButton.Instance;
            m_DataGambleDP = DataGambleDP.Instacne;
            m_SoundManager.Initialize();

            m_SymbolList.Initialize();
            m_ReelCntrl.Initialize();
            m_ReelCntrl.End += End_Reel;// ExitToSlot;
            m_ManagerLanguage.Initialize();

            m_Paytable.Initialize();

            m_Paytable.Disabled += End_PayTable;
            m_Paytable.Enabled += Paytable_Enabled;
            m_Paytable.EmptyPage += Paytable_EmptyPage;

            m_Gamble.Initialize();
            m_AnimSymbols.Initialize();

            m_Gamble.Click += Click_GambleColor;
            m_Gamble.Quit += Gamble_Disabled;
     
            m_Gamble.Ready += Gamble_Ready;
            m_Gamble.EndAnim += Gamble_ShowTicketWin;

            m_Data.Bet = 4;
            m_Data.Lines = 1;
          
            m_Language = m_ManagerLanguage.m_GLanguage;
            TMoney = new CL_TMoney();

            m_bonus.Initialize();

            m_Interface.BBetMinus.OnClick(RemoveBet);
            m_Interface.BBetPlus.OnClick(AddBet);
            m_Interface.BLineMinus.OnClick(RemoveLine);
            m_Interface.BLinePlus.OnClick(AddLine);

            // DefInit
            m_Interface.BAutoplay.Text = m_Language.Autoplay;
            m_Interface.BStart.Text = m_Language.Start;
            m_Interface.BGamble.Text = m_Language.Gamble;
            m_Interface.BPaytable.Text = m_Language.Paytable;

            m_Interface.BStart.OnClick(NextRoll);
            m_Interface.BAutoplay.OnClick(Click_AutoPlay);
            m_Interface.BPaytable.OnClick(Click_PayTable);
            //### m_Interface.BScretch.OnClick(Click_Sratch);
            SetUpdate(Update_Ready);
        }
        //---------------------
        public void SetReelData()
        {
            m_ReelCntrl.SetSymbols();

            SetData();
        }

        public void SetGambleData()
        {
            m_Gamble.SetData();
        }

        public void SetData()
        {

            m_Interface.vBet.text = m_Data.Bet.ToString();
            m_Interface.vLine.text = "27";// m_Data.Lines.ToString();

            TotalBet();
        }

        public void SetUpdateCredit()
        {
 
            var param = _dllGame.GetParams;
            m_Data.credit = (int)(param.Credit );


            m_Interface.vBet.text = m_Data.Bet.ToString();
                m_Interface.vLine.text = "27";

            m_Interface.vCredit.text = m_Data.credit.ToString();

         
        }
        public void PlayGamble()
        {


            if (m_DataGambleDP.isWin)
                m_Interface.vDialog.text = m_DataGambleDP.countGame + 1 + m_Language.Of + m_DataGambleDP.totalGame.ToString();
            else
                m_Interface.vDialog.text = "";
            SetUpdate(Update_Gamble);
            m_Gamble.Play();

        }

        public void PlayReadyGamble()
        {
           
            m_Interface.BAutoplay.SetEnabled(true);
            m_Interface.BPaytable.SetEnabled(true);
            m_Interface.BStart.SetEnabled(true);

            m_Interface.BStart.OnClick(Click_CollectGamble);

            m_Gamble.PlayReady();
            SetUpdate(Update_Gamble);
           
        }
        public void PlayReady()
        {
            SetUpdateCredit();

            m_Interface.vDialog.text = m_Language.PleaseYourBet;
            m_Interface.vCredit.text = m_Data.credit.ToString();

            m_Interface.BPaytable.SetEnabled(true);
            m_Interface.BAutoplay.SetEnabled(true);
          

            m_Interface.BStart.SetEnabled(true);

            m_Interface.BLineMinus.SetEnabled(true);
            m_Interface.BLinePlus.SetEnabled(true);
            m_Interface.BBetMinus.SetEnabled(true);
            m_Interface.BBetPlus.SetEnabled(true);

            m_menuButton.ctrlLampStart(1);
            m_menuButton.ctrlLampLine(1);
            m_menuButton.ctrlLampBet(1);
            m_menuButton.ctrlLampAuto(1);
            m_menuButton.ctrlLampInfo(1);
            m_menuButton.ctrlLampMenu(1);
            m_menuButton.ctrlLampDouble(0);

            m_Interface.BStart.Text = m_Language.Start;
            m_Interface.BGamble.Text = m_Language.Gamble;
            m_Interface.BPaytable.Text = m_Language.Paytable;
            //### m_Interface.BScretch.Text = m_Language.Scratch;

            m_Interface.BStart.OnClick(NextRoll);
            m_Interface.BAutoplay.OnClick(Click_AutoPlay);
            m_Interface.BPaytable.OnClick(Click_PayTable);
            //### m_Interface.BScretch.OnClick(Click_Sratch);

            SetUpdate(Update_Ready);
        }
        void NextDouble()
        {
            //m_Gamble.DisableButtons();

            m_Interface.BStart.SetEnabled(false);
            m_Interface.BPaytable.SetEnabled(false);
            //m_Interface.BGamble.SetActive(false);
            m_Interface.BBetPlus.SetEnabled(false);
            m_Interface.BBetMinus.SetEnabled(false);
            m_Interface.BLineMinus.SetEnabled(false);
            m_Interface.BLinePlus.SetEnabled(false);
            m_Interface.BAutoplay.SetEnabled(false);
            //### m_Interface.BScretch.SetEnabled(false);

            SetLastUpdate(GetGambleDP);
        }
        void NextRoll()
        {
            if (TotalBet() > m_Data.credit + ((m_Data.bonus_ex_type == 5) ? m_Data.bonus_cred : 0))
            {
                m_Interface.vDialog.text = "Few Credit!";

                if (m_Autoplay) // STOP AUTO PLAY LOW CREDIT
                Click_AutoPlay();

                return;
            }
            else
            {
                m_Data.FindNextBet(0,1);//test bet * denom % 2 == 0
            }

            m_Interface.BStart.SetEnabled(false);
            m_Interface.BPaytable.SetEnabled(false);
            //m_Interface.BGamble.SetActive(false);
            m_Interface.BBetPlus.SetEnabled(false);
            m_Interface.BBetMinus.SetEnabled(false);
            m_Interface.BLineMinus.SetEnabled(false);
            m_Interface.BLinePlus.SetEnabled(false);
            m_Interface.BAutoplay.SetEnabled(true);//#
            //### m_Interface.BScretch.SetEnabled(false);

            m_menuButton.ctrlLampLine(0);
            m_menuButton.ctrlLampBet(0);
            if (!m_Autoplay)
            {
                m_menuButton.ctrlLampAuto(0);
            }
            m_menuButton.ctrlLampInfo(0);
            m_menuButton.ctrlLampMenu(0);

            SetLastUpdate(GetRoll);
        }
        public void PlayReel()
        {

            //m_SourceView.Stop();
            //==============================DELETE===========================
            //byte[][] mas = new byte[5][]{
            //    new byte[]{ 0,0,0,0,0 },
            //    new byte[]{ 0,0,0,0,0 },
            //    new byte[]{ 0,0,0,0,0 },
            //    new byte[]{ 0,0,0,8,0 },
            //    new byte[]{ 0, 0, 0, 0, 0, 0 }
            //}; ;

            //m_Data.roll = mas;
            //============================================================================
            m_AnimSymbols.Stop();
          
            
            m_ReelCntrl.Run(m_Data.WaitMoln);

            

            Playing();
        }
        void Playing()
        {
            m_Interface.BStart.SetEnabled(true);
            m_Interface.BPaytable.SetEnabled(false);
            m_Interface.BGamble.SetEnabled(false);

            m_Interface.vPrice.text = string.Format("{0:g}", ((double)(m_Data.Lines * m_Data.Bet * m_Data.video_denom) / 100)) + m_Language._Сurrency;
    
            m_Interface.vTicked.text = m_Data.ticket;
            m_Interface.vProduction.text = "";
            m_Interface.vCredit.text = m_Data.credit.ToString();

            m_Interface.vDialog.text = m_Language.GoodLuck;

            m_Interface.BStart.OnClick(Click_StopReel);
            m_Interface.BAutoplay.OnClick(Click_AutoPlay);

            SetUpdate(Update_Playing);
        }
        void Click_StopReel()
        {
          
            m_Interface.BStart.SetEnabled(false);
            m_ReelCntrl.Stop();
        }
        public void ShowGamble()
        {
            Click_Gamble();
        }

        int TotalBet()
        {
            var r = m_Data.Bet * m_Data.Lines;
            m_Data.TotalBet = r;
            m_Interface.vTotalBet.text = r.ToString();

            return r;
        }
        void End_Reel()
        {

            if (m_Data.CountWin > 0)
            {
                if (m_Data.masIdol.first > 0)
                {
                    m_bonus.Show();
                    m_bonus.End += ViewWin;
                }
                else
                {
                    ViewWin();
                }
            }
            else
                Ready();
        }
        void End_SourceView()
        {
            if (m_Data.wonRoll == 0)
                Ready();
            else if (m_Data.wonRoll > 0)
                WaitCollect();
        }
        void Gamble_ShowTicketWin()
        {

        }
        void ViewWin()
        {
    
            m_Interface.BStart.SetEnabled(true);
            m_Interface.BPaytable.SetEnabled(false);
            m_Interface.BGamble.SetEnabled(false);
            m_Interface.BAutoplay.SetEnabled(true);//#
        
            m_Interface.BStart.OnClick(Click_ViewSkip);

            m_Interface.BGamble.SetSprite = m_Interface.gold;
            m_Interface.BStart.SetSprite = m_Interface.green;

          
            m_AnimSymbols.StartAnim(m_Data.CountWin);
            m_AnimSymbols.End += End_SourceView; //ExitToSlot; //###

            SetUpdate(Update_ViewWin);
        }
        void WaitCollect()
        {
            m_Interface.BStart.SetEnabled(true);
            m_Interface.BPaytable.SetEnabled(false);
     
            m_Interface.BGamble.SetEnabled(true);
            m_menuButton.ctrlLampDouble(1);

            m_Interface.BAutoplay.SetEnabled(true);

            m_Interface.BGamble.OnClick(Click_Gamble);
            m_Interface.BStart.OnClick(Click_Collect);
            m_Interface.BAutoplay.OnClick(Click_AutoPlay);

            m_Interface.BStart.Text = m_Language.Collect;

            if (m_Autoplay)
            {
                Click_Collect();
            }
            else
            {
                m_SoundManager.PlaySound(38, 0, true);

                m_Interface.vDialog.text = m_Language.DoubleOrTake +"\n"+ m_Language.Won + " " + m_Data.shWP.ToString();

                SetUpdate(Update_WaitCollect);
            }
        }
        void Click_Gamble()
        {
  
            m_AnimSymbols.Stop();
           

            m_Interface.vDialog.text = m_Language.Won + " " + m_Data.WP.ToString();

            m_Interface.BStart.SetEnabled(false);
            m_Interface.BPaytable.SetEnabled(false);
            m_Interface.BGamble.SetEnabled(false);
            m_Interface.BAutoplay.SetEnabled(false);
     
            m_Interface.BStart.OnClick(Click_CollectGamble);

            m_SoundManager.StopSound(0);

            m_Gamble.Show();

            SetUpdate(null);
        }

        void Click_CollectGamble()
        {
            if(!m_Gamble.IsInteractive)
                return;

            if (m_DataGambleDP.win == 0) return;
            m_Interface.BAutoplay.SetEnabled(false);
            m_Interface.BPaytable.SetEnabled(false);
            m_Interface.BStart.SetEnabled(false);

            m_Gamble.DisableButtons();

   
            m_Data.shCREDIT = m_Data.credit;
            m_Data.WP = m_DataGambleDP.win;
           
            TMoney.SetTMoney((uint)m_Data.credit, (uint)m_Data.WP, (uint)((1.286) / Time.smoothDeltaTime));
          
            m_SoundManager.PlaySound(0,0);

            SetUpdate(null);

            SetUpdate(Update_Collecting);
        }
        void TicMoney()
        {
            if (TMoney.Test() > 0)
            {
                int par = TMoney.TakeMoney();
                if (par == 2)
                {
                    m_Data.shCREDIT = (int)TMoney.GetMoney(0);
                    m_Data.shWP = (int)TMoney.GetMoney(1);
                    m_Interface.vDialog.text = m_Data.shWP.ToString();
                    m_Interface.vCredit.text = m_Data.shCREDIT.ToString();
                }
                else
                if (par == 0)
                {
                    End_CollectingGamble();
                  
                    m_Interface.vDialog.text = m_Data.shWP.ToString();
                  
                }
            }
        }
        void Click_GambleColor()
        {
            NextDouble();
        }
        void Click_One()
        {
            m_DataGambleDP.ChoicedCard = 0;

            NextDouble();
        }

        void Click_Two()
        {
            m_DataGambleDP.ChoicedCard = 1;

            NextDouble();
        }
        void Click_Three()
        {
            m_DataGambleDP.ChoicedCard = 2;

            NextDouble();
        }

        void Click_Four()
        {
            m_DataGambleDP.ChoicedCard = 3;

            NextDouble();
        }
        void Gamble_Ready()
        {
            SetLastUpdate(WaitingInputGamle);
        }

        void Gamble_Disabled()
        {
            if (m_DataGambleDP.isWin)
            {
//###                m_SourceView.Pause(false);

            }
            else
            {
//###                m_SourceView.Stop();
                m_Data.isWinAny = false;
            }

            m_Interface.BAutoplay.SetSprite = m_Interface.green;
            m_Interface.BAutoplay.Text = m_Language.Autoplay;

            m_Interface.BPaytable.SetSprite = (m_Interface.blue);

            Ready();
        }
        void Click_Collect()
        {
            m_SoundManager.StopSound(0);
            m_SoundManager.PlaySound(1,0);
            
            m_Interface.BStart.SetEnabled(true);
            m_Interface.BPaytable.SetEnabled(false);
            m_Interface.BGamble.SetEnabled(false);
            m_Interface.BAutoplay.SetEnabled(true);

            Ready();
          
        }
        void Click_AutoPlay()
        {
            if (!m_Autoplay && TotalBet() > m_Data.credit)
                return;

            m_Autoplay = !m_Autoplay;

            m_Interface.BAutoplay.Text = m_Autoplay ? m_Language.Stop : m_Language.Autoplay;

            if (m_Autoplay)
                m_SoundManager.PlaySound(36,2);
            else
                m_SoundManager.PlaySound(37,2);

            m_Interface.BAutoplay.SetEnabled(true);
        }
        void Click_ViewSkip()
        {
    
            m_AnimSymbols.BreakAnim();
            m_SoundManager.StopSound(1); 
        }
        void Update_WaitCollect()
        {
            if (m_Autoplay || Input.GetKeyDown(keys[7]) || (keys[7] == KeyMenu))
                m_Interface.BStart.Click();
            else if (Input.GetKeyDown(keys[9]) || (keys[9] == KeyMenu))
                m_Interface.BAutoplay.Click();
            else if (Input.GetKeyDown(keys[2]) || (keys[2] == KeyMenu))
                m_Interface.BGamble.Click();
        }
        void Update_ViewWin()
        {
            if (Input.GetKeyDown(keys[7]) || (keys[7] == KeyMenu))
                m_Interface.BStart.Click();
            else if ((Input.GetKeyDown(keys[9])) || (Input.GetKeyDown(keys[6])) || (keys[9] == KeyMenu))
                m_Interface.BAutoplay.Click();
        }
        void Ready()
        {
            SetLastUpdate(WaitingInput);
        }
     
        void ClickStopRotate()
        {
            m_ReelCntrl.Stop();
            SetUpdate(Update_TakeOrRisk);
        }
       void ClickStartTake()
        {
            SetUpdate(Update_Ready);
        }
        void End_CollectingGamble()
        {
            m_Gamble.Hide();

            m_Interface.BStart.SetEnabled(false);
        }
        void Click_PayTable()
        {
            m_AnimSymbols.Stop();
          

            m_SoundManager.PlaySound(7,0);

            m_Paytable.Show();

   
            m_Interface.BStart.SetEnabled(false);
            m_Interface.BPaytable.SetEnabled(false);
            m_Interface.BGamble.SetEnabled(false);
            m_Interface.BAutoplay.SetEnabled(false);
         

            m_Interface.BBetPlus.SetEnabled(false);
            m_Interface.BBetMinus.SetEnabled(false);
            m_Interface.BLineMinus.SetEnabled(false);
            m_Interface.BLinePlus.SetEnabled(false);

            m_Interface.BPaytable.OnClick(Click_NextPaytable);
            m_Interface.BGamble.OnClick(Click_NextPaytable);

         
            m_Interface.BGamble.Text = m_Language.Next;

            SetUpdate(null);
        }
        public void Click_HidePaytable()
        {

            m_Paytable.Hide();

            m_Interface.BStart.SetEnabled(false);
            m_Interface.BPaytable.SetEnabled(false);
            m_Interface.BGamble.SetEnabled(false);
            m_Interface.BAutoplay.SetEnabled(false);

            m_Interface.BBetPlus.SetEnabled(false);
            m_Interface.BBetMinus.SetEnabled(false);
            m_Interface.BLineMinus.SetEnabled(false);
            m_Interface.BLinePlus.SetEnabled(false);

            m_Interface.BPaytable.Text = m_Language.Paytable;

            m_Interface.BBetMinus.OnClick(RemoveBet);
            m_Interface.BBetPlus.OnClick(AddBet);
            m_Interface.BLineMinus.OnClick(RemoveLine);
            m_Interface.BLinePlus.OnClick(AddLine);

            SetUpdate(null);
        }

        void Click_NextPaytable()
        {
            m_Paytable.Next();
        }

        void Paytable_Enabled()
        {
     
            m_Interface.BPaytable.SetEnabled(true);
            m_Interface.BGamble.SetEnabled(true);
      

            m_Interface.BBetPlus.SetEnabled(true);
            m_Interface.BBetMinus.SetEnabled(true);
            m_Interface.BLineMinus.SetEnabled(true);
            m_Interface.BLinePlus.SetEnabled(true);

  
            SetUpdate(Update_Paytable);
        }

        void Paytable_EmptyPage()
        {
            Click_HidePaytable();
        }

        void Update_Paytable()
        {
            if (Input.GetKeyDown(keys[0]) || (keys[0] == KeyMenu))
            {
                m_Interface.BLinePlus.Click();
            }
            else if (Input.GetKeyDown(keys[1]) || (keys[1] == KeyMenu))
            {
                m_Interface.BBetPlus.Click();
            }
            else if (Input.GetKeyDown(keys[8]) || Input.GetKeyDown(keys[2]) || (keys[8] == KeyMenu))
                m_Interface.BPaytable.Click();

        }

        void End_PayTable()
        {
            Ready();
        }
        void Update_Collecting()
        {
            TicMoney();
          
        }

        void AddLine()
        {
          

        }
        void RemoveLine()
        {
          
        }
        void RemoveBet()
        {
            m_Data.FindNextBet(1);
            m_Interface.vBet.text = m_Data.Bet.ToString();
            TotalBet();
            m_SoundManager.PlaySound(8, 0);
        }
        void AddBet()
        {
            m_Data.FindNextBet(0);
            m_Interface.vBet.text = m_Data.Bet.ToString();
            TotalBet();
            m_SoundManager.PlaySound(8, 0);
        }
        // SCRATCH

        void Click_Sratch()
        {
            m_Scratc.Show();
        }

        void Update_Scratch()
        {

        }

        void End_Scratch()
        {
            Ready();
        }

        void Scratc_EndPlay()
        {
            SetUpdate(Update_Scratch);
        }
        //--------------------------
        void Update_Gamble()
        {
            if (Input.GetKeyDown(keys[3]) || (keys[3] == KeyMenu))
                m_Interface.BDPOne.onClick.Invoke();
            else if (Input.GetKeyDown(keys[4]) || (keys[4] == KeyMenu))
                m_Interface.BDPTwo.onClick.Invoke();
            else if (Input.GetKeyDown(keys[5]) || (keys[5] == KeyMenu))
                m_Interface.BDPThree.onClick.Invoke();
            else if (Input.GetKeyDown(keys[6]) || (keys[6] == KeyMenu))
                m_Interface.BDPFour.onClick.Invoke();
            else if (Input.GetKeyDown(keys[7]) || (keys[7] == KeyMenu))
                m_Interface.BStart.Click();
        }

        void Update_Playing()
        {
            if (Input.GetKeyDown(keys[7]) || (keys[7] == KeyMenu))
                m_Interface.BStart.Click();
            else
            if (Input.GetKeyDown(keys[9]) || Input.GetKeyDown(keys[6]) || (keys[9] == KeyMenu))
                m_Interface.BAutoplay.Click();
        }
            //--------------------------
            void Update_Ready()
        {
            CreditUpdate();
            m_Interface.vCredit.text = m_Data.credit.ToString();
            UpdateJackpot();
            if (m_Autoplay || Input.GetKeyDown(keys[7]) || (keys[7] == KeyMenu))
                m_Interface.BStart.Click();
            else if (Input.GetKeyDown(keys[9]) || Input.GetKeyDown(keys[6]) || (keys[9] == KeyMenu))
                m_Interface.BAutoplay.Click();
            else if (Input.GetKeyDown(keys[0]) || (keys[0] == KeyMenu))
                m_Interface.BLinePlus.Click();
            else if (Input.GetKeyDown(keys[1]) || (keys[1] == KeyMenu))
                m_Interface.BBetPlus.Click();
            else if (Input.GetKeyDown(keys[8]) || Input.GetKeyDown(keys[2]) || (keys[8] == KeyMenu))
                m_Interface.BPaytable.Click();
            else if (Input.GetKeyDown(keys[4]) || (keys[10] == KeyMenu))
                Other.ClosedInGame.Exit();
        }

        public void UpdateJackpot()
        {
            UPC_Work.TipeJackpot[] type;
            UPC_Work.ColorJackpot[] color;
            uint[] sum;

            UPC_Work._dllGame.GetJackPotStatistics(out type, out color, out sum);

            m_Interface.JPBronze.SetValue(type[0], sum[0]);
            m_Interface.JPBronze.gameObject.SetActive(type[0] != UPC_Work.TipeJackpot.None);
            m_Interface.JPSilver.SetValue(type[1], sum[1]);
            m_Interface.JPSilver.gameObject.SetActive(type[1] != UPC_Work.TipeJackpot.None);
            m_Interface.JPGold.SetValue(type[2], sum[2]);
            m_Interface.JPGold.gameObject.SetActive(type[2] != UPC_Work.TipeJackpot.None);
        }

        void Update_StopRotate()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                ClickStopRotate();
        }
        void Update_TakeOrRisk()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                ClickStartTake();
        }

        void SetLastUpdate(VoidDelegate method)
        {
            l_Update = method;

            enabled = method != null;

            if (enabled)
                SetUpdate(LastUpdate);
        }

        void LastUpdate()
        {
            SetUpdate(null);

            if (l_Update != null)
                l_Update();
        }
        void SetUpdate(VoidDelegate method)
        {
            m_Update = method;

            enabled = method != null;
        }

        KeyCode KeyMenu = 0;
        void Update()
        {
            if (m_Update != null)
            {
                KeyMenu = m_menuButton.KbHit();
                m_Update();
                KeyMenu = 0;
            }
            else
                enabled = false;
        }
    }
}