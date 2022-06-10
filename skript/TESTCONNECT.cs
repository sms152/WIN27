using UnityEngine;
using UPC_Work;

namespace nmWins27
{


        public class TESTCONNECT : MonoBehaviour
        {
            public GameConsturct g;


            void Start()
            {
            }


            void F1()
            {

                g.InitServer();
                _dllGame.SetThread();
                gameObject.SetActive(false);

            }
            void OnDestroy()
            {
                _dllGame.DieThread();
            }

            void Update()
            {

                F1();

            }
        }

}