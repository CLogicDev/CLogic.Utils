using CLogic.Core.DataSaving;
using UnityEngine;
namespace CLogic.Utils.DataSaving
{
    public class UPersistentProperty<T> : PersistentProperty<T>
    {

        public UPersistentProperty(string id, string sectionId, T defaultValue = default) : base(GameData.DataSaver, id, sectionId, false, defaultValue)
        {
            GameData.OnSectionsUpdated += HandleSectionsUpdated;

            if(GameData.IsInitialized)
            {
                dataSaver = GameData.DataSaver;
                Init(defaultValue); 
                return;
            }
            
            GameData.OnDataInitialized += () =>
            {
                if(dataSaver != null)
                    return;
                
                dataSaver = GameData.DataSaver;
                Init(defaultValue);
            };
        }

        ~UPersistentProperty()
        {
            GameData.OnSectionsUpdated -= HandleSectionsUpdated;
        }
        
        void HandleSectionsUpdated()
        {
            dataSaver = GameData.DataSaver;
        }
    }
}
