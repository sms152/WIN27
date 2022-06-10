using NewGame2;
using UnityEngine;

namespace nmWins27
{
    public delegate void VoidDelegate();

    public class RSymbol : MonoBehaviour
    {
        Transform _transform;

        SpriteRenderer sprt;

        NewGame2.SymbolList m_SymbolList;

        public Symbol _symbol;

        public event VoidDelegate PreEnd;

        float time = 0f;

        int indexSprite, length;

        bool isPlay;
        bool tudi_sudi = false;
        SAnimation _play;
        int _indexSymbol, _indexBackSymbol = -1;
        public Vector3 Position { get { return _transform.localPosition; } set { _transform.localPosition = value; } }

        public Sprite Image
        {
            get { return sprt.sprite; }
            set
            {
                sprt.sprite = value;
            }
        }

        public Symbol Symbol { get { return _symbol; } }

        public void Initialize()
        {
            m_SymbolList = NewGame2.SymbolList.Instance;

            _transform = transform;

            sprt = GetComponent<SpriteRenderer>();
            _indexSymbol = 0;
        }

        void Start()
        {
            enabled = false;
        }
        public int GetIndexSymbol()
        {
            return _indexSymbol;
        }
        public void SetSymbol(int symbol, float zz=0)
        {
            _indexSymbol = symbol;
            _symbol = m_SymbolList.GetSymbol(symbol);

            if (_symbol != null)
                sprt.sprite = _symbol.Head;
            if (zz < 0)
            {
                _transform.localPosition = new Vector3(_transform.localPosition.x, _transform.localPosition.y, zz);
            }
        }
        int _first_index=0;
        public void Play(int ind, int first_index=0, bool flag = true)
        {
            tudi_sudi = flag;
            if (!_symbol || !_symbol[ind])
                return;

            _play = _symbol[ind];
            _first_index = first_index;

            indexSprite = 0;
            sprt.color = Color.white;
            length = _play.Length;
            time = -_play.Fps;
            enabled = true;
            isForward = false;
            isPlay = true;
        }
        public void PlayNext()
        {
            Play(1,0,false);
        }
        public bool IsPlayed()
        {
            return isPlay;
        }
        public void Pause(bool pause)
        {
            if (!isPlay && !pause)
                return;

            enabled = !pause;
        }

        public void Normal()
        {
            if (!_symbol)
                return;

            sprt.sprite = _symbol.Head;
            sprt.color = Color.white;
            enabled = false;
            isPlay = false;
        }

        public void Grey()
        {
            sprt.color = Color.grey;
            enabled = false;
        }

        public void Forward(float zz=-3f)
        {
            _transform.localPosition = new Vector3(0f, _transform.localPosition.y, zz);
        }
        public void AddForward(float zz)
        {
            _transform.localPosition = new Vector3(0f, _transform.localPosition.y, _transform.localPosition.z+zz);
        }
        public void Back(float zz=-1f)
        {
            _transform.localPosition = new Vector3(0f, _transform.localPosition.y, zz);
        }

        bool isForward;

        private void Update()
        {
            time += Time.smoothDeltaTime;

            if (time < 0f)
                return;

            sprt.sprite = isForward ? _play[indexSprite--] : _play[indexSprite++];

            time -= _play.Fps;

            if (indexSprite >= length)
            {
                if (tudi_sudi)
                {
                    indexSprite = length - 1;

                    isForward = !isForward;
                }
                else
                {
                    indexSprite = _first_index;
                }
                if (!_play.IsLoop)
                    enabled = false;

                if (PreEnd != null)
                {
                    PreEnd.Invoke();
                    PreEnd = null;
                }
            }
            else if (indexSprite <= 0)
            {
                if (tudi_sudi)
                {
                    isForward = !isForward;
                    indexSprite = 0;
                }
                else
                {
                    indexSprite = length - 1;
                }
            }

           
        }
    }
}