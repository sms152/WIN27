using UnityEngine;

namespace nmWins27
{
    public class AnimationCard : MonoBehaviour
    {
        [SerializeField]
        UnityEngine.UI.Image[] card;

        [SerializeField]
        Sprite red, black;

        float vShift, mHeight, mWidth, nHeight, nWidth;

        Vector2 mPos, c1Pos, c2Pos,
            c3Pos, c4Pos, c5Pos;

        public event VoidDelegate End;

        VoidDelegate update;

        Vector2 pos;

        Color color = Color.white;

        Sprite cardWin;

        float time;

        bool temp;

        void Awake()
        {
            enabled = false;
        }

        public void Initialize()
        {
            mPos = card[0].rectTransform.localPosition;
            mHeight = card[0].rectTransform.GetHeight();
            mWidth = card[0].rectTransform.GetWidth();
            nHeight = card[1].rectTransform.GetHeight();
            nWidth = card[1].rectTransform.GetWidth();

            c1Pos = card[1].rectTransform.localPosition;
            c2Pos = card[2].rectTransform.localPosition;
            c3Pos = card[3].rectTransform.localPosition;
            c4Pos = card[4].rectTransform.localPosition;
            c5Pos = card[5].rectTransform.localPosition;

            vShift = c2Pos.x - c1Pos.x;

            SetUpdate(null);
        }

        public void SetCard(Sprite[] spr)
        {
            for (int i = 0; i < 5; i++)
                card[i + 1].sprite = spr[i];
            //for (int i = 1, Length = spr.Length + 1; i < Length; i++)
            //    card[i].sprite = spr[i - 1];
        }

        public void PlayRotate()
        {
            SetUpdate(RotateCard);
        }

        public void PlayShift(Sprite card)
        {
            cardWin = card;

            SetUpdate(CloseShiftCardAnimation);
        }

        public void PlayOpen(Sprite card)
        {
            cardWin = card;

            SetUpdate(CloseCardAnimation);
        }

        void ShiftCard()
        {
            card[5].sprite = card[4].sprite;
            card[4].sprite = card[3].sprite;
            card[3].sprite = card[2].sprite;
            card[2].sprite = card[1].sprite;
            card[1].sprite = card[0].sprite;
            card[0].sprite = cardWin;
        }

        public void SetDefault()
        {
            color.a = 1f;

            card[0].rectTransform.localPosition = mPos;
            card[0].rectTransform.SetHeight(mHeight);
            card[0].rectTransform.SetWidth(mWidth);

            card[0].sprite = red;

            card[5].rectTransform.localPosition = c5Pos;
            card[4].rectTransform.localPosition = c4Pos;
            card[3].rectTransform.localPosition = c3Pos;
            card[2].rectTransform.localPosition = c2Pos;
            card[1].rectTransform.localPosition = c1Pos;

            card[5].color = color;
        }

        void RotateCard()
        {
            time += Time.deltaTime;

            if (time > .1f)
            {
                card[0].sprite = temp ? red : black;

                temp = !temp;
                time = 0f;
            }
        }

        void CloseCardAnimation()
        {
            time += Time.smoothDeltaTime * 2.5f;

            if (time > 1f)
            {
                time = 1f;

                card[0].sprite = cardWin;

                SetUpdate(OpenCardAnimation);
            }

            var w = (1 - time) * mWidth + time * 0f;

            card[0].rectTransform.SetWidth(w);
        }

        void OpenCardAnimation()
        {
            time -= Time.smoothDeltaTime * 2.5f;

            if (time < 0f)
            {
                time = 0;

                SetUpdate(EndAnim);
            }

            var w = (1 - time) * mWidth + time * 0f;

            card[0].rectTransform.SetWidth(w);
        }

        void CloseShiftCardAnimation()
        {
            time += Time.smoothDeltaTime * 2;

            if (time > 1f)
            {
                time = 1f;

                card[0].sprite = cardWin;

                card[0].rectTransform.SetWidth(0);

                SetUpdate(OpenShiftCardAnimation);
            }

            var w = (1 - time) * mWidth + time * 0f;

            card[0].rectTransform.SetWidth(w);
        }

        void OpenShiftCardAnimation()
        {
            time -= Time.smoothDeltaTime * 2f;

            if (time < 0f)
            {
                time = 0f;

                SetUpdate(ShiftCardAnimation);
            }

            var w = (1 - time) * mWidth + time * 0f;

            card[0].rectTransform.SetWidth(w);
        }

        void ShiftCardAnimation()
        {
            time += Time.smoothDeltaTime * 2f;

            if (time < 0)
                return;

            if (time > 1f)
            {
                time = 0f;

                ShiftCard();

                SetDefault();

                SetUpdate(EndAnim);

                return;
            }

            float t = 1 - time;

            pos.x = 0f;

            pos.y = t * mWidth + time * nWidth;
            card[0].rectTransform.SetWidth(pos.y);

            pos.y = t * mHeight + time * nHeight;
            card[0].rectTransform.SetHeight(pos.y);

            pos.y = t * mPos.y + time * c1Pos.y;
            card[0].rectTransform.localPosition = pos;

            pos.y = c1Pos.y;

            pos.x += t * 0f + t * c1Pos.x;
            card[1].rectTransform.localPosition = pos;

            pos.x += vShift;
            card[2].rectTransform.localPosition = pos;
        
            pos.x += vShift;
            card[3].rectTransform.localPosition = pos;

            pos.x += vShift;
            card[4].rectTransform.localPosition = pos;

            pos.x += vShift;
            card[5].rectTransform.localPosition = pos;

            color.a = t * 1f + time * 0f;

            card[5].color = color;
        }

        void SetUpdate(VoidDelegate _update)
        {
            update = _update;

            enabled = update != null;
        }

        void EndAnim()
        {
            SetUpdate(null);

            if (End != null)
                End();
        }

        void Update()
        {
            if (update != null)
                update();
            else
                enabled = false;
        }
    }
}