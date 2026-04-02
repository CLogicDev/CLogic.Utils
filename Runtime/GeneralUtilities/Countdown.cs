using System;
using UnityEngine;
namespace CLogic.Utils
{
    [Serializable]
    public class Countdown
    {
        [field: SerializeField]
        public double TargetDuration { get; protected set; }
        
        [field: SerializeField]
        public double TimeLeftSeconds { get; set; }
        
        public virtual bool IsActive {get; protected set;}
        
        public virtual bool IsFinished => TimeLeftSeconds <= 0;

        public virtual float PercentageCompletion => Mathf.InverseLerp((float)TargetDuration, 0,  (float)TimeLeftSeconds);
        
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
            TargetDuration = targetTime;
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

        public virtual void Start() => Reset();
        
        public virtual void Reset()
        {
            SetTargetTime(TargetDuration);
        }
        
        public static implicit operator bool(Countdown t)
        {
            return t.IsActive && t.IsFinished;
        }
    }
}
