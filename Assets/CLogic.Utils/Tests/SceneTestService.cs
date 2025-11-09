using System;
using UnityEngine;
using CLogic.Utils.ServiceSystem;

namespace CLogic.Utils.Tests
{
	public class SceneTestService : MonoBehaviour, IDisposable
	{
		private void Awake()
		{
			if (Services.HasService<SceneTestService>(SceneLifeCycle.Instance))
			{
				Dispose();
				return;
			}

			Services.Register(this, SceneLifeCycle.Instance);
		}
		public void Dispose()
		{
			Debug.Log("Disposing");
			Destroy(this);
		}
	}
}
