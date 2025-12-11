using CLogic.Core.DataSaving;

namespace CLogic.Utils.DataSaving
{
	public class UPersistentProperty<T> : PersistentProperty<T>
	{
		private T defaultValue;
		public UPersistentProperty(string id, string sectionId, T defaultValue = default) : base(GameData.DataSaver, id, sectionId, false, defaultValue)
		{
			GameData.OnSectionsUpdated += HandleSectionsUpdated;

			this.defaultValue = defaultValue;
			if (GameData.IsInitialized)
			{
				DataSaver = GameData.DataSaver;
				Init(defaultValue);
				return;
			}

			GameData.OnDataInitialized += () =>
			{
				if (DataSaver != null)
					return;

				DataSaver = GameData.DataSaver;
				Init(defaultValue);
			};
		}

		~UPersistentProperty()
		{
			GameData.OnSectionsUpdated -= HandleSectionsUpdated;
		}

		void HandleSectionsUpdated()
		{
			DataSaver = GameData.DataSaver;
			Init(defaultValue);
		}
	}
}
