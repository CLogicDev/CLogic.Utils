using CLogic.Utils.Logger;
using UnityEngine;
namespace CLogic.Utils.Tests
{
    public class LogTester : MonoBehaviour
    {
        public string msg = "something els";
        public LogLevel level;
        public ULogger logger = new("Log Tester");

        [ContextMenu("Log")]
        public void LogMsg()
        {
            logger.Log(msg);
        }
    }
}
