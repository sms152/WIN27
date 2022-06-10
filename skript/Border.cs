using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace nmWins27
{
    public class Border : MonoBehaviour
    {
        LineRenderer rLine;
        Transform _this;

        float[] ofsY = new float[4] { 0, -1.75f, -3.47f, -3.93f };

        public void Initialize()
        {
            _this = transform;
            rLine = GetComponent<LineRenderer>();

            rLine.enabled = false;
        }

        public void ShowBorder(Color cc)
        {
            _this.localPosition = new Vector3(_this.localPosition.x, _this.localPosition.y, -3.21f);
            rLine.startColor = cc;
            rLine.endColor = cc;
            rLine.enabled = true;
        }

        public void SetColorBorder(Color cc)
        {
            rLine.startColor = cc;
            rLine.endColor = cc;
           
        }

        public void HideBorder()
        {
            rLine.enabled = false;
        }
    }
}