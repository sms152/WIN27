using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace nmWins27
{
    public class CL_TMoney
    {

        System.Random srand = new System.Random();

        uint M_Credit, M_WP, M_ALL, stepen;
        int M_time_delay;
        int fl_stop, fl_StartTake;

        uint[] mas = new uint[1000];
        uint indMas, Count;
        uint Time;

        public CL_TMoney()
        {
            //p = pos.Getpos;
            M_Credit = 0; M_WP = 0;
            M_time_delay = 0;
            fl_stop = 0;
            fl_StartTake = 0;
        }
        void CreateMas(uint cr, uint wp, uint time)
        {
            Count = time;
            if (Count > 1000)
                Count = 1000;
            indMas = 0;

            uint CountHag;
            uint delta = 0;
            for (uint i = 0; i < 1000; i++)
            {
                mas[i] = 0;
            }

            double hag = (double)wp / (double)time;

            if (hag <= 1)
            {
                hag = 1;
                
                CountHag = wp;
                delta = Count / wp;
                int i = 0;
                while ((CountHag > 0) && (i < Count))
                {
                    mas[i] = (uint)hag;

                    i += (int)delta;

                    CountHag--;
                }
                if (CountHag > 0)
                {
                    while (CountHag > 0)
                    {
                        i = srand.Next((int)Count);

                        mas[i] += (uint)hag;
                        CountHag--;
                    }
                }
            }//hag<1
            else
            {
                uint i = 0;
                uint HagInt = wp / time;

                for (i = 0; i < Count; i++)
                {
                    mas[i] = HagInt;
                }

                CountHag = wp - (HagInt * Count);

                while (CountHag > 0)
                {
                    i = (uint)srand.Next((int)Count);
                   
                    mas[i]++;
                    CountHag--;
                    
                }
            }
        }
        void DestroyMas()
        {
            //delete [] mas;
        }

        public void SetTMoney(uint cr, uint wp, uint time)
        {
            M_Credit = cr;
            M_WP = wp;// - cr; ;
            M_ALL = wp;
            CreateMas(M_Credit, M_WP, time);
            fl_StartTake = 1;
            fl_stop = 0;
            indMas = 0;
        }
        void AddCredit(uint win)
        {
            M_Credit += win;
            M_ALL += win;
        }
        public int Test()
        {
            return fl_StartTake;
        }
        public uint GetMoney(int par)
        {
            if (par == 0) return M_Credit;
            else if (par == 1) return M_WP;
            else return stepen;
        }
        public int TakeMoney()
        {
            if (M_WP > 0)
            {
                //     M_time_delay--;
                //     if(M_time_delay<=0)

                if (indMas < Count)
                {
                    stepen = mas[indMas];

                    if ((M_WP >= stepen) && (fl_stop == 0))
                    {
                        M_WP -= stepen;
                        M_Credit += stepen;
                        indMas++;
                    }
                    else
                    {
                        M_Credit = M_ALL;
                        stepen = M_WP;
                        M_WP = 0;
                        indMas = Count;
                        fl_StartTake = 0;

                        return 0;
                    }

                    return 2;
                }
                else
                {
                    fl_StartTake = 0;
                    return 0;
                }
                return 1;
            }
            else
            {
                fl_StartTake = 0;
                return 0;
            }

        }
        public void StopTake()
        {
            if (M_WP > 0)
            {
                fl_stop = 1;
            }
        }

    }
}