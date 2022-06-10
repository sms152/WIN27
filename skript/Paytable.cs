using UnityEngine;

namespace nmWins27
{
    public class Paytable : MonoBehaviour
    {
        [SerializeField]
        GameObject[] list;

        public SoundManager m_SoundManager;

        GameObject obj;

        PaytableCalc text;
        RectTransform img;
        Vector2 pos;
        float start, time;
        byte count;

        public event VoidDelegate Enabled, Disabled, EmptyPage;

        VoidDelegate update;

        public void Initialize()
        {
            img = GetComponent<RectTransform>();
            text = list[0].GetComponent<PaytableCalc>();

            text.Initalize();

            start = img.localPosition.y;

            obj = gameObject;

            for (int i = list.Length - 1; i > -1; --i)
                list[i].SetActive(false);

            enabled = false;
        }

        void Start() { }

        public bool Next()
        {
            m_SoundManager.PlaySound(7, 0);
            if (list.Length == count + 1)
            {

                EmptyPage();

                return false;
            }
            else
            {
                list[count].SetActive(false);

                list[++count].SetActive(true);
            }

            return true;
        }
        public void Next_UI()
        {
            Next();
        }
        public void Prev_UI()
        {
            m_SoundManager.PlaySound(7, 0);
            list[count].SetActive(false);

            if (count > 0)
            {
                count--;
            }
            else
            {
                count = (byte)(list.Length-1);
            }
            list[count].SetActive(true);
        }
        public void Hide()
        {
            m_SoundManager.PlaySound(7, 0);

            time = 0f;
            update = HideAnimation;
            enabled = true;
        }

        public void Show()
        {
            count = 0;
            list[count].SetActive(true);

            time = 0f;
            update = ShowAnimation;

            enabled = true;
        }

         void Update()
        {
            if (update != null)
                update.Invoke();
        }

        void ShowAnimation()
        {
            time += Time.smoothDeltaTime * 4f;

            if (time > 1f)
            {
                time = 1f;

                enabled = false;

                if (Enabled != null)
                    Enabled();
            }

            pos.y = (1f - time) * start + time * 0;

            img.localPosition = pos;
        }

        void HideAnimation()
        {
            time += Time.smoothDeltaTime * 4f;

            if (time > 1f)
            {
                time = 1f;
                enabled = false;

                list[count].SetActive(false);

                if (Disabled != null)
                    Disabled.Invoke();
            }

            pos.y = (1f - time) * 0 + time * start;

            img.localPosition = pos;
        }
    }

    [System.Serializable]
    public struct BetsSymbol
    {
        [SerializeField]
        BetSymbol[] bet;

        [SerializeField]
        bool isLine;

        public void SetValue(int bet, int line)
        {
            if (isLine)
                bet *= line;

            for (int i = this.bet.Length - 1; i > -1; --i)
                this.bet[i].SetValue(ref bet);
        }

        public void Active(bool active)
        {
            for (int i = bet.Length - 1; i > -1; --i)
                bet[i].Active(active);
        }

        [System.Serializable]
        struct BetSymbol
        {
            [SerializeField]
            int multy;

            [SerializeField]
            UnityEngine.UI.Text texst;

            public void SetValue(ref int bet)
            {
                var r = bet * multy;
                texst.text = r > 999999 ? Two(r) : r > 999 ? One(r) : r.ToString();
            }

            public void Active(bool active)
            {
                texst.enabled = active;
            }

            string One(int v)
            {
                string str = v.ToString();
                int startId = str.Length - 3;

                return str.Substring(0, startId) + " " + str.Substring(startId);
            }

            string Two(int v)
            {
                string str = v.ToString();
                int startId1 = str.Length - 6, startId2 = str.Length - 3;

                return str.Substring(0, startId1) + " " + str.Substring(startId1, 3) + " " + str.Substring(startId2);
            }
        }
    }
}