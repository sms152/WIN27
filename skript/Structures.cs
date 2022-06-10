using Data;
using UnityEngine;

namespace nmWins27
{
    public struct StmasWin
    {
        public int symv;
        public int kolvo;
        public int numline;

        public int[] pNormal;
        public int win;

        public int first;

    }

    public class Data
    {
        static Data _instance;

        public static Data Instance { get { if (_instance == null) _instance = new Data(); return _instance; } }

        public static void Dispose() { _instance = null; }

        public int Lines, LineWin, WP, shWP, Bet, TotalBet, inBet, shCREDIT;
        public float video_denom;

        public int CountWin;
        public int[] masAnim = new int[9];
        public StmasWin[] masWin
            ;
        public StmasWin masIdol;
        public int WaitMoln, flMoln;
        public int indexWin27 = 0;

        public bool Five_of_kind;

        public sbyte[] reelStopSound = new sbyte[] { 14, 14, 14, 14, 14 };

        public readonly int[][] masLine = new int[5][]
        {
            new int[3] {1,1,1},
            new int[3] {0,0,0},
            new int[3] {2,2,2},
            new int[3] {0,1,2},
            new int[3] {2,1,0},

       
        };

        int[] BETs_malan = { 100, 200, 400, 800, 1000, 2000, 3000, 4000 };
        int[] BETs_full = { 10, 20, 40, 80, 200, 400, 600, 800, 1000 };
        public int[] BETs
        {
            get { return (Ini.Project == 1) ? BETs_malan : BETs_full; }
        }

        public int MINBET = 0;
        public int MAXBET
        {
            get { return (Ini.Project == 1) ? BETs_malan.Length - 1 : BETs_full.Length - 1; }
        }

        public int findIndex(int bb)
        {
            for (int i = 0; i < MAXBET+1; i++)
            {
                if (BETs[i] == bb) return i;
            }
            return 0;
        }
        public void FindNextBet(byte side, byte par = 0)
        {//0 - ++,1 - --

            int TestBet;
            byte find;
            uint MIN_BET = 1, MAX_BET =2000;
            uint lw_credit = 0;
            byte lb_status = 0;
            UPC_Work_Old.upc_dll.Get.UPC_Status(ref lw_credit, ref lb_status, ref video_denom);
            do
            {
                if (par == 0)
                {
                    if (side == 0)
                    {
                        inBet++;
                        if (inBet > MAXBET) inBet = MINBET;
                    }
                    else
                    {
                        if (inBet > MINBET) inBet--;
                        else inBet = MAXBET;
                    }
                }
                par = 0;
                TestBet = (int)(5*BETs[inBet] * video_denom);
                Bet = BETs[inBet];
                if ( (TestBet%10==0) && ((double)(Bet * Lines * video_denom) % 10 == 0))
                {
                    UPC_Work_Old.upc_dll.Get.UPC_MinMaxBet(ref MIN_BET, ref MAX_BET);

                    if (MIN_BET > BETs[inBet] * video_denom) find = 0;
                    else
                 if (MAX_BET < BETs[inBet] * video_denom) find = 0;
                    else find = 1;

                }
                else find = 0;

            } while (find == 0);

        }
        //=============================
        public LineWin[] lines;

        public bool isWinAny;

        public int credit,
                   wonRoll,
                   wonTicket,
                   wonFragTicket,
                   previousWonFragTicket,
                   cost,
                   line = 5,
                   bet;
        public uint bonus_cred;
        public uint bonus_ex_type;
        public string series,
                      ticket,
                      denom,
                      cash,
                      price,
                      bonus,
                      card,
                      product;

        

        public int[] betValues;
        public bool isBigWin;

        public byte[][] roll;
        public byte[][] rollPlay;

        public Color[] LineColor = new Color[43]
        {
            new Color(0.466f,0.435f,0.443f,1),
            new Color(0.972f,0.686f,0.047f,1),
            new Color(1f,0f,0f,1),
new Color(0.831f,0.6f,0.149f,1),
new Color(0,0.745f,0.717f,1),
new Color(0.800f,0.133f,0.113f,1),
new Color(0.325f,0.298f,0.141f,1),
new Color(0,0.305f,0.035f,1),
new Color(0.717f,0.576f,0,1),
new Color(0,0.643f,0.274f,1),
new Color(0.647f,0.352f,0.113f,1),
new Color(0.839f,0.486f,0.184f,1),
new Color(0,0.631f,0.584f,1),
new Color(0.898f,0.286f,0.686f,1),
new Color(0.792f,0.309f,0.109f,1),
new Color(0.870f,0.337f,0.337f,1),
new Color(0.835f,0.470f,0.631f,1),
new Color(0.745f,0.529f,0.298f,1),
new Color(0.839f,0.254f,0.690f,1),
new Color(0.862f,0.149f,0.333f,1),
new Color(0.466f,0.568f,0.490f,1),
new Color(0.980f,0.203f,0.192f,1),
new Color(0.415f,0.223f,0.580f,1),
new Color(0.305f,0.333f,0.705f,1),
new Color(0.698f,0.313f,0.760f,1),
new Color(0.290f,0.250f,0.701f,1),
new Color(0.713f,0.407f,0.333f,1),
new Color(0,0.419f,0.388f,1),
new Color(0.772f,0.678f,0.470f,1),
new Color(0.721f,0.658f,0.121f,1),
new Color(0.776f,0.674f,0.639f,1),
new Color(0.796f,0.305f,0.101f,1),
new Color(0.741f,0.262f,0.447f,1),
new Color(0.749f,0.568f,0.376f,1),
new Color(0.505f,0.631f,0.701f,1),
new Color(0.725f,0.152f,0.223f,1),
new Color(0.015f,0.254f,0.674f,1),
new Color(0.631f,0.584f,0.6f,1),
            new Color(0.862f,0.262f,0.639f,1),
            new Color(0.800f,0.615f,0.192f,1),
            new Color(0.800f,0.615f,0.192f,1),
            new Color(0.0f,1.0f,0.0f,1),
            new Color(0.0f,1.0f,0.0f,1)
        };

        Data()
        {
            CountWin = 0;
            masWin = new StmasWin[43];

            betValues = new int[1];

            rollPlay = new byte[5][];

            roll = new byte[5][];

            for (int i = 0; i < 5; i++)
            {
                roll[i] = new byte[5];
                rollPlay[i] = new byte[3];
            }

        }
    }

}