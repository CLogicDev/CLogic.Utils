using UnityEngine;
namespace CLogic.Utils.Tests
{
    public class DelayTest : MonoBehaviour
    {
        public float delay;
        
        [ContextMenu("AddDelay")]
        public void CallDelay()
        {
            float c = delay;
            
            StaticUtils.ExecuteDelayed(() => Debug.Log("Delayed by " + c), delay);
            Debug.Log($"Delay: {delay}");
        }
    }
}
