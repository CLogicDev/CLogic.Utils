using CLogic.Systems.Logging;
using UnityEngine;
namespace CLogic.Utils.Tests
{
    public class PreferencesTester : MonoBehaviour
    {
        public bool value;

        public string text;

        [ContextMenu("Set and save")]
        public void SetAndSave()
        {
            var prefs = ExampleUserPreferences.GetOrCreatePreferences();
            prefs.value = value;
            prefs.text = text;
            prefs.Save();
        }
    }
}
