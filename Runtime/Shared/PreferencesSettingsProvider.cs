#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
namespace CLogic.Utils.Shared
{
    internal static class PreferencesSettingsProvider
    {
        [SettingsProviderGroup]
        public static SettingsProvider[] CreateProviders()
        {
            return GetPreferenceTypes()
                    .Select(CreateProviderForType)
                    .Where(provider => provider != null)
                    .ToArray();
        }

        private static List<Type> GetPreferenceTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => t.IsClass && !t.IsAbstract)
                    .Where(t => t.BaseType?.IsGenericType == true && t.BaseType.GetGenericTypeDefinition() == typeof(PreferencesSo<>))
                    .ToList();
        }

        private static SettingsProvider CreateProviderForType(Type type)
        {
            MethodInfo getOrCreate = type.GetMethod("GetOrCreatePreferences", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            
            if (getOrCreate?.Invoke(null, null) is not PreferencesSo preferences)
                return null;

            return new SettingsProvider(preferences.MenuPath, SettingsScope.User)
            {
                guiHandler = (searchContext) =>
                {
                    SerializedObject so = new(preferences);
                    SerializedProperty property = so.GetIterator();
                    property.NextVisible(true);

                    while (property.NextVisible(false))
                    {
                        EditorGUILayout.PropertyField(property, true);
                    }

                    if (so.ApplyModifiedProperties())
                        preferences.Save();
                }
            };
        }
    }
}
#endif
