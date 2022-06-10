using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace nmWins27
{
    public class AnimStar : MonoBehaviour
    {
        Transform _transform;
        SpriteRenderer sprt;

        [SerializeField]
        NewGame2.SAnimation[] _play;

        int anim, index, leng;
        bool isPlay;
        float time = 0f;

        void Awake()
        {
            _transform = transform;
            sprt = GetComponent<SpriteRenderer>();
            sprt.enabled = false;
            isPlay = false;
            enabled = false;
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        void StopAnim()
        {
            //sprt.enabled = false;
            isPlay = false;
        }

        public void PlayAnim(int index_anim, int count5)
        {
            if ((_play == null) || (index_anim >= _play.Length))
                return;

            anim = index_anim;
            index = 0;

            sprt.sprite = _play[anim][index];
            sprt.enabled = true;

            leng = _play[anim].Length;

            time = 0f; //first delay

            isPlay = true;
            enabled = true;
        }
        // Update is called once per frame
        void Update()
        {
            if (isPlay)
            {


                time += Time.smoothDeltaTime;

                if (time < 0f)
                    return;

                time -= _play[anim].Fps;

                if (index + 1 >= leng)
                {
                    if (_play[anim].IsLoop)
                    {
                        index = 0;
                    }
                    else
                    {
                        StopAnim();
                        return;
                    }
                }
                else
                {
                    index++;
                }
                sprt.sprite = _play[anim][index];

            }
        }
    }
}
