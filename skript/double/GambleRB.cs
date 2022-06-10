using UnityEngine;

namespace nmWins27
{
    public enum Card : byte
    { Diamonds, Hearts, Spades, Clubs, Nothing }

    public class DataGambleRB
    {
        public bool isWin, isLast;
        public byte card;
        public byte[] cards;
        public int win, toWin, countGame, totalGame, clickColor;

        static DataGambleRB instacne;

        public static DataGambleRB Instacne { get { if (instacne == null) instacne = new DataGambleRB(); return instacne; } }

        public static void Dispose() { instacne = null; }

        DataGambleRB()
        {
            cards = new byte[5];
        }
    }

    public class GambleRB : MonoBehaviour
    {
        public event VoidDelegate Quit, Ready, Click, EndAnim;

        [SerializeField]
        AnimationCard m_AnimationCard;

        [SerializeField]
        UnityEngine.UI.Text gambleAmount, gambleToWin;

        [SerializeField]
        UnityEngine.UI.Button red, black;

        [SerializeField]
        Sprite[] m_Cards;

        [SerializeField]
        GambleRedBlack.SoundManagerGamble m_SoundManager;

        RectTransform t_RectTransform;

        GameObject obj;

        Vector2 pos;

        float start, time;

        DataGambleRB m_DataGambleRB;

        VoidDelegate update;

        void Start() { enabled = false; }

        public void Initialize()
        {
            m_DataGambleRB = DataGambleRB.Instacne;

            m_AnimationCard.Initialize();

            m_AnimationCard.End += AnimCard_End;

            red.onClick.AddListener(ClickRed);
            black.onClick.AddListener(ClickBlack);

            t_RectTransform = GetComponent<RectTransform>();

            start = t_RectTransform.localPosition.y;

            obj = t_RectTransform.parent.gameObject;

            gambleAmount.text = "";

            gambleToWin.text = "";

          //  SetData();
        }

        public void SetData()
        {
            var cards = m_DataGambleRB.cards;

            if (cards != null && cards.Length > 0)
            {
                Sprite[] c = new Sprite[cards.Length];

                for (int i = 0; i < cards.Length; i++)
                {
                    c[i] = m_Cards[cards[i]];
                }

                m_AnimationCard.SetCard(c);
            }

            //gambleToWin.text = m_DataGambleRB.toWin.ToString();
            //gambleAmount.text = m_DataGambleRB.win.ToString();
        }

        //public void SetCredit(int amount, int towin)
        //{
        //    gambleToWin.text = towin.ToString();
        //    gambleAmount.text = amount.ToString();
        //}

        void ClickRed()
        {
            m_DataGambleRB.clickColor = 0;

            DisableButtons();

            if (Click != null)
                Click();
        }

        void ClickBlack()
        {
            m_DataGambleRB.clickColor = 1;

            DisableButtons();

            if (Click != null)
                Click();
        }

        public void Play()
        {
            DisableButtons();

            if (m_DataGambleRB.isWin && !m_DataGambleRB.isLast)
            {
                m_AnimationCard.PlayShift(m_Cards[m_DataGambleRB.card]);

                m_SoundManager.PlayCard();

            }
            else
            {
                m_AnimationCard.PlayOpen(m_Cards[m_DataGambleRB.card]);

                m_SoundManager.PlayCardHalf();
            }
            //    m_SoundManager.GambleWin();
        }

        void AnimCard_End()
        {
            if (!m_DataGambleRB.isWin || m_DataGambleRB.isLast)
            {
                time = -1;
                SetUpdate(WaitLose);
            }
            else
                SetUpdate(WaitReady);
            if (EndAnim != null)
            {
                EndAnim();
            }
        }

        void WaitReady()
        {
            time += Time.deltaTime;

            if (time > 0f)
            {
                SetUpdate(null);

                if (Ready != null)
                    Ready();
            }
        }

        public void PlayReady()
        {
            gambleToWin.text = m_DataGambleRB.toWin.ToString();
            gambleAmount.text = m_DataGambleRB.win.ToString();

            // if (m_DataGambleRB.isWin && !m_DataGambleRB.isLast)
            // {
            m_SoundManager.PlayRotateCard();

            m_AnimationCard.PlayRotate();

            EnableButtons();
            // }
        }

        public void DisableButtons()
        {
            red.interactable = false;
            black.interactable = false;
        }

        void EnableButtons()
        {
            red.interactable = true;
            black.interactable = true;
        }

        public void Hide()
        {
            m_SoundManager.PlayHide();

            DisableButtons();

            time = 0f;

            SetUpdate(HideAnimation);
        }

        public void Show()
        {
            gambleToWin.text = "";
            gambleAmount.text = "";

            m_SoundManager.PlayShow();

            DisableButtons();

            //SetData();

            obj.SetActive(true);

            time = 0f;

            SetUpdate(ShowAnimation);
        }

        void ShowAnimation()
        {
            time += Time.smoothDeltaTime * 4f;

            if (time > 1f)
            {
                time = 1f;

                SetUpdate(EndShow);

                //    m_SoundManager.GambleSuspanse();
            }

            pos.y = (1f - time) * start + time * 0;

            t_RectTransform.localPosition = pos;
        }

        void HideAnimation()
        {
            time += Time.smoothDeltaTime * 4f;

            if (time > 1f)
            {
                time = 1f;

                SetUpdate(EndHide);
            }

            pos.y = (1f - time) * 0 + time * start;

            t_RectTransform.localPosition = pos;
        }

        void EndShow()
        {
            SetUpdate(null);

            if (Ready != null)
                Ready();
        }

        void EndHide()
        {
            SetUpdate(null);

            obj.SetActive(false);

            if (Quit != null)
                Quit();
        }

        void WaitLose()
        {
            time += Time.deltaTime;

            if (time > 0)
                SetUpdate(HideAnimation);
        }

        void SetUpdate(VoidDelegate _update)
        {
            update = _update;

            enabled = update != null;
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