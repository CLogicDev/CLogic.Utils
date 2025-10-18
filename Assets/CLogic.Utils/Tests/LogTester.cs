using System;
using CLogic.Utils.Logger;
using UnityEngine;
namespace CLogic.Utils.Tests
{
    public class LogTester : MonoBehaviour
    {
        public string msg = "something els";
        public LogLevel level;
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
                DoLog();
        }

        void DoLog()
        {
            Logging.Log(msg, level);
            Debug.Log(msg);
        }
    }
}
