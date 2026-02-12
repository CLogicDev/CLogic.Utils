using System;
using CLogic.Utils.Shared;
using UnityEngine;
using UnityEngine.PlayerLoop;
namespace CLogic.Utils.Tests
{
    public class PlayerLoopTest : MonoBehaviour
    {
        private void Start()
        {
            PlayerLoopInterface.InsertSystemBefore(typeof(PlayerLoopTest), UpdateFunction, typeof(Update.ScriptRunBehaviourUpdate));
        }

        void UpdateFunction()
        {
            Debug.Log("frame "  + Time.frameCount);
        }
    }
}
