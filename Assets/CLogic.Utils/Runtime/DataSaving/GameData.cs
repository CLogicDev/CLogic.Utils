using System;
using System.Collections.Generic;
using System.Linq;
using CLogic.Core.DataSaving;
using CLogic.Utils.DataSaving.Sections;
using UnityEngine;
using Object = UnityEngine.Object;
namespace CLogic.Utils.DataSaving
{
    public static class GameData
    {
        internal static DataSaver DataSaver { get; private set; }
        
        public static List<BaseDataSectionSo> dataSections = new();
        
        private static Dictionary<string, BaseDataSectionSo> sectionLookup = new();
        
        public static HashSet<PersistentDataContainer> dirtyContainers  = new();
        private static HashSet<string> pausedDirty = new();
        
        public static bool IsInitialized => DataSaver != null;

        private static bool canUpdateSections = true;

        public static bool AutoSaveDirtySections
        {
            get => isAutoSaving;
            set
            {
                isAutoSaving = value;
                
                SetupAutoSave();
            }
        }
        private static bool isAutoSaving = false;
        private static RuntimeAutoSaver autoSaver;

        public static TimeSpan autoSaveInterval = TimeSpan.FromSeconds(30);
        
        public static Action OnSectionsUpdated;
        public static Action OnDataInitialized;

        private static void UpdateLookup()
        {
            sectionLookup.Clear();
            foreach (BaseDataSectionSo section in dataSections.Where(s => s != null))
            {
                sectionLookup.Add(section.GetSectionId(),  section);
            }
        }

        private static void SetupAutoSave()
        {
            #if UNITY_EDITOR
            EditorAutoSaver.SetActive(!Application.isPlaying && isAutoSaving); //No editor save while playing
            #endif

            if(Application.isPlaying)
            {
                switch (isAutoSaving)
                {
                    case false when autoSaver != null:
                        Object.Destroy(autoSaver.gameObject);
                    #if UNITY_EDITOR
                        UnityEditor.EditorApplication.playModeStateChanged -= PlayModeChange;
                    #endif
                        return;
                    case true when autoSaver == null:
                    {
                        SetupRuntimeAutoSaver();
                        break;
                    }
                }
            }
            #if UNITY_EDITOR
            if(!Application.isPlaying)
            {
                UnityEditor.EditorApplication.playModeStateChanged -= PlayModeChange; //Prevents duplicates
                UnityEditor.EditorApplication.playModeStateChanged += PlayModeChange;
            }

            void PlayModeChange(UnityEditor.PlayModeStateChange state)
            {
                if(state != UnityEditor.PlayModeStateChange.EnteredPlayMode)
                    return;

                if(autoSaver == null)
                    SetupRuntimeAutoSaver();
                UnityEditor.EditorApplication.playModeStateChanged -= PlayModeChange;
            }
            #endif
            
            void SetupRuntimeAutoSaver()
            {
                GameObject saver = new("Game Data Auto Saver");
                autoSaver = saver.AddComponent<RuntimeAutoSaver>();
                saver.hideFlags = HideFlags.HideAndDontSave;
            }
        }
        
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        #if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        #endif
        public static void UpdateSections()
        {
            if(!canUpdateSections)
                return;
            if(!EnviromentUtils.EnvironmentSet)
            {
                EnviromentUtils.OnEnvironmentSet += UpdateSectionsInternal;
                return;
            }
            UpdateSectionsInternal();
        }

        static void UpdateSectionsInternal()
        {
            bool isFirstInit = DataSaver == null;
            
            canUpdateSections = false; // Prevents updating section twice due to OnValidate
            DataSectionSettings settings = DataSectionSettings.GetOrCreateSettings();
            canUpdateSections = true;
            
            //Ensure default slots from project settings
            if(Slotter.CurrentSlotId == Slotter.defaultSlotId || Environment.GetEnvironmentVariable(Slotter.ENVIRONMENT_VARIABLE_NAME) == null)
                Slotter.CurrentSlotId = settings.defaultSlot;
            
            
            dataSections = settings.dataSections;
            
            List<IDataSection> saveSections = new (settings.dataSections.Where(s => s != null));
            DataSaver = new DataSaver(saveSections);

            UpdateLookup();
            AutoSaveDirtySections = true;
            autoSaveInterval = TimeSpan.FromSeconds(5);
            
            if(isFirstInit)
                OnDataInitialized?.Invoke();
            
            OnSectionsUpdated?.Invoke();
            EnviromentUtils.OnEnvironmentSet -= UpdateSectionsInternal;
            
        }

        public static void SaveDirtyToDisk()
        {
            foreach (PersistentDataContainer dataContainer in dirtyContainers)
                dataContainer.SaveToDisk();
            
            dirtyContainers.Clear();
        }

        public static void PauseDirty(string sectionId)
        {
            dirtyContainers.Remove(GetDataContainer(sectionId));
            
            pausedDirty.Add(sectionId);
        }

        public static void ResumeDirty(string sectionId)
        {
            pausedDirty.Remove(sectionId);
        }

        public static void SaveSection(string sectionId, bool removeAsDirty = true) => SaveSection(GetDataContainer(sectionId), removeAsDirty);
        
        public static void SaveSection(PersistentDataContainer dataContainer, bool removeAsDirty = true)
        {
            dataContainer.SaveToDisk();
            
            if(removeAsDirty)
                dirtyContainers.Remove(dataContainer);
        }
        
        public static void SaveSectionIfDirty(string sectionId, bool removeAsDirty = true) => SaveSectionIfDirty(GetDataContainer(sectionId), removeAsDirty);
        
        public static void SaveSectionIfDirty(PersistentDataContainer dataContainer, bool removeAsDirty = true)
        {
            if(!dirtyContainers.Contains(dataContainer))
                return;
            
            dataContainer.SaveToDisk();
            
            if(removeAsDirty)
                dirtyContainers.Remove(dataContainer);
        }

        public static bool MakeDirty(string sectionId, bool forced = false)
        {
            PersistentDataContainer dataContainer = GetDataContainer(sectionId);
            if(forced)
            {
                dirtyContainers.Add(dataContainer);
                return true;
            }
            
            BaseDataSectionSo section = sectionLookup[sectionId];
            if(section.savingMode != SavingMode.Dirty || pausedDirty.Contains(section.GetSectionId()))
                return false;
            
            dirtyContainers.Add(dataContainer);
            return true;

        }

        private static bool HandleDirtyCheck(string sectionId, bool? shouldUpdate)
        {
            BaseDataSectionSo section = sectionLookup[sectionId];

            shouldUpdate ??= section.savingMode switch
            {
                SavingMode.Manual => false,
                SavingMode.Instant => true,
                SavingMode.Dirty => false,
                _ => throw new ArgumentOutOfRangeException()
            };

            MakeDirty(sectionId);
            
            return shouldUpdate.Value;
        }
        
        #region Data Saver Access

        public static void UpdateAllFromDisk() => DataSaver.UpdateAllFromDisk();
        
        public static void UpdateAllToDisk() => DataSaver.UpdateAllToDisk();
        
        public static PersistentDataContainer GetDataContainer(string sectionID) => DataSaver.GetDataContainer(sectionID);

        public static void SetData(string id, object data, string sectionId, bool? shouldUpdate = null)
        {
            bool updateData = HandleDirtyCheck(sectionId, shouldUpdate);
            GetDataContainer(sectionId).SetData(id, data, updateData);
        }
        
        public static T GetData<T>(string id, string sectionId, T defaultValue = default) => DataSaver.GetData<T>(id, sectionId, defaultValue);
        
        public static bool TryGetData<T>(string id, out T data, string sectionId, T defaultValue = default) => DataSaver.TryGetData(id, out data, sectionId, defaultValue);
        
        public static bool HasData(string id,  string sectionId) => DataSaver.HasData(id, sectionId);

        public static void AddSection(BaseDataSectionSo section)
        {
            DataSaver.AddSection(section);
            sectionLookup.Add(section.GetSectionId(), section);
        }
        public static void AddSection(IEnumerable<BaseDataSectionSo> sections)
        {
            foreach (BaseDataSectionSo section in sections)
            {
                AddSection(section);
            }
        }

        public static void ClearSectionData(string sectionId, bool? shouldUpdate = null)
        {
            bool updateData = HandleDirtyCheck(sectionId, shouldUpdate);
            GetDataContainer(sectionId).ClearData(updateData);
        }
        
        
        
        #endregion
    }
    
    public class RuntimeAutoSaver : MonoBehaviour
    {
        private double lastSaveTime;

        private void Start()
        {
            lastSaveTime = Time.realtimeSinceStartupAsDouble;
        }

        private void OnApplicationQuit()
        {
            SaveDirty();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if(pauseStatus)
                SaveDirty();
        }

        private void SaveDirty(double? now = null)
        {
            GameData.SaveDirtyToDisk();
            lastSaveTime = now ?? Time.realtimeSinceStartupAsDouble;
        }

        private void Update()
        {
            double now = Time.realtimeSinceStartupAsDouble;
            if(now - lastSaveTime >= GameData.autoSaveInterval.TotalSeconds)
                SaveDirty(now);
        }
    }
#if UNITY_EDITOR
    internal static class EditorAutoSaver
    {
        private static double lastSaveTime;

        private static bool isActive;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        [UnityEditor.InitializeOnLoadMethod]
        public static void RegisterExit()
        {
            UnityEditor.EditorApplication.playModeStateChanged += change =>
            {
                if(change == UnityEditor.PlayModeStateChange.EnteredEditMode)
                    SetActive(GameData.AutoSaveDirtySections); //
            };
        }
        
        internal static void SetActive(bool active)
        {
            switch (active)
            {
                case true when !isActive:
                    lastSaveTime = UnityEditor.EditorApplication.timeSinceStartup;
                
                    UnityEditor.EditorApplication.update += Update;
                    UnityEditor.EditorApplication.quitting += SaveDirtyWrapper;
                    UnityEditor.EditorApplication.focusChanged += FocusChange;
                    isActive = true;
                    break;
                case false:
                    UnityEditor.EditorApplication.update -= Update;
                    UnityEditor.EditorApplication.quitting -= SaveDirtyWrapper;
                    UnityEditor.EditorApplication.focusChanged -= FocusChange;
                    isActive = false;
                    break;
            }
            
            return;//

            void SaveDirtyWrapper()
            {
                SaveDirty();
            }

            void FocusChange(bool focused)
            {
                if(!focused)
                    SaveDirty();
            }
        }


        private static void SaveDirty(double? now = null)
        {
            GameData.SaveDirtyToDisk();
            lastSaveTime = now ?? UnityEditor.EditorApplication.timeSinceStartup;
        }

        public static void Update()
        {
            double now = UnityEditor.EditorApplication.timeSinceStartup;
            if(now - lastSaveTime >= GameData.autoSaveInterval.TotalSeconds)
                SaveDirty(now);
        }
    }
#endif
}


