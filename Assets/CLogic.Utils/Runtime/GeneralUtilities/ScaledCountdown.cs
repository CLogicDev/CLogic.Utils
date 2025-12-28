using System;
using UnityEngine;
namespace CLogic.Utils
{
    [Serializable]
    public class ScaledCountdown : Countdown
    {
        [field: SerializeField]
        public double MinimumTime { get; set; }
        
        [field: SerializeField]
        public double MaximumTime { get; set; }

        public override bool IsFinished => TimeLeftSeconds <= MinimumTime;
        
        public override float PercentageCompletion => Mathf.InverseLerp((float)CurrentTargetTime, (float)MinimumTime,  (float)TimeLeftSeconds);

        public override void Tick(double timeSinceLastFrameSeconds)
        {
            if(!IsActive)
                return;
            
            if(TimeLeftSeconds <= MinimumTime)
                return;
            
            TimeLeftSeconds -= timeSinceLastFrameSeconds;

            TimeLeftSeconds = Math.Clamp(TimeLeftSeconds, MinimumTime, MaximumTime);
            
            if(TimeLeftSeconds <= MinimumTime)
            {
                OnComplete?.Invoke();
            }
        }

        public override void SetTargetTime(double targetTime)
        {
            Math.Clamp(targetTime, MinimumTime, MaximumTime);
            
            IsActive = true;
            TimeLeftSeconds = targetTime;
            CurrentTargetTime = targetTime;
        }
    }
}
