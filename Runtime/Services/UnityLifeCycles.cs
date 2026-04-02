using CLogic.Core.LifeCycles;
using UnityEngine.SceneManagement;

namespace CLogic.Utils.Service
{
	public class SceneLifeCycle : LifeCycle<SceneLifeCycle>
	{
		private bool sceneChanged = false;
		public override bool IsAlive => !sceneChanged;
		public SceneLifeCycle()
		{
			SceneManager.sceneUnloaded += SceneUnload;

			return;

			void SceneUnload(Scene _)
			{
				sceneChanged = true;
				SceneManager.sceneUnloaded -= SceneUnload;

				Services.CheckLifeCycle(Instance);
				Instance = new SceneLifeCycle();
			}
		}
	}

	public class UnitySingletonLifeCycle : LifeCycle<UnitySingletonLifeCycle>
	{
		public override bool IsAlive => !stateChanged;
		private bool stateChanged = false;

#if UNITY_EDITOR

		public UnitySingletonLifeCycle()
		{
			UnityEditor.EditorApplication.playModeStateChanged += PlayModeStateChanged;

			return;

			void PlayModeStateChanged(UnityEditor.PlayModeStateChange stateChange)
			{
				if (stateChange == UnityEditor.PlayModeStateChange.EnteredPlayMode)
					return;

				stateChanged = true;

				Services.CheckLifeCycle(Instance);

				Instance = new UnitySingletonLifeCycle();
			}
		}
#endif
	}
}
