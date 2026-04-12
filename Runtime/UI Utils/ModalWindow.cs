using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
namespace CLogic.Utils.UI
{
    public class ModalWindow : MonoBehaviour
    {
        [Serializable]
        public struct ModalButton
        {
            public Button button;
            public TextMeshProUGUI textDisplay;
        }
        
        public TextMeshProUGUI titleDisplay;
        
        public TextMeshProUGUI modalQuestionDisplay;
        
        public ModalButton confirmButton;
        
        public ModalButton denyButton;
        
        public GameObject backgroundDim;
        
        public void SetupModal(string modalQuestion, Action confirm, Action deny, bool destroyOnComplete = true, string confirmButtonText = "Yes", string denyButtonText = "No", string title = null)
        {
            modalQuestionDisplay.text = modalQuestion;
            
            if (destroyOnComplete)
            {
                confirm += () => Destroy(gameObject);
                deny += () => Destroy(gameObject);
            }
            
            bool hasConfirmButton = !string.IsNullOrEmpty(confirmButtonText);
            bool hasDenyButton = !string.IsNullOrEmpty(denyButtonText);
            bool hasTitle = !string.IsNullOrEmpty(title);
            
            if (hasConfirmButton)
                confirmButton.textDisplay.text = confirmButtonText;
            else
                confirmButton.button.gameObject.SetActive(false);
            
            if (hasDenyButton)
                denyButton.textDisplay.text = denyButtonText;
            else
                denyButton.button.gameObject.SetActive(false);
            
            if (hasTitle)
                titleDisplay.text = title;
            else
                titleDisplay.gameObject.SetActive(false);
            
            confirmButton.button.onClick.RemoveAllListeners();
            denyButton.button.onClick.RemoveAllListeners();
            
            confirmButton.button.onClick.AddListener(() => confirm?.Invoke());
            denyButton.button.onClick.AddListener(() => deny?.Invoke());
        }
    }
    
    public class ModalWindowBuilder
    {
        public ModalWindow modalWindow;
        
        private string modalTitle;
        
        private string modalQuestion;
        
        private string acceptText;
        private string denyText;
        
        private Action acceptCallback;
        private Action denyCallback;
        
        private bool destroyOnComplete;
        private bool useBackgroundDim;
        
        public ModalWindowBuilder(string modalQuestion, bool useDefaults = true)
        {
            modalWindow = UISettings.GetOrCreateSettings().modalWindow;
            
            this.modalQuestion = modalQuestion;
            
            if (useDefaults)
            {
                acceptText = "Yes";
                denyText = "No";
                
                destroyOnComplete = true;
            }
        }
        
        public ModalWindowBuilder WithTitle(string title)
        {
            modalTitle = title;
            
            return this;
        }
        
        public ModalWindowBuilder WithAcceptButton(Action callback, string buttonText)
        {
            acceptCallback = callback;
            acceptText = buttonText;
            
            return this;
        }
        
        public ModalWindowBuilder WithDenyButton(Action callback, string buttonText)
        {
            denyCallback = callback;
            denyText = buttonText;
            
            return this;
        }
        
        public ModalWindowBuilder SetDestroyOnComplete(bool shouldDestroy)
        {
            destroyOnComplete = shouldDestroy;
            
            return this;
        }
        
        public ModalWindowBuilder SetBackgroundDimEnabled(bool enabled)
        {
            useBackgroundDim = enabled;
            return this;
        }
        
        public ModalWindow Build()
        {
            ModalWindow mw = Object.Instantiate(modalWindow);
            
            mw.SetupModal(modalQuestion, acceptCallback, denyCallback, destroyOnComplete, acceptText, denyText, modalTitle);
            
            mw.backgroundDim.SetActive(useBackgroundDim);
            
            return mw;
        }
    }
}
