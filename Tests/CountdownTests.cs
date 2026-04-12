using System;
using UnityEngine;
namespace CLogic.Utils.Tests
{
    public class CountdownTests : MonoBehaviour
    {
        public ScaledCountdown cooldown = new();
        
        public float target;
        
        private void Start()
        {
            cooldown.OnComplete += () => Debug.Log("Cooldown Complete");
        }
        
        private void Update()
        {
            cooldown.Tick(Time.deltaTime);
            
            if (cooldown.IsActive && !cooldown.IsFinished)
            {
                Debug.Log(cooldown.PercentageCompletion);
            }
        }
        
        [ContextMenu("Set target")]
        public void SetNewTarget()
        {
            cooldown.SetTargetTime(target);
        }
        
        [ContextMenu("Add time")]
        public void AddTime()
        {
            cooldown.AddTargetTime(target);
        }
    }
}
