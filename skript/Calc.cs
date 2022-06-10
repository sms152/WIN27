using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace nmWins27
{
    public class Calc
    {
        Data m_Data;

       public  void Initialize()
        {
            m_Data = Data.Instance;
        }

        byte[][] MBaraban = new byte[5][]
            {
            new byte[5],
            new byte[5],
            new byte[5],
            new byte[5],
            new byte[5],
            };

        byte[] matWild = new byte[5];

        const byte WILD = 9;
        const byte WIN27 = 10;
        //const byte BAKS = 9;

        int[,] mZ_WIN = new int[11, 2] { //matrica wiigrihei
                            {0,2},          //  0   Cherry
                            {0,2},      //  1   Lemon
                            {0,2},      //  2    Orange
                            {0,4},     //  3  Plume
                            {0,4},     //  4   Melon
                            {0,10},     //  5   Grape
                            {0,10},      //  6   Apple
                            {0,30},      //  7   Bell
                            {0,30},       //  8   Seven
                            {0,50},       //  9   Wild
                            {0,1},       //  10   Win27
                          };
        int[] Win27 = new int[6] { 25, 40, 55, 70, 85, 100 };
      
        int TestPriz(int sum, int simv)
        {
            int rez = 0;

            if ((sum >= 2) && (sum <= 5))
            {
                if (simv < 10)
                {
                    rez = mZ_WIN[simv, sum - 2];
                }
                else
                {
                    rez = Win27[MBaraban[4][4]];
                }
            }
           

            return rez;
        }
        int Obrabotka(int i, ref int SYMVOL)
        {
            int k=0, kk = 1;

            for (int j = 0; j <= 2; j++)
            {
                k = SummmaSymvolLent(j, (byte)SYMVOL);
                if (k > 0)
                {
                    kk *= k;
                }
                else
                {
                    kk = 0;
                    break;
                }
            }

            return kk;
        }

        byte SummmaSymvol(byte symv)
        {
            byte sum = 0;
            for (int i = 0; i < 3; i++)
                for (int j = 1; j < 4; j++)
                    if (MBaraban[i][j] == symv) { sum++; }
            return sum;
        }
        byte SetSymvolAnim(byte symv)
        {
            byte sum = 0;
            for (int i = 0; i < 3; i++)
                for (int j = 1; j < 4; j++)
                    if (MBaraban[i][j] == symv) { m_Data.masAnim[i*3+(j-1)]=1; }
                    else if ((MBaraban[i][j] == WILD) && (matWild[i]>0)) { m_Data.masAnim[i * 3 + (j - 1)] = 1; }
            return sum;
        }

        byte SummmaSymvolLent(int indexLent, byte symv, byte fl_add=1)
        {
            byte sum = 0;
            for (int j = 1; j < 4; j++)
                if (MBaraban[indexLent][j] == symv) {
                    sum++;
                }else if((MBaraban[indexLent][j] == WILD) && (WIN27 != symv))
                {
                    sum++;
                    if(fl_add==1) matWild[indexLent]++;
                    else if(matWild[indexLent]>0) matWild[indexLent]--;
                }
        
            return sum;
        }
        byte GetPosSymvolLent(int indexLent, byte symv)
        {
            byte pos = 0;
            for (byte j = 1; j < 4; j++)
                if (MBaraban[indexLent][j] == symv) { pos = j; }
            return pos;
        }

        int indLineWin = 0;//, CountWin = 0;


        public void MakeWinLine()
        {

            int i, j;
            int priz, sum, SYMVOL;
            int lox_win = 0;

            for (i = 0; i < 9; i++)
            {
                m_Data.masAnim[i] = 0;
            }
            for (i = 0; i < 5; i++)
            {
                matWild[i] = 0;
            }
            MBaraban = m_Data.roll;

            indLineWin = 0; m_Data.CountWin = 0;

            m_Data.WP = 0; m_Data.shWP = 0;

            m_Data.LineWin = 0;
            m_Data.Five_of_kind = false;
            m_Data.masIdol.first = 0;
            m_Data.WaitMoln = -1;
            m_Data.flMoln = 0;
            m_Data.indexWin27 = -1;

            for (i = 0; i <= 10; i++)
            {
                priz = 0;
                sum = 0;

                SYMVOL = i;

         
                sum = Obrabotka((byte)i, ref SYMVOL);
                if (sum > 0)
                {
                    priz = TestPriz((byte)3, SYMVOL);
                    if (priz > 0)
                    {
                        indLineWin = m_Data.CountWin;

                      
                        if (SYMVOL == WIN27) {
                            m_Data.masIdol.first = 1;
                            m_Data.WaitMoln = 1;
                            m_Data.indexWin27 = MBaraban[4][4];
                        }

                        SetSymvolAnim((byte)SYMVOL);
                        m_Data.masWin[indLineWin].symv = SYMVOL;
                        m_Data.masWin[indLineWin].kolvo = sum;
                        m_Data.masWin[indLineWin].numline = i;
                       
                            m_Data.masWin[indLineWin].first = 0;

                        m_Data.masWin[indLineWin].win = (sum * priz * m_Data.Bet);
                        

                        m_Data.CountWin++;


                        // Debug.Log("Lines " + i+1+", Symbol "+SYMVOL+", sum "+sum+", win "+ masWin[indLineWin].win);

                        m_Data.WP += sum * priz * m_Data.Bet;

                        lox_win += (sum * priz * m_Data.Bet);
                    }
                }
                else
                {
                    for (j = 0; j < 3; j++)
                        SummmaSymvolLent(j, (byte)SYMVOL, 0);
                }
            }

            m_Data.reelStopSound = new sbyte[5] { 13, 13, 13, 13, 13 };
            //-----------------------------SEt Sound Scater stop
            int count_scater = 0;
            for (int i2 = 0; i2 < 3; i2++)
            {
                for (int j2 = 1; j2 < 3+1; j2++)
                    if (MBaraban[i2][j2] == WIN27)
                    {
                        if ((i2 == 0) || ((i2 == 1) && (count_scater > 0)) || ((i2 == 2) && (count_scater >= 2)))
                        {
                            m_Data.reelStopSound[i2] = (sbyte)(2 + i2);
                        }
                        count_scater++;
                        break;
                    }
            }
            //--------------------------------------------------------------------
            m_Data.LineWin = lox_win;
            m_Data.wonRoll = lox_win;
            //return m_Data.CountWin;

        }
    }
}