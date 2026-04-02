using System;
using UnityEngine;
using CLogic.Core.LifeCycles;
using CLogic.Core.Services;

namespace CLogic.Utils.Service
{
	public static class Services
	{
		static ServiceManager manager = new();
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#if UNITY_EDITOR
		[UnityEditor.InitializeOnLoadMethod]
#endif
		static void SetupManager()
		{
			manager?.Dispose();

			manager = new ServiceManager();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void SetupUpdater()
		{
			GameObject updater = new("Service Updater");
			updater.AddComponent<ServiceUpdater>();
			updater.hideFlags = HideFlags.HideAndDontSave;
		}

		public static T Resolve<T>(LifeCycle lifeCycle) => manager.Resolve<T>(lifeCycle);

		public static void Register<T>(T instance, LifeCycle lifeCycle) where T : IDisposable => manager.Register(instance, lifeCycle);

		public static bool HasService<T>(LifeCycle lifeCycle) where T : IDisposable => manager.HasService<T>(lifeCycle);

		public static bool DeRegister<T>(LifeCycle lifeCycle) => manager.DeRegister<T>(lifeCycle);

		public static void EndLifeCycle(LifeCycle lifeCycle) => manager.EndLifeCycle(lifeCycle);

		public static void CheckLifeCycles() => manager.CheckLifeCycles();

		public static void CheckLifeCycle(LifeCycle lifeCycle) => manager.CheckLifeCycle(lifeCycle);
	}

	public class ServiceUpdater : MonoBehaviour
	{
		private void Update()
		{
			Services.CheckLifeCycles();
		}
	}
}
