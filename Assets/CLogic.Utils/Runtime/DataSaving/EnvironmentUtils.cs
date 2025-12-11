using System;
using UnityEngine;
using System.Collections.Generic;

namespace CLogic.Utils
{
	public static class EnvironmentUtils
	{
		public static bool EnvironmentSet { get; private set; } = false;

		public static event Action OnEnvironmentSet;

		/// <summary>
		/// Adds some useful environment variables for the saving system
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#if UNITY_EDITOR
		[UnityEditor.InitializeOnLoadMethod]
#endif
		public static void InjectEnvironmentVariables()
		{
			Dictionary<string, string> environmentVariables = new()
			{
				{"PERSISTENT_DATA", Application.persistentDataPath},
				{"TEMP_DATA", Application.temporaryCachePath},
				{"STREAMING_ASSETS", Application.streamingAssetsPath},
				{"DATA_PATH", Application.dataPath},
				{"PLATFORM", Application.platform.ToString()},
			};

			foreach (var kvp in environmentVariables)
			{
				if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(kvp.Key)))
				{
					Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
				}
			}
			EnvironmentSet = true;
			OnEnvironmentSet?.Invoke();
		}
	}
}
