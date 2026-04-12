using CLogic.Utils.UI;
using UnityEngine;
namespace CLogic.Utils.Tests
{
    public class ModalWindowTester : MonoBehaviour
    {
        public string title;
        public string modalQuestion;
        
        [ContextMenu("Test")]
        public void ShowModal()
        {
            ModalWindowBuilder builder = new(modalQuestion);
            
            builder.WithAcceptButton(() => Debug.Log("Accept pressed"), "Yes");
            builder.WithDenyButton(() => Debug.Log("Deny pressed"), null);
            
            builder.SetDestroyOnComplete(true);
            
            builder.WithTitle(title);
            
            builder.Build();
        }
    }
}
