using System.IO;
using UnityEngine;
namespace CLogic.Utils.Shared
{
    public abstract class PreferencesSo : ScriptableObject
    {
        private const string PREFERENCES_FOLDER = "UserSettings";

        protected abstract string FileName { get; }

        public virtual string MenuPath => "Preferences/CLogic/" + GetType().Name;

        protected string FilePath => Path.Combine(PREFERENCES_FOLDER, FileName);

        public void Save()
        {
            #if UNITY_EDITOR
            Directory.CreateDirectory(PREFERENCES_FOLDER);
            UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] { this }, FilePath, true);
            #endif
        }

        protected virtual void OnPreferencesCreated() {}
    }

    public abstract class PreferencesSo<T> : PreferencesSo where T : PreferencesSo<T>
    {
        protected static T preferencesCache;

        public static T GetOrCreatePreferences()
        {
            #if UNITY_EDITOR
            if (preferencesCache != null)
                return preferencesCache;

            var probe = CreateInstance<T>();
            Object[] loaded = UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(probe.FilePath);

            if (loaded != null && loaded.Length > 0 && loaded[0] is T existing)
            {
                DestroyImmediate(probe);
                preferencesCache = existing;
                return preferencesCache;
            }

            preferencesCache = probe;
            preferencesCache.OnPreferencesCreated();
            preferencesCache.Save();

            return preferencesCache;
            #else
            return preferencesCache;
            #endif
        }
    }
}
