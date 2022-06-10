using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace nmWins27
{
    public class bonus : MonoBehaviour
    {
        [SerializeField]
        UnityEngine.UI.Text[] LineText;

        [SerializeField]
        SpriteRenderer sprt_fon;

        [SerializeField]
        SpriteRenderer sprt_col;

        [SerializeField]
        Transform _transform_col;

        [SerializeField]
        AnimStar m_AnimStar;

        Data m_Data;

        GameObject obj;
        public event VoidDelegate End;
        float time_exit = 0;

        int count_index_anim;
        int show_index = 5;
        bool side_down = true;
        float time = 0;

        float[] pos_tablo = {-1.93f, -1.46f, -.99f, -.52f, -.05f, .42f };
        // Use this for initialization
        private void Awake()
        {
            obj = gameObject;
        }

        public void Initialize()
        {
            m_Data = Data.Instance;
            End = null;
            enabled = false;
            Hide();
        }
        public void Show()
        {
            int i;
            int[] maspyr = { 25, 40, 55, 70, 85, 100 };
            count_index_anim = 0;
            show_index = 5;
            side_down = true;
            for (i = 0; i < LineText.Length; i++)
            {
                LineText[i].text = (maspyr[i] * m_Data.Bet).ToString();
                LineText[i].enabled = true;
            }
            sprt_fon.enabled = true;
            sprt_col.enabled = true;

            m_AnimStar.enabled = false;

            obj.SetActive(true);

            m_AnimStar.PlayAnim(0, 1);

            enabled = true;
        }
        public void Hide()
        {
            int i;
            for (i = 0; i < LineText.Length; i++)
            {
                LineText[i].enabled = false;
            }
            sprt_fon.enabled = false;
            sprt_col.enabled = false;

            m_AnimStar.enabled = false;

            enabled = false;
            obj.SetActive(false);
        }
        // Update is called once per frame
        void Update()
        {
            if (sprt_fon.enabled)
            {
                if (count_index_anim == 4)
                {
                    if (End != null)
                    {
                        time_exit += Time.smoothDeltaTime;
                        if (time_exit >= 0)
                        {
                            Hide();
                            End();
                            End = null;
                        }
                    }
                }

                time += Time.smoothDeltaTime;

                if (time < 0f)
                    return;

                time -= .15f;

                if (count_index_anim < 2) {
                    if (side_down) {
                        if (show_index > 0) show_index--;
                        if (show_index == 0) side_down = false;
                    }
                    else
                    {
                        if (show_index < 5) show_index++;
                        if (show_index == 5)
                        {
                            count_index_anim++;
                            side_down = true;
                        }
                    }
                    _transform_col.localPosition = new Vector3(_transform_col.localPosition.x, pos_tablo[show_index], _transform_col.localPosition.z);
                }
                else if(count_index_anim == 2)
                {
                    if (show_index != m_Data.indexWin27)
                    {
                        show_index--;
                        _transform_col.localPosition = new Vector3(_transform_col.localPosition.x, pos_tablo[show_index], _transform_col.localPosition.z);
                    }
                    else
                    {
                        count_index_anim = 3;
                        side_down = true;
                        _transform_col.localPosition = new Vector3(_transform_col.localPosition.x, pos_tablo[show_index], _transform_col.localPosition.z);
                        show_index = 0;
                    }
                    
                }
                else if (count_index_anim == 3)//mig 
                {
                    side_down = !side_down;
                    if (!side_down) show_index++;
                    sprt_col.enabled = side_down;

                    if ((side_down) && (show_index > 3))
                    {
                        count_index_anim = 4;//start exit
                        time_exit = -3f;
                    }
                }

            }
        }
    }
}