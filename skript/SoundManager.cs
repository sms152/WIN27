using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nmWins27
{
    public class SoundManager : MonoBehaviour
    {

        [SerializeField]
        AudioClip[] masSound;

        AudioSource[] audio;
        Data m_Data;

        // Use this for initialization
        public void Initialize()
        {
            m_Data = Data.Instance;

            audio = new AudioSource[6];

            for (byte i = 0; i < audio.Length; ++i)
                audio[i] = gameObject.AddComponent<AudioSource>();


        }
        public bool IsPlaying(int channel)
        {
            return audio[channel].isPlaying;
        }

        public void PlaySound(int number, int channel, bool loop = false)
        {
            audio[channel].loop = loop;
            audio[channel].clip = masSound[number];
            audio[channel].Play();
        }
        public void StopSound(int all=-1)
        {
            if (all == -1)
            {
                for (int i = 0; i < audio.Length; i++)
                    audio[i].Stop();
            }
            else
            {
                audio[all].Stop();
            }
        }

        public void ReelStop(int index)
        {
            PlaySound(m_Data.reelStopSound[index], 0);

            if ((m_Data.WaitMoln > 0) && (m_Data.WaitMoln == index))
            {
                PlaySound(39, 3);
            }
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}