using System;
using System.Collections.Generic;
using System.IO;
using CLogic.Utils.Shared;
using CLogic.Utils.UI;
using UnityEngine;
namespace CLogic.Utils
{
    public class UISettings : SettingsSo<UISettings>
    {
        internal const string KEY = "dev.clogic.settings.ui";
        internal const string MODAL_WINDOW_GUID = "7c7eecc439bd4654591705ad3208f05d";
        
        protected override string AssetName { get; set; } = "UISettings.asset";
        protected override string Key { get; set; } = KEY;
        
        public ModalWindow modalWindow;
        
        #if UNITY_EDITOR
        protected override void OnSettingsCreated()
        {
            string path = UnityEditor.AssetDatabase.AssetPathToGUID(MODAL_WINDOW_GUID);
            
            if (path == null)
            {
                Integrations.LogWarning("Modal window prefab could not be found");
                return;
            }
            
            var mw = UnityEditor.AssetDatabase.LoadAssetAtPath<ModalWindow>(path);
            
            modalWindow = mw;
        }
        #endif
    }
}
