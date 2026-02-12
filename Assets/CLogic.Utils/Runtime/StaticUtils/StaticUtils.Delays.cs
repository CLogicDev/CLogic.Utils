using System;
using System.Collections.Generic;
using CLogic.Utils.Shared;
using UnityEngine;
using UnityEngine.PlayerLoop;
namespace CLogic.Utils
{
    class CLogicDelays {}
    public partial class StaticUtils
    {
        private static IDelayCaller delayCaller;
        
        public static void ExecuteDelayed(Action action, float delaySeconds)
        {
            delayCaller.AddDelay(action, delaySeconds);
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        #if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        #endif
        public static void SetupCallers()
        {
            if(Application.isPlaying)
            {
                if(delayCaller is DelayCallerRuntime)
                    return;

                delayCaller = new DelayCallerRuntime();
            }
            #if UNITY_EDITOR
            else
            {
                delayCaller = new DelayCallerEditor();
            }
            #endif
        }
    }

    public class DelayHandle
    {
        public Action callback;
        public double dueTime;
        
        public DelayHandle(Action callback, double dueTime)
        {
            this.callback = callback;
            this.dueTime = dueTime;
        }
    }

    internal interface IDelayCaller
    {
        public void AddDelay(Action callback, float delaySeconds);
    }
    
    internal class DelayCallerRuntime : IDelayCaller
    {
        internal DelayScheduler scheduler = new();

        public DelayCallerRuntime()
        {
            PlayerLoopInterface.InsertSystemBefore(typeof(DelayCallerRuntime), Update, typeof(Update.ScriptRunBehaviourUpdate));
        }
        
        public void AddDelay(Action callback, float delaySeconds)
        {
            scheduler.AddDelay(new DelayHandle(callback, Time.realtimeSinceStartupAsDouble + delaySeconds));
        }
        
        private void Update()
        {
            scheduler.CheckDelays(Time.realtimeSinceStartupAsDouble);
        }
    }
    
    #if UNITY_EDITOR
    internal class DelayCallerEditor : IDelayCaller
    {
        private DelayScheduler scheduler = new();

        public DelayCallerEditor()
        {
            UnityEditor.EditorApplication.update += Update;
        }
        
        public void AddDelay(Action callback, float delaySeconds)
        {
            scheduler.AddDelay(new DelayHandle(callback, UnityEditor.EditorApplication.timeSinceStartup + delaySeconds));
        }
        
        private void Update()
        {
            scheduler.CheckDelays(UnityEditor.EditorApplication.timeSinceStartup);
        }
        
        ~DelayCallerEditor()
        {
            scheduler.CallAllDelays();
            UnityEditor.EditorApplication.update -= Update;
        }
    }
    #endif
    internal class DelayScheduler
    {
        private List<DelayHandle> pendingActions = new();
        
        public void AddDelay(DelayHandle delay)
        {
            for(int i = 0; i < pendingActions.Count; i++)
            {
                DelayHandle handle = pendingActions[i];
                if(delay.dueTime <= handle.dueTime)
                {
                    pendingActions.Insert(i, delay);
                    return;
                }
            }

            pendingActions.Add(delay);
        }

        public void CheckDelays(double currentTime)
        {
            foreach (DelayHandle delay in pendingActions.ToArray())
            {
                if(delay.dueTime > currentTime)
                    return;

                delay.callback?.Invoke();
                pendingActions.Remove(delay);
            }
        }

        public void CallAllDelays()
        {
            foreach (DelayHandle delay in pendingActions)
            {
                delay.callback?.Invoke();
            }
        }
    }
}
