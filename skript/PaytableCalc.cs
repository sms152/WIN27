using UnityEngine;

namespace nmWins27
{
    public class PaytableCalc : MonoBehaviour
    {
        [SerializeField]
        BetsSymbol[] symbolBets;

        Data m_Data;

        VoidDelegate _update;

        int bet, line;

        void Start() { }

        public void Initalize()
        {
            m_Data = Data.Instance;

            _update = UPDATE;
        }

        void UPDATE()
        {
            if (m_Data.Bet != bet || m_Data.Lines != line)
            {
                bet = m_Data.Bet;
                line = m_Data.Lines;

                for (int i = (symbolBets.Length - 1); i > -1; --i)
                    symbolBets[i].SetValue(bet, line);
            }
        }

        void Update()
        {
            if (_update != null)
                _update();
        }
    }
}