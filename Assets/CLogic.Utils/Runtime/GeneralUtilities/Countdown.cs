using System;
using UnityEngine;
namespace CLogic.Utils
{
    [Serializable]
    public class Countdown
    {
        [field: SerializeField]
        public double TimeLeftSeconds { get; set; }

        public double CurrentTargetTime { get; protected set; }

        public virtual bool IsActive {get; protected set;}
        
        public virtual bool IsFinished => TimeLeftSeconds <= 0;

        public virtual float PercentageCompletion => Mathf.InverseLerp((float)CurrentTargetTime, 0,  (float)TimeLeftSeconds);
        
        public Action OnComplete;

        public Countdown() {}

        public Countdown(double targetTime)
        {
            SetTargetTime(targetTime);
        }

        public virtual void SetTargetTime(double targetTime)
        {
            IsActive = true;
            TimeLeftSeconds = targetTime;
            CurrentTargetTime = targetTime;
        }

        public virtual void AddTargetTime(double targetTime)
        {
            SetTargetTime(targetTime + TimeLeftSeconds);
        }

        public virtual void Tick(double timeSinceLastFrameSeconds)
        {
            if(!IsActive)
                return;
            
            if(TimeLeftSeconds <= 0)
                return;
            
            TimeLeftSeconds -= timeSinceLastFrameSeconds;

            if(TimeLeftSeconds <= 0)
            {
                OnComplete?.Invoke();
            }
        }

        public static implicit operator bool(Countdown t)
        {
            return t.IsActive && t.IsFinished;
        }
    }
}
