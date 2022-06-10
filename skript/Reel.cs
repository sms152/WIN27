//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

namespace nmWins27
{
    public class Reel : MonoBehaviour
    {
        readonly Vector2 shift = new Vector2(0f, 1.74f);

        [SerializeField]
        RSymbol[] symbols;

        public int index;

        public SoundManager m_SoundManager;

        public event VoidDelegate EndReel;//, PreEnd;

        Vector2[][] pointsEnd, pointsRunEnd;
        Vector2 startPosition;

        VoidDelegate _update;

        Data mData;

        Transform trans;

        int count, stepRotate;
        float time;
        bool isStop;
        int ind_rotate;
        float speed = 18f;
        public int SCount { get { return count; } }
        public int LengthReel { get { return symbols.Length; } }

        public void Initialize()
        {
            trans = transform;

            mData = Data.Instance;

            startPosition = trans.localPosition;

            for (int i = 0; i < symbols.Length; i++)
            {
                symbols[i].Initialize();
            }
            
            pointsEnd = new Vector2[][]
            {
                new Vector2[]{ startPosition, startPosition - shift * .15f },
                new Vector2[]{ startPosition - shift * .15f, startPosition }
            };

            pointsRunEnd = new Vector2[][]
            {
                new Vector2[]{ startPosition, startPosition - shift * .15f },
                 new Vector2[]{ startPosition - shift * .15f, startPosition }
            };
        }

        void Start()
        {
            enabled = false;
        }

        public void SetSymbols(byte[] symbol)
        {
            for (int i = 0; i < symbols.Length; ++i)
                symbols[i].SetSymbol(symbol[i]);
        }

        public void Play(int c, float offset)
        {
            speed = 18f;
            count = c < 0 ? -c : c;
            time = offset > 0f ? -offset : offset;
            stepRotate = 0;
            ind_rotate = Random.Range(0, 30);
            _update = Rotate;
            enabled = true;
            isStop = false;
        }

        public void Stop(int c = 4)
        {
            if (count < 5 || c < 4)
                return;

            count = c < 0 ? -c : c;

            isStop = true;
        }
        int[] masrotate = { 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 3, 3, 3, 3, 3, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 2, 2, 2, 2, 9, 8, 8, 8, 8, 8, 5, 5, 5, 5, 5, 5, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 3, 3, 3, 3, 6, 6, 6, 6, 6, 4, 4, 4, 4, 4, 4, 4, 7, 7, 7, 8, 8, 8, 8, 8, 2, 2, 2, 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 7, 7, 7, 7, 7, 7, 3, 3, 3, 3, 3, 3, 5, 5, 5, 5, 5, 5, 1, 1, 1, 1, 1, 1, 6, 6, 6, 6, 6, 6, 3, 3, 3, 3, 3, 3 };
        void Rotate()
        {
            if (time > 1f)
            {
                time = 0;

                //symbols[5].Image = symbols[4].Image;
                symbols[4].Image = symbols[3].Image;
                symbols[3].Image = symbols[2].Image;
                symbols[2].Image = symbols[1].Image;
                symbols[1].Image = symbols[0].Image;

                if (--count > 4)
                {
                    if ((index == 0) || (index == 4))
                    {
                        ind_rotate++;
                        while (masrotate[ind_rotate] == 8)
                        {
                            ind_rotate++;
                        }
                    }
                    else
                    {
                        ind_rotate++;
                    }
                    symbols[0].SetSymbol(masrotate[ind_rotate]);
                }
                else
                {
                    if (count > 0)
                    {
                        symbols[0].SetSymbol(mData.roll[index][count]);
                    }
                    else
                    {
                        var r = mData.roll;

                        symbols[0].SetSymbol(r[index][0]);
                        symbols[1].SetSymbol(r[index][1]);//, (r[index][1] == 8) ? -2.1f : 0);
                        symbols[2].SetSymbol(r[index][2]);//, (r[index][2] == 8) ? -2.2f : 0);
                        symbols[3].SetSymbol(r[index][3]);//, (r[index][3] == 8) ? -2.3f : 0);
                        symbols[4].SetSymbol(r[index][4]);
                      

                        m_SoundManager.ReelStop(index);

                        _update = isStop ? RunEndRotate : (VoidDelegate)EndRotate;

                        if (!isStop)
                        {
                            if ((mData.flMoln == 0) && (mData.WaitMoln > 0) && (index == mData.WaitMoln))
                            {
                                mData.flMoln = 1;
                            }
                        }
                    }
                }
            }

            trans.localPosition = (1f - time) * startPosition + time * (startPosition - shift);
        }

        void EndRotate()
        {
            if (time > 1f)
            {
                time = 0;
                speed = 5f;
                if (++stepRotate > 1)
                {
                    stepRotate = 1;

                    time = 1f;

                    _update = EndUpdate;
                }
            }

            trans.localPosition = (1f - time) * pointsEnd[stepRotate][0] + time * pointsEnd[stepRotate][1];
        }

        void RunEndRotate()
        {
            if (time > 1f)
            {
                time = 0;
                speed = 5f;
                if (++stepRotate > 1)
                {
                    stepRotate = 1;

                    time = 1f;

                    _update = EndUpdate;
                }
            }

            trans.localPosition = (1f - time) * pointsRunEnd[stepRotate][0] + time * pointsRunEnd[stepRotate][1];
        }

        void EndUpdate()
        {
            enabled = false;

            _update = null;

            if (EndReel != null)
                EndReel();
        }

        void Update()
        {
            time += Time.smoothDeltaTime * ((mData.flMoln > 0) ? 7f : speed);

            if (_update == null)
                enabled = false;
            else if (time > 0f)
                _update();
        }
    }
}