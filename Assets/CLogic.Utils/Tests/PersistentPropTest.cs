using UnityEngine;
using CLogic.Utils.DataSaving;

namespace CLogic.Utils.Tests
{
	public class PersistentPropTest : MonoBehaviour
	{
		public string value = "SomeValue";

		public UPersistentProperty<string> property = new("TestProp", "default", "DefaultValue");

		[ContextMenu("Get")]
		public void GetValue()
		{
			value = property;
		}

		[ContextMenu("Set")]
		public void SetValue()
		{
			property.Value = value;
		}
	}
}
