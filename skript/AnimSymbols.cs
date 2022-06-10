using UnityEngine;
using InterfaceGame;

namespace nmWins27
{
    public class AnimSymbols : MonoBehaviour
    {
        readonly float[] lengWin = { 1.129f, 1.733f, 1.692f, 3.409f, 2.77f, 2.132f, 3.21f, 2.04f, 2.647f, 3.725f, 4.882f,
        4.362f, 4.842f, 4.884f, 5.568f, 10.187f}; //big win  10..14 11..15

        Vector3[] startPosition = new Vector3[5];


        [SerializeField]
        RSymbol[] symbols;

        [SerializeField]
        Border[] borders;

        [SerializeField]
        NovoInterface m_Interface;

        [SerializeField]
        AnimBigMoney m_BigMoney;

        [SerializeField]
        ManagerLanguage m_ManagerLanguage;
        
        public SoundManager m_SoundManager;

        GLanguage m_Language;

        Data mData;

        int show_line_new = 0;
        int stop_add_wp = 0;
       
        WaitForSeconds wait = new WaitForSeconds(0f); //3

        public event VoidDelegate End;
        VoidDelegate _update, _wait_update;

        float time;
        int indLineWin = 0, ShowLineWin = 0, CountWin = 0;
        int firstanim = 0;


        int playWinSnd;

        int flStopAnim = 0;
        CL_TMoney TMoney;

    
        public void Initialize()
        {
            int i;
            mData = Data.Instance;
            for (i = 0; i < borders.Length; i++)
            {
                borders[i].Initialize();
            }
            
            Stop(0);
            TMoney = new CL_TMoney();
            m_BigMoney.Hide();
            m_Language = m_ManagerLanguage.m_GLanguage;

        }
        public void Start()
        {
            
        }
        public void Clear()
        {
            CountWin = 0;
            flStopAnim = 0;
        }
        public void BreakAnim()
        {
            if ((enabled) && (flStopAnim == 0))
            {
                flStopAnim = 1;

                if (TMoney.Test() > 0)
                {
                    flStopAnim = 2;
                    TMoney.StopTake();
                    TicMoney();
                }

            }
        }
        void SetForvard(int Count)
        {


            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (mData.masAnim[i + j * 3] == 1) 
                    {
                        if (symbols[i + j * 3].Position.z == -3.2f)
                        {
                            symbols[i + j * 3].Forward(-3f);// symbols[mData.masWin[i].pNormal[j] + j * 4].Position.z + 1f);
                        }
                      
                    }

                }
            }
        }
        void SetWild()
        {
            int i, j;
            if (mData.masIdol.first > 0)
            {
                for (j = 0; j < mData.masIdol.pNormal.Length; j++)
                {
                    if (mData.masIdol.pNormal[j] >= 0)
                    {
                        for (i = 0; i < 3; i++)
                        {
                            if (symbols[i + j * 3].GetIndexSymbol() != mData.masIdol.symv)
                            {
                                symbols[i + j * 3].SetSymbol(mData.masIdol.symv);
                            }
                        }
                    }
                }
            }
        }
        public void StartAnim(int Count)
        {
            float ofsZ_border;
            int i, j;
            stop_add_wp = 0;

            Clear();


            CountWin = Count;
            enabled = true;
            firstanim = 0;

            if (mData.masIdol.first > 0)
            {
               
                time = 0f;
                _wait_update = StartAnimAll;
            }
            else
            {
                StartAnimAll();
            }


        }

        void StartAnimAll()
        {
            float ofsZ_border;
            int i, i2, j;

            indLineWin = 0;
            ShowLineWin = 0;

            

            //for (i = 0; i < CountWin; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    for (i2 = 0; i2 < 3; i2++)
                    {
                        if (mData.masAnim[i2 + j * 3] == 1)
                        {
                            //symbols[i2 + j * 3].Play(1);
                            i = symbols[i2 + j * 3].GetIndexSymbol();
                            if (i >= 9)
                            {
                                symbols[i2 + j * 3].Play(0, 0, false);

                                if (i == 9) {
                                    symbols[i2 + j * 3].PreEnd += symbols[i2 + j * 3].PlayNext;
                                    ofsZ_border = -3.3f;
                                }
                                else
                                {
                                    ofsZ_border = -3f;
                                }
                                
                            }
                            else
                            {
                                symbols[i2 + j * 3].Play(0, 0);
                                ofsZ_border = -3f;
                            }
                            
                            symbols[i2 + j * 3].Forward(ofsZ_border);
                        }
                    }
                }

            
            }

            TickNormal();
        }
    
        void TickNormal()
        {
            int comb = 0;
            int side = 0;
            int regim = 0;
            int i = 0;

            SetForvard(CountWin);
           
            Color cc = new Color(1f, 0f, 0f, 1);
            if (mData.masWin[indLineWin].symv == 10)
            {
                cc = new Color(0f, 1f, 0f, 1);
            }
            ShowLineWin = indLineWin;
            

            for (i = 0; i < borders.Length; i++)
            {
                borders[i].HideBorder();
            }
            

            for ( i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (mData.masAnim[i + j * 3] == 1)
                    {
                        borders[i + j * 3].ShowBorder(cc);

                        if (symbols[i + j * 3].Position.z == -3f)
                        {
                            symbols[i + j * 3].Forward(-3.2f);
                        }
                    }
                }
            }

          

            if (firstanim<=1)
            {
             

                        time = -2.5f;

                        if ((mData.masWin[indLineWin].numline == 41))
                        {
                            if (firstanim == 0)
                            {
                                // wait = new WaitForSeconds(.1f);
                                m_SoundManager.PlaySound(25, 1);
                                playWinSnd = 8;
                                //SoundManagerZeus.PlaySound(104, 0, false);
                            }
                        }else
                        if ((mData.masWin[indLineWin].numline == 42))
                        {
                            if (firstanim == 0)
                            {
                                // wait = new WaitForSeconds(.1f);
                                m_SoundManager.PlaySound(26, 1);
                                playWinSnd = 9;
                                //SoundManagerZeus.PlaySound(104, 0, false);
                            }
                        }
                        else
                        {
                            if ((mData.Five_of_kind)|| (mData.WP > 25 * mData.TotalBet))
                            {
                                if (mData.WP <= 10 * mData.TotalBet)
                                {
                                    playWinSnd = 11;
                                }
                                else
                                if (mData.WP <= 15 * mData.TotalBet)
                                {
                                    playWinSnd = 12;
                                }
                                else
                                if (mData.WP <= 20 * mData.TotalBet)
                                {
                                    playWinSnd = 13;
                                }
                                else
                                if (mData.WP <= 25 * mData.TotalBet)
                                {
                                    playWinSnd = 14;
                                }
                                else
                                {
                                    //### start money
                                    m_BigMoney.Play();
                                    playWinSnd = 15;
                                }
                                m_SoundManager.PlaySound(playWinSnd + 20, 1);
                            }
                            else
                            {
                                m_SoundManager.PlaySound(mData.masWin[indLineWin].symv + 16, 1);
                                playWinSnd = mData.masWin[indLineWin].symv;
                            }
                        }

                        TMoney.SetTMoney((uint)mData.shWP, (uint)mData.WP, (uint)((lengWin[playWinSnd]+10.013f) / Time.smoothDeltaTime));

                        if (show_line_new == 1)
                        {
                            mData.shWP = mData.masWin[indLineWin].win;
                            m_Interface.vDialog.text = m_Language.Won + " " + mData.shWP.ToString();
                        }
                        firstanim = 1;
                    
                
                firstanim=2;
            }
            else
            {
                if (show_line_new == 0)
                {
                    time = -2.5f;
                }
                else
                {
                    if (firstanim == 3)
                    {
                        time = -0.5f;
                    }
                    else
                    {
                        time = -2.5f;
                    }

                    if( (firstanim <= 3)&& (stop_add_wp==0))
                    {
                        mData.shWP += mData.masWin[indLineWin].win;
                        m_Interface.vDialog.text = m_Language.Won + " " + mData.shWP.ToString();
                    }
                }
            }

            indLineWin = (indLineWin + 1) >= CountWin ? 0 : indLineWin + 1;
            if (indLineWin == 0)
            {
                stop_add_wp = 1;
                StartCoroutine(Wait());
            }
            _update = TickNormal;
        }
        void SetRamkColor(Color cc)
        {
           
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (mData.masAnim[i + j * 3] == 1)
                    {
                        borders[i + j * 3].SetColorBorder(cc);
                    }
                }
            }
           
        }
        void TicMoney()
        {
            if (TMoney.Test() > 0)
            {
                if (flStopAnim == 1)
                {
                    flStopAnim = 2;
                    TMoney.StopTake();
                }
                int par = TMoney.TakeMoney();
                if (par == 2)
                {
                    if (show_line_new == 0)
                    {
                        mData.shWP = (int)TMoney.GetMoney(0);
                        m_Interface.vDialog.text = m_Language.Won + " " + mData.shWP.ToString();
                    }
                    if( (!m_SoundManager.IsPlaying(1)) && (firstanim==2))
                    {
                        firstanim = 3;
                        m_SoundManager.PlaySound(15, 0);
                    }
                }
                else
                if (par == 0)
                {

                    mData.shWP = mData.WP;
                    m_Interface.vDialog.text = m_Language.Won + " " + mData.shWP.ToString();
                    if (show_line_new == 0) m_SoundManager.PlaySound(1, 2);
                    m_BigMoney.Hide();
                    
                    if (End != null)
                    {
                        if (show_line_new == 1)
                        {
                            firstanim = 4; stop_add_wp = 1;
                        }
                        End();
                        End = null;
                    }
                }
            }
        }
        public void Stop(int pos=1)
        {
            _update = null;

            int i;
            float zz = -1f;
            for (i = 0; i < symbols.Length; i++)
            {
                if (i % 4 == 0) zz = -1f;
                symbols[i].Normal();
                symbols[i].Back(zz);
                zz += -0.1f;
                
            }
            if(pos==1)
          
            for (i=0; i< borders.Length; i++)
            {
                borders[i].HideBorder();
            }
            m_BigMoney.Hide();
            
            m_SoundManager.StopSound(0);
            m_SoundManager.StopSound(1);
        }
        float colorPulseFactor = -1;
        int znak = 1;

        void Update()
        {
            if (_wait_update != null)
            {
                time += Time.deltaTime;
                if (time < 0f) return;

                _wait_update();
                _wait_update = null;
                return;
            }
            if (_update != null)
            {
                TicMoney();

                time += Time.deltaTime;

                colorPulseFactor -= Time.deltaTime;// 0.15f;
                if (colorPulseFactor < 0f)
                {
                    colorPulseFactor = .5f;
                    Color c = new Color(1f, 0f, 0f, 1);// mData.LineColor[mData.masWin[ShowLineWin].numline];
                    if (mData.masWin[indLineWin].symv == 10)
                    {
                        c = new Color(0f, 1f, 0f, 1);
                    }

                    if (znak > 0)
                    {
                        znak = -1;
                        c.r += 0.2f * znak;
                        c.g += 0.2f * znak;
                        c.b += 0.2f * znak;
                    }
                    else znak = 1;

                    SetRamkColor(c);
                }

                

                if (time < 0f)
                    return;

                

                _update();
            }
            else
                enabled = false;
        }

        System.Collections.IEnumerator Wait()
        {
            if (flStopAnim > 0) yield break;

            yield return wait;

            if (show_line_new > 0)
            {
                if (End != null)
                {
                    firstanim = 4; stop_add_wp = 1;
                    End();
                    End = null;
                }
            }
        }
    }
}