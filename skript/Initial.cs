using UnityEngine;
using UPC_Work;


namespace nmWins27
{
    public class Initial : MonoBehaviour
    {
        public GameConsturct g;

        float time;
        [SerializeField]
        long num = 9662916217536863;
        
        void Start()
        {
            _dllGame.SetThread();

    
            time = -1f;
        }

        void OnDestroy()
        {
            _dllGame.DieThread();
        }

        void F1()
        {
            var p = _dllGame.GetParams;

            if (p.Status == UPC_Work.DLLConnect.On)
            {
                enabled = false;

                _dllGame.SetSerias(new byte[] { (byte)'J', (byte)'W', 0 });

                g.InitServer();

            }
            else if (p.Status != UPC_Work.DLLConnect.TryOn)
            {
                enabled = false;


                Debug.LogError(p.Status);
            }

        }

        void Update()
        {
            time += Time.deltaTime;

            F1();

        }
    }
}