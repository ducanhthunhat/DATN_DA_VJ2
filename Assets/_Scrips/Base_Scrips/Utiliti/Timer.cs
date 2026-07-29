using System;
using UnityEngine;

namespace DucAnh.Utilities
{
    public class Timer 
    {
        public event Action OnTimerDone;

        private float startTime;
        private float duration;
        private float targetTime;

        private bool isAtacive;

        public Timer(float duration)
        {
            this.duration = duration;
        }

        public void StartTimer()
        {
            startTime = Time.time;
            targetTime = startTime + duration;
            isAtacive = true;
        }

        public void StopTimer()
        {
            isAtacive = false;
        }

        public void Tick()
        {
            if(!isAtacive) return;
            if(Time.time >= targetTime)
            {
                OnTimerDone?.Invoke();
                StopTimer();
            }
        }
    }
}
