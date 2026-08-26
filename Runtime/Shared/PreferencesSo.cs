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
            var instance = CreateInstance<T>();
            #if UNITY_EDITOR

            if (preferencesCache != null || TryLoad(instance.FilePath, out preferencesCache))
            {
                DestroyImmediate(instance);
                return preferencesCache;
            }

            preferencesCache = instance;

            instance.OnPreferencesCreated();
            instance.Save();

            #else
            DestroyImmediate(instance);
            #endif

            return preferencesCache;
        }

        #if UNITY_EDITOR
        private static bool TryLoad(string path, out T result)
        {
            Object[] loaded = UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(path);
            result = loaded != null && loaded.Length > 0 ? loaded[0] as T : null;
            return result != null;
        }
        #endif
    }
}
