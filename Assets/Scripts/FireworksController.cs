using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jackpot.UI
{
    public class FireworksController : MonoBehaviour
    {
        [SerializeField] private List<ParticleSystem> particles;
        [SerializeField] private List<SoundPlayTime> soundPlayTime;


        public void Play()
        {
            particles[0].Play();
            soundPlayTime.ForEach(x =>
            {
                x.playOnAwake = true;
                x.Init();
            });
        }

        public void SetFireworkColor(Color color)
        {
            particles.ForEach(x =>
            {
                var MM = x.main;
                Color prevColor = MM.startColor.color;
                MM.startColor = new Color(color.r, color.g, color.b, prevColor.a);
            });
        }

        public void Stop()
        {
            particles.ForEach(x =>
            {
                x.Stop();
                x.Clear();
            });
            soundPlayTime.ForEach(x =>
            {
                x.playOnAwake = false;
                x.Init();
            });
        }
    }
}
