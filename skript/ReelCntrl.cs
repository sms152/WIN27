using System.Collections.Generic;
using UnityEngine;

namespace nmWins27
{
    public class ReelCntrl : MonoBehaviour
    {
        NewGame2.SymbolList m_SymbolList;

        public SoundManager m_SoundManager;

        public void Initialize()
        {
            m_SymbolList = NewGame2.SymbolList.Instance;

            mData = Data.Instance;

            for (int i = 0; i < reels.Length; ++i)
            {
                reels[i].Initialize();

                reels[i].index = i;

                reels[i].m_SoundManager = m_SoundManager;

                reels[i].EndReel += EndReel;
            }
        }

        [SerializeField]
        Reel[] reels;

        Data mData;

        int count, countReel;

        public event VoidDelegate End;

        //public void TimeStop(float index) { wait = new WaitForSeconds(index); }

        public void Run(int WaitMoln)
        {
            count = 0;

            countReel = 3;

            m_SoundManager.PlaySound(8, 0);

            reels[0].Play(12, 0f);
            reels[1].Play(19, .1f);
            if (WaitMoln == 1)
            {
                reels[2].Play(26+12, .2f);
            }
            else
            {
                reels[2].Play(26, .2f);
            }

        }

        void EndReel()
        {
            if (++count == countReel)
            {
                StartCoroutine(Wait());
            }
        }
        public void Stop()
        {
            for (int i = 0; i < reels.Length; i++)
                reels[i].Stop();
        }
        public void SetSymbols(byte[][] symbol = null)
        {
            byte[][] s = symbol;

            if (symbol == null)
                s = mData.roll;

            for (int c = 0; c < reels.Length; ++c)
            {
                reels[c].SetSymbols(s[c]);
            }
        }

        System.Collections.IEnumerator Wait()
        {
            yield return new WaitForSeconds(.02f);

            if (End != null)
                End();
        }
    }
}