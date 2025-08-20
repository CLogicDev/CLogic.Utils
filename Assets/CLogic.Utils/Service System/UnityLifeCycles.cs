using CLogic.Core.LifeCycles;
using UnityEditor.Build.Content;
using UnityEngine.SceneManagement;
namespace CLogic.Utils
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
}
