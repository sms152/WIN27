using GambleDrowPoker;
using GambleRedBlack;
using InterfaceGame;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Data;

using UPC_Work;

namespace nmWins27
{
    public class GameConsturct : MonoBehaviour
    {
        [SerializeField]
        Game m_Game;

        [SerializeField] NovoInterface m_Interface;

        [SerializeField] ManagerLanguage m_ManagerLanguage;

        VoidDelegate _update;

        GLanguage m_Language;

        Data m_Data;

        DataGambleDP m_DataGambleDP;

        Calc m_Calc;

        Queue<SwapRoll> lwins = new Queue<SwapRoll>(50);

        byte[][] roll;

        uint number;

        bool postInit, postInitGamble, isReconnect;

        Ini _ini = Ini.Instance;

        void Start()
        {
            enabled = false;

            Initialize();
        }

        void Initialize()
        {
            roll = DefRoll();

            m_Calc = new Calc();
            m_Calc.Initialize();

            m_Data = Data.Instance;

            m_Game.GetRoll += GetRoll;

            m_Game.GetGambleDP += GetGambleDP;

            m_Game.WaitingInput += Game_WaitingInput;
            m_Game.WaitingInputGamle += Game_WaitingInputGamle;

            m_Game.CreditUpdate += CreditUpdate;

            m_Game.Initialize();



            m_Language = m_ManagerLanguage.m_GLanguage;

        }

        void CreditUpdate()
        {
            var v = _dllGame.GetParams;
            m_Data.credit = (int)(v.Credit );
            m_Data.bonus_cred = v.Bonus_Cred;
            m_Data.bonus_ex_type = v.Bonus_Ext_Type;

            BonusBar.Instance.SetTotalBet(m_Data.TotalBet);
        }


        unsafe void InitServerGamble(SwapInit swapInit)
        {
            m_DataGambleDP = DataGambleDP.Instacne;

            var point = _dllGame.GetSwap();
            if (swapInit.Operation == Operation.DrowPoker)
                m_DataGambleDP.nextDiller = point[SwapInit.SIZE + SwapDP.SIZE];
            else
                m_DataGambleDP.nextDiller = point[SwapInit.SIZE + swapInit.Count * SwapDP.SIZE];
            m_DataGambleDP.diller = 0;

            m_DataGambleDP.countGame = 0;
            m_DataGambleDP.isLast = false;
            m_DataGambleDP.totalGame = 0;
            m_DataGambleDP.toWin = 0;
            m_DataGambleDP.win = 0;
            m_DataGambleDP.isWin = false;
        }

        unsafe public struct SwapInit
        {
            public readonly int Version, SubVersion; // 4 + 4 = 8
            public readonly Operation Operation; // 4
            public readonly int Denom; // 4 
            public readonly int Lines, Bet;// 4 + 4 = 8
            public readonly int Price, Win;// 4 + 4 = 8
            public readonly int Count;// 4 
            public readonly byte[][] Rolls;//[25];
            public readonly byte[] Double;//[5]; 41

            public SwapInit(byte* swap)
            {
                int* iSwap = (int*)swap;

                Version = iSwap[0];
                SubVersion = iSwap[1];
                Operation = (Operation)iSwap[2];
                Denom = iSwap[3];
                Lines = iSwap[4];
                Bet = iSwap[5];
                Price = iSwap[6];
                Win = iSwap[7];
                Count = iSwap[8];

                Rolls = new byte[5][];

                Double = new byte[5];

                for (int c = 0, column = (9 * 4); c < 5; c++, column = (c * 5) + (9 * 4))
                {
                    Rolls[c] = new byte[5];

                    for (int r = 0; r < 5; r++)
                    {
                        Rolls[c][r] = swap[(column + r)];
                    }

               //     Rolls[c][5] = swap[column];
                }

                for (int i = (9 * 4) + 25, c = 0; i < 5 + (9 * 4) + 25; i++, c++)
                {
                    Double[c] = swap[i];
                }
            }

            public const int SIZE = 66;
        }

        unsafe public struct SwapRoll
        {
            public readonly int Win;
            public readonly byte[][] Rolls;//[25];

            public SwapRoll(byte* swap)
            {
                int* iSwap = (int*)swap;

                Win = iSwap[0];

                Rolls = new byte[5][];

                for (int c = 0, column = 4; c < 5; c++, column = (c * 5) + 4)
                {
                    Rolls[c] = new byte[5];

                    for (int r = 0; r < 5; r++)
                    {
                        Rolls[c][r] = swap[column + r];
                    }

                    //Rolls[c][5] = swap[column];
                }
            }

            public const int SIZE = 29;
        }

        unsafe public struct SwapDouble  // игра  - риск
        {
            public readonly int Win;
            public readonly byte Card;

            public SwapDouble(byte* swap)
            {
                Win = swap[3];

                Win <<= 8;
                Win |= swap[2];
                Win <<= 8;
                Win |= swap[1];
                Win <<= 8;
                Win |= swap[0];

                Card = swap[4];
            }
        }

        public byte[][] DefRoll()
        {
            return new byte[][]
            {
                new byte[]{ 1,1,1,1,1 },
                new byte[]{ 1,1,1,1,1 },
                new byte[]{ 1,1,1,1,1 },
                new byte[]{ 1,1,1,1,1 },
                new byte[]{ 1,1,1,1,1 }
            };
        }

        //public byte[][] DefBorder()
        //{
        //    return new byte[][]
        //    {
        //        new byte[]{ 0,0,0,0 },
        //        new byte[]{ 0,0,0,0 },
        //        new byte[]{ 0,0,0,0 },
        //        new byte[]{ 0,0,0,0 },
        //        new byte[]{ 0,0,0,0 },
        //    };
        //}

        byte[][] DefaultRoll()
        {
            byte[][] Rolls = new byte[][]
                {
                new byte[] { 0, 0, 0, 0, 0 },
                new byte[] { 0, 0, 0, 0, 0 },
                new byte[] { 0, 0, 0, 0, 0 },
                new byte[] { 0, 0, 0, 0, 0 },
                new byte[] { 0, 0, 0, 0, 0 },
                };

            return Rolls;
        }

        public unsafe void InitServer()
        {
            var param = _dllGame.GetParams;

            var point = _dllGame.GetSwap();

            SwapInit swapInit = new SwapInit(point);

            //if ((param.GameStat == GameStat.InGame) && (swapInit.Operation == Operation.Rolling) && (swapInit.Denom != param.Denom))
            //{
            //    _dllGame.GameStop();

            //    param = _dllGame.GetParams;
            //    point = _dllGame.GetSwap();

            //    swapInit = new SwapInit(point);
            //}
            // START SCREEN SYMBOLS
            if (swapInit.Operation == Operation.Rolling && param.GameStat == UPC_Work.GameStat.GameOver)
            {
                point = &point[SwapInit.SIZE + ((swapInit.Count - 1) * SwapRoll.SIZE)];

                var swapRoll = new SwapRoll(point);

                m_Data.roll = swapRoll.Rolls;
            }
            else if (param.GameStat == UPC_Work.GameStat.InGame && swapInit.Operation == Operation.Rolling)
            {
                m_Data.roll = swapInit.Rolls;
            }
            else
                m_Data.roll = DefaultRoll();

            m_Data.credit = (int)(param.Credit == 0 || swapInit.Denom == 0 ? 0 : param.Credit);// / param.Denom);

            
            //if (param.GameStat == GameStat.InGame && swapInit.Operation == Operation.Rolling)
            //{
            //    m_Data.credit += (swapInit.Lines * swapInit.Bet);
            //}

            m_Data.Bet = swapInit.Bet;
            m_Data.Lines = swapInit.Lines;
            m_Data.inBet = m_Data.findIndex(m_Data.Bet);
   
            m_Data.video_denom = (float)param.Denom / ((float)param.DenomDenominator / (float)100);
            m_Data.FindNextBet(0, 1);

            if (m_Data.video_denom == 0.5f)
            {
                m_Data.denom = m_Language._Denom + "0.010" + _ini.Currency;
            }
            else
                m_Data.denom = m_Language._Denom + Math.Round(m_Data.video_denom / 100f, 5) + _ini.Currency;
            m_Interface.vDenom.text = m_Data.denom;
            m_Interface.vCredit.text = m_Data.credit.ToString();
            m_Interface.vTicked.text = _dllGame.SeriesString + "-" + param.Number;

            m_Interface.vBonus.text = (param.Bonus_Cred / 100u).ToString() + " ₴";
            m_Interface.vCard.text = param.LogInString;

            postInitGamble = postInit = true;

            if (param.GameStat != UPC_Work.GameStat.InGame)
                PostInit();

            InitServerGamble(swapInit);

            m_Game.SetReelData();

            SetUpdate(InitializePlay);
        }

        unsafe void PostInit()
        {
            postInit = false;

            var param = _dllGame.GetParams;

            var point = _dllGame.GetSwap();

            SwapInit swapInit = new SwapInit(point);

            m_Data.Bet = 100;// param.Bet_Min < 10 ? 4 : 10;
            m_Data.inBet = 0;// param.Bet_Min < 10 ? 0 : 2;

            m_Data.Lines = 1;

            m_Data.credit = (int)(param.Credit);// param.Denom);

            m_Interface.vCredit.text = m_Data.credit.ToString();

            SetMinMaxBet(param.Bet_Max, param.Bet_Min, param.Denom, swapInit.Bet);
            m_Data.video_denom = (float)param.Denom / ((float)param.DenomDenominator / (float)100);
            if (m_Data.video_denom == 0.5f)
            {
                m_Data.denom = m_Language._Denom + "0.010" + m_Language._Сurrency;
            }
            else
                m_Data.denom = m_Language._Denom + Math.Round(m_Data.video_denom / 100f, 5) + m_Language._Сurrency;
            m_Data.FindNextBet(0, 1);
            m_Interface.vDenom.text = m_Data.denom;

            m_Game.SetData();
        }

        unsafe void InitializePlay()
        {
            SetUpdate(null);

            var param = _dllGame.GetParams;
            var swap = _dllGame.GetSwap();
            var swapInit = new SwapInit(swap);

            if (param.GameStat == UPC_Work.GameStat.InGame)
            {
                if (swapInit.Operation == Operation.Rolling)
                {
                    WaitRoll();
                }
                else // Operation.Gamble
                {
                    if (swapInit.Win > 0)
                    {
                        m_Game.ShowGamble();
                    }
                    else
                    {
                        CollectCredit();
                    }
                }
            }
            else
            {
                m_Game.PlayReady();
                SetUpdate(UpdateReady);
            }
        }

        void UpdateReady()
        {
            var param = _dllGame.GetParams;
            m_Data.credit = (int)param.Credit;// / param.Denom;
            m_Interface.vBonus.text = (param.Bonus_Cred / 100u).ToString() + _ini.Currency;
        }

        unsafe public struct SwapDP  // игра  - риск
        {
            public byte dealer,            // карта дилера 
               Count,         // кол-во карт, доступных игроку
               Select;    // индекс выбранной игроком карты 
            public byte[] Player;    // карты игрока

            public SwapDP(byte* swap)
            {
                dealer = (byte)swap;
                Count = (byte)swap[1];
                Select = (byte)swap[2];
                Player = new byte[10];
                Player[0] = swap[3];
                Player[1] = swap[4];
                Player[2] = swap[5];
                Player[3] = swap[6];
                Player[4] = swap[7];
                Player[5] = swap[8];
                Player[6] = swap[9];
                Player[7] = swap[10];
                Player[8] = swap[11];
                Player[9] = swap[12];

            }
            public const int SIZE = 13;
        }

        unsafe void WaitDrowPoker()
        {
            var param = _dllGame.GetParams;

            if (param.Status != UPC_Work.DLLConnect.On)
            {
                isReconnect = true;
                if (param.Status != UPC_Work.DLLConnect.TryOn)
                    SetUpdate(null);

                return;
            }
            if (isReconnect)
            {
                isReconnect = false;

                GetGambleDP();
            }
            if (_dllGame.GetDone)
            {
                SetUpdate(null);

                enabled = false;

                if (number != param.Number)
                {
                    number = param.Number;

                    var swap = _dllGame.GetSwap();

                    var swapInit = new SwapInit(swap);

                    var swapD = new SwapDP(&swap[SwapInit.SIZE]);//+SwapRoll.SIZE*swapInit.Count]);

                    m_DataGambleDP.countGame = swapInit.Count;
                    m_DataGambleDP.totalGame = 5;
                    if (swapInit.Operation == Operation.DrowPoker)
                        m_DataGambleDP.nextDiller = swap[SwapInit.SIZE + SwapDP.SIZE];
                    else
                        m_DataGambleDP.nextDiller = swap[SwapInit.SIZE + swapInit.Count * SwapDP.SIZE];

                    m_DataGambleDP.isLast = swapInit.Count > 4;
                    m_DataGambleDP.cards = swapD.Player;
                    m_DataGambleDP.win = swapInit.Win;
                    m_DataGambleDP.isWin = swapInit.Win > 0;
                    m_DataGambleDP.toWin = swapInit.Win * 2;

                    m_Data.wonTicket = swapInit.Win;
                    m_Data.wonRoll = swapInit.Win;

                    if (m_Data.isWinAny)
                    {
                        m_Data.isWinAny = swapInit.Win > 0; 
                    }

                    m_Game.PlayGamble();
                }
            }
        }

        void WaitStop()
        {
            var param = _dllGame.GetParams;

            if (param.Status != UPC_Work.DLLConnect.On)
            {
                isReconnect = true;
                if (param.Status != UPC_Work.DLLConnect.TryOn)
                    SetUpdate(null);

                return;
            }
            if (isReconnect)
            {
                isReconnect = false;

                CollectCredit();
            }
            if (!_dllGame.GetDone)
                return;

            SetUpdate(null);

            if (_dllGame.IsJackPot)
            {
                Other.ClosedInGame.Exit();

                m_Interface.vDialog.text = ("************  Jack Pot ***********");
               // _dllGame.GameStop();
            }
            else
            {
                m_Game.PlayReady();
            }
        }

        unsafe void WaitRoll()
        {
            var param = _dllGame.GetParams;

            if (param.Status != UPC_Work.DLLConnect.On)
            {
                isReconnect = true;
                if (param.Status != UPC_Work.DLLConnect.TryOn)
                    SetUpdate(null);

                return;
            }
            if (isReconnect)
            {
                isReconnect = false;

                GetRoll();
            }
            if (!_dllGame.GetDone)
                return;

            SetUpdate(null);

            //var param = _dllGame.GetParams;

            if (!postInit)
                m_Data.credit = (int)(param.Credit);// / param.Denom);

            if (number != param.Number)
            {
                number = param.Number;

                var swap = _dllGame.GetSwap();

                var swapInit = new SwapInit(swap);
                var spoint = &swap[SwapInit.SIZE];
                m_DataGambleDP.nextDiller = spoint[SwapRoll.SIZE * swapInit.Count];

                if (swapInit.Count > 0)
                {
                    var point = &swap[SwapInit.SIZE];

                    m_Data.wonTicket = 0;
 
                    m_Data.ticket = _dllGame.SeriesString + "-" + param.Number;

                    var points = &swap[SwapInit.SIZE];
                    m_DataGambleDP.nextDiller = points[SwapRoll.SIZE * swapInit.Count];

                    for (int i = 0; i < swapInit.Count; i++, point += SwapRoll.SIZE)
                    {
                        var r = new SwapRoll(point);

                        lwins.Enqueue(r);

                        m_Data.wonTicket += r.Win;
                    }

                    NextRoll();
                }
            }
            else
            {
                m_Interface.vDialog.text = "Error #113 : No Ticket";
                m_Interface.vDialog.color = Color.red;

            }
        }

        void NextRoll()
        {
            var r = lwins.Dequeue();

            m_Data.previousWonFragTicket = m_Data.wonFragTicket;

            m_Data.wonFragTicket += r.Win;

            m_Data.roll = r.Rolls;
            //------------------------------------------- DELETE
            //byte[][] mas = new byte[5][]{
            //    new byte[]{ 0,0,10,3,0 },
            //    new byte[]{ 0,0,9,3,0 },
            //    new byte[]{ 0,0,10,3,0 },
            //    new byte[]{ 0,0,0,9,0 },
            //    new byte[]{ 0, 0, 0, 0, 0 }
            //}; ;
            //m_Data.roll = mas;
            //-----------------------------------------------
            m_Calc.MakeWinLine();

            if (r.Win != m_Data.wonRoll)
            {
                Roll();


                m_Interface.vDialog.text = "Error #001\nswp: " + r.Win + " - calc: " + m_Data.wonRoll;
                m_Interface.vDialog.color = Color.red;

                Debug.Log(m_Interface.vDialog.text);
                return;
            }

            m_Game.PlayReel();
        }

        void Roll()
        {
            var rol = m_Data.roll;

            StringBuilder str = new StringBuilder(50);

            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    str.Append(rol[c][r] + " ");
                }
                str.AppendLine();
            }

            Debug.Log(str);
        }

     
        void SetMinMaxBet(int max, int min, int denamination, int bet)
        {
            List<int> betList = new List<int>
            {
                //1,
                //2,
                //3,
                4,
                6,
                10,
                15,
                20,
                25,
                30,
                40,
                50,
                60,
                70,
                80,
                90,
                100
            };

            for (sbyte i = 0; i < betList.Count; ++i)
            {
                var b = betList[i] * denamination;

                if (b % 10 != 0 || b < min || b > max)
                    betList.Remove(betList[i--]);
            }

            m_Data.betValues = betList.ToArray();

            if (!betList.Contains(bet))
                m_Data.bet = betList[0];
        }


        unsafe private void Game_WaitingInputGamle()
        {
            var param = _dllGame.GetParams;
            var swap = _dllGame.GetSwap();
            var swapInit = new SwapInit(swap);

            m_DataGambleDP.win = swapInit.Win;
            m_DataGambleDP.toWin = swapInit.Win * 2;

            if (postInitGamble)
            {
                postInitGamble = false;

                if (param.GameStat == UPC_Work.GameStat.InGame)
                {
                    if (swapInit.Operation == Operation.Gamble)
                    {
                        SetUpdate(WaitDrowPoker);
                    }
                    else
                        m_Game.PlayReadyGamble();
                }
            }
            else
            {
                m_Game.PlayReadyGamble();
            }
        }

        void Game_WaitingInput()
        {
            if (lwins.Count == 0)
            {
                if (postInit)
                    PostInit();

                CollectCredit();

             }
            else
            {

                m_Game.PlayReady();
            }

        }

        private void CollectCredit()
        {
            _dllGame.GameStop();

            SetUpdate(WaitStop);
        }

        private void GetGambleDP()
        {
            _dllGame.RollDouble(10, 4, (byte)m_DataGambleDP.ChoicedCard);
            SetUpdate(WaitDrowPoker);
        }

        private void GetRoll()
        {
            if (lwins.Count > 0)
            {
                NextRoll();

                return;
            }
   
            _dllGame.Roll(m_Data.Bet, m_Data.Lines);

            SetUpdate(WaitRoll);
        }

        void SetUpdate(VoidDelegate method)
        {
            _update = method;

            enabled = method != null;
        }

        void Update()
        {
            if (_update != null)
                _update();
        }

    }


    public class DataGame
    {
        public bool isWinAny,
                    isFreegame,
                    isWinFreegame,
                    isEndFreegame;

        public int credit,
                   rollWinCredit,
                   product,
                   totalWinCredit,
                   line,
                   bet,
                   countWinFreegame;

        public string series,
                      ticket,
                      denom,
                      cash,
                      price,
                      bonus,
                      card,
                      //totalBet, 
                      currentFreegame,
                      countFreegame;

//###        public ESymbol[][] symbols;

        public int[] betValues;
    }

     public class LanguageInterface
    {
        public LanguageInterface(int index)
        {
            txt = new string[11];

            switch (index)
            {
                case 0:
                    {
                        txt[0] = "Баланс:";
                        txt[1] = "Бонус:";
                        txt[2] = "Квиток:";
                        txt[3] = "Ціна:";
                        txt[4] = "Видобуток:";
                        txt[5] = "Карта:";
                        txt[6] = "Лінії";
                        txt[7] = "Ставка/Лінії";
                        txt[8] = "Ставка";
                        txt[9] = "1 Кредит = ";
                        txt[10] = "Зачинити";
                        break;
                    }
                case 1:
                    {
                        txt[0] = "CREDIT";
                        txt[1] = "BONUS";
                        txt[2] = "TICKET";
                        txt[3] = "PRICE";
                        txt[4] = "Production:";
                        txt[5] = "Card:";
                        txt[6] = "LINES";
                        txt[7] = "TBET";
                        txt[8] = "BET";
                        txt[9] = "1 Credit = ";
                        txt[10] = "Close";
                        break;
                    }
                case 2:
                    {
                        txt[0] = "Баланс:";
                        txt[1] = "Бонус:";
                        txt[2] = "Билет:";
                        txt[3] = "Цена:";
                        txt[4] = "Выигрыш:";
                        txt[5] = "Карта:";
                        txt[6] = "Линий";
                        txt[7] = "Ставка/Линий";
                        txt[8] = "Ставка";
                        txt[9] = "1 Кредит = ";
                        txt[10] = "Закрыть";
                        break;
                    }
                default:

                    break;
            }
        }

        public readonly string[] txt;
    }

    public class LanguageGamble
    {
        public LanguageGamble(int index)
        {
            txt = new string[6];

            switch (index)
            {
                case 0:
                    {
                        txt[0] = "Виграна Сума";
                        txt[1] = "Грати на Виграш";
                        txt[2] = "Попередні карти";
                        txt[3] = "Червона";
                        txt[4] = "Чорна";
                        txt[5] = "Виберіть «Червоний» або «Чорний», щоб грати або забрати виграш!";
                        break;
                    }
                case 1:
                    {
                        txt[0] = "Gamble Amount";
                        txt[1] = "Gamble To Win";
                        txt[2] = "Previous Card";
                        txt[3] = "Red";
                        txt[4] = "Black";
                        txt[5] = "Choose Red or Black to gamble, or take the win!";
                        break;
                    }
                case 2:
                    {
                        txt[0] = "Выигранная сумма";
                        txt[1] = "Играть на выигрыш";
                        txt[2] = "Предыдущие карты";
                        txt[3] = "Красный";
                        txt[4] = "Черный";
                        txt[5] = "Выберите «Красный» или «Черный», чтобы сыграть, или забрать выигрыш!";
                        break;
                    }
                default:

                    break;
            }
        }

        public readonly string[] txt;
    }

    public class LanguagePaytable
    {
        public LanguagePaytable(int index)
        {
            txt = new string[4];

            switch (index)
            {
                case 0:
                    {
                        txt[0] = "Тільки на 2,3 та 4 барабані. Замінює всі символи, крім скаттера";
                        txt[1] = "Виграшна комбінація являє собою безперервну послідовність однакових символів. Всі комбінації рахуються зліва направо на сусідніх барабанах, на обраних лініях, починаючи з самого лівого барабана, за винятком скаттерів. Виграш Скаттера додається до виграша ліній. По кожній з ліній / комбінацій скаттерів виплачується тільки найбільший виграш. Виграші по лініях представляють множення ставки на Значення виграшної лінії. Виграші Скаттера множаться на загальну суму ставки. Несправність анулює всі виплати та ігри.";
                        txt[2] = "ЛІНІЇ ВИПЛАТ І ПРАВИЛА";
                        break;
                    }
                case 1:
                    {
                        txt[0] = "On 2nd, 3rd and 4th reels only. Substitutes all symbols except scatter";
                        txt[1] = "All pays are for combinations of a kind. All pays are left to right on adjacent reels, on selected lines, beginning with the leftmost reel, except scatters. Scatter wins are added to the payline wins. Highest payline and/or scatter wins only paid. Line wins are multiplied by the bet value on the winning line. Scatter wins are multiplied by the total bet value. Malfunction voids all pays and plays.";
                        txt[2] = "PAYLINES AND RULES";
                        break;
                    }
                case 2:
                    {
                        txt[0] = "Только на 2,3 и 4 барабане. Заменяет все символы, кроме скаттера";
                        txt[1] = "Выигрышная комбинация представляет собой непрерывную последовательность одинаковых символов. Все комбинации считаются слева направо на соседних барабанах, на выбранных линиях, начиная с самого левого барабана, за исключением скаттеров. Выигрыш Скаттера добавляются к выигрышам линий. По каждой из линий/комбинаций скаттеров выплачиннется только самый крупный выигрыш. Выигрыши по линиям представляют умножение ставки на значение выигрышной линии. Выигрыши Скаттера умножаются на общую сумму ставки. Неисправность аннулирует все выплаты и игры.";
                        txt[2] = "ЛИНИИ ВЫПЛАТ И ПРАВИЛА";
                        break;
                    }
                default:

                    break;
            }
        }

        public readonly string[] txt;
    }

    public class GLanguage
    {
        Ini _ini = Ini.Instance;

        public GLanguage(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        GoodLuck = "Нехай Щастить";
                        Red = "Червоний";
                        Black = "Чорний";
                        Collect = "Забрати";
                        Autoplay = "";
                        Start = "Старт";
                        Gamble = "Подвоїти";
                        Paytable = "Допомога";
                        PleaseYourBet = "Оберіть Ставку";
                        Close = "Зачинити";
                        _Сurrency = " ГРН";
                        Stop = "Автогра";
                        Scratch = "Скретч";
                        FreeGame = " Бонус ігри";
                        Of = " з ";
                        Won = " виграно";
                        Next = "Далі";
                        WinBonusGame = "Виграно Бонусних Ігор {0}";
                        Congratulations = "Вітання!";
                        Line = " Лінія";
                        Sum = " Сума";
                        _Denom = "1 Кредит = ";
                        DoubleOrTake = "ПодвоЇти чи забрати";
                        break;

                    }
                case 1:
                    {
                        GoodLuck = "Good Luck";
                        Red = "Red";
                        Black = "Black";
                        Collect = "Collect";
                        Start = "Start";
                        Gamble = "Gamble";
                        Paytable = "Paytable";
                        PleaseYourBet = "Please Your Bet";
                        Close = "Close";
                        _Сurrency = _ini.Currency;
                        Stop = "Auto";
                        Scratch = "Scratch";
                        FreeGame = " Bonus Game";
                        Of = " of ";
                        Won = " Won";
                        Next = "Next";
                        WinBonusGame = "Win Bonus Game {0}";
                        Congratulations = "Congratulations!";
                        Line = " Line";
                        Sum = " Sum";
                        _Denom = "1 Credit = ";
                        DoubleOrTake = "Double or Take";
                        break;
                    }
                case 2:
                    {
                        GoodLuck = "Удачи";
                        Red = "Красный";
                        Black = "Черный";
                        Collect = "Забрать";
                        Autoplay = "";
                        Start = "Старт";
                        Gamble = "Удвоить";
                        Paytable = "Таблица";
                        PleaseYourBet = "Пожалуйста, сделайте ставку.";
                        Close = "Закрыть";
                        _Сurrency = " ГРН";
                        Stop = "Автоигра";
                        Scratch = "Скретчь";
                        FreeGame = " Бонусные игры";
                        Of = " из ";
                        Won = " Выиграно";
                        Next = "Дальше";
                        WinBonusGame = "Выиграно бонусных игр {0}";
                        Congratulations = "Поздравления!";
                        Line = " Линия";
                        Sum = " Сумма";
                        _Denom = "1 Кредит = ";
                        DoubleOrTake = "Удвоить или Забрать";
                        break;
                    }

            }
        }

        public readonly string
            GoodLuck = "Good Luck",
            Red = "Red",
            Black = "Black",
            Collect = "Collect",
            Autoplay = "",
            Start = "Start",
            Gamble = "Gamble",
            Paytable = "Paytable",
            PleaseYourBet = "Please Your Bet",
            Close = "Close",
            _Сurrency = " ILS.",
            Stop = "Auto",
            Scratch = "Scratch",
            FreeGame = " Bonus Game",
            Of = " of ",
            Won = " Won",
            Next = "Next",
            WinBonusGame = " Win Bonus Game ",
            Congratulations = "Congratulations!",
            Line = " Line",
            _Denom = "1 Credit = ",
             Sum = " Sum ",
             DoubleOrTake = "Double or Take";
    }

}
