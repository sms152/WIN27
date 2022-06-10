using UnityEngine;


namespace nmWins27
{
    public class ManagerLanguage : MonoBehaviour
    {
        [SerializeField]
        LangSetApp m_Paytable, m_Gamble, m_Interface;

        public GLanguage m_GLanguage;

        public void Initialize()
        {
            int indexLang = global::Data.Ini.Instance.Language;

            m_GLanguage = new GLanguage(indexLang);

            var m_LanguagePaytable = new LanguagePaytable(indexLang);

            var t = m_Paytable.txt;
            var rt = m_LanguagePaytable.txt;

            for (int i = 0, length = m_Paytable.txt.Length; i < length; i++)
            {
                t[i].text = rt[i];
            }

            var m_LanguageInterface = new LanguageInterface(indexLang);

             t = m_Interface.txt;
             rt = m_LanguageInterface.txt;

            for (int i = 0, length = m_Interface.txt.Length; i < length; i++)
            {
                t[i].text = rt[i];
            }
        }
    }
}