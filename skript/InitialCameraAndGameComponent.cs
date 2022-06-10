using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nmWins27
{
    public class InitialCameraAndGameComponent : MonoBehaviour
    {
        [SerializeField]
        Camera cam;
        [SerializeField]
        Transform gamble, help, interfase, left, right, up, down;

        static public float pr = 0;

        float defaultOrthographicSize;
        // Use this for initialization
        void Awake()
        {

            var w = Screen.currentResolution.width;
            var h = Screen.currentResolution.height;
            pr = (float)w / (float)h;

            if (pr == 1.25f)
            {
                cam.orthographicSize = 3.83f;
                up.localPosition = new Vector3(up.localPosition.x, 3.03f, up.localPosition.z);
                up.localScale = new Vector3(1f, 1.4f);
                down.localPosition = new Vector3(down.localPosition.x, -3.46f, down.localPosition.z);
                down.localScale = new Vector3(1f, 1.3f);
                gamble.localScale = new Vector3( 0.71f, 0.7f);
                help.localScale = new Vector3(0.95f, 0.95f);

            }

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}