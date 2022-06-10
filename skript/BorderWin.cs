using UnityEngine;

namespace nmWins27
{
    public class BorderWin : MonoBehaviour
    {
        public void Initialize()
        {
            background = GetComponent<RectTransform>();

            line = GetComponent<UnityEngine.UI.Image>();

            obj = gameObject;
            obj.SetActive(false);
        }

        [SerializeField]
        UnityEngine.UI.Text sumLineWin;

        UnityEngine.UI.Image line;

        RectTransform background;

        readonly
           int[] posX1 = { -417, -225, -42, 152, 336 };

        readonly
             int[] posY1 = { 293, 120, -50, -90 };

        readonly
             int[] posX2 = { -375, -77, 222, 140, 320 };

        readonly 
          int[] posY2 = { 277, 110, -49, -95 };

        GameObject obj;

        public void Show(int column, int row, int value, UnityEngine.Color col)
        {
            if (row < 0)
            {
                Hide();
                return;
            }

            sumLineWin.text = value.ToString();

            background.SetWidth((sumLineWin.text.Length * 25) + 25);

            if (InitialCameraAndGameComponent.pr == 1.25f)
            {
                background.localPosition = new Vector3(posX2[column], posY2[row], -0f);
            }
            else
            {
                background.localPosition = new Vector3(posX1[column], posY1[row], -0f);
            }

            line.color = col;

            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
            }
        }

        public void Hide()
        {
            if (obj.activeInHierarchy)
            {
                obj.SetActive(false);
            }
        }

      
    }
}