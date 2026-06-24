using System;
using System.Collections.Generic;
using CLogic.Utils.Shared;
using UnityEngine;
using UnityEngine.PlayerLoop;
namespace CLogic.Utils
{
    internal class CLogicDelays {}
    public partial class StaticUtils
    {
        private static IDelayCaller delayCaller;
        
        public static DelayHandle ExecuteDelayed(Action action, float delaySeconds) => delayCaller.AddDelay(action, delaySeconds);
        
        public static bool CancelDelay(DelayHandle handle) => delayCaller.RemoveDelay(handle);
        
        #if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration), UnityEditor.InitializeOnLoadMethod]
        #endif
        public static void SetupCallers()
        {
            if (Application.isPlaying)
            {
                if (delayCaller is DelayCallerRuntime)
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
        public DelayHandle AddDelay(Action callback, float delaySeconds);
        
        public bool RemoveDelay(DelayHandle handle);
    }
    
    internal class DelayCallerRuntime : IDelayCaller
    {
        internal DelayScheduler scheduler = new();
        
        public DelayCallerRuntime()
        {
            PlayerLoopInterface.InsertSystemBefore(typeof(DelayCallerRuntime), Update, typeof(Update.ScriptRunBehaviourUpdate));
        }
        
        public DelayHandle AddDelay(Action callback, float delaySeconds)
        {
            var handle = new DelayHandle(callback, Time.realtimeSinceStartupAsDouble + delaySeconds);
            scheduler.AddDelay(handle);
            
            return handle;
        }
        public bool RemoveDelay(DelayHandle handle) => scheduler.RemoveDelay(handle);
        
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
        
        public DelayHandle AddDelay(Action callback, float delaySeconds)
        {
            var handle = new DelayHandle(callback, UnityEditor.EditorApplication.timeSinceStartup + delaySeconds);
            scheduler.AddDelay(handle);
            
            return handle;
        }
        public bool RemoveDelay(DelayHandle handle) => scheduler.RemoveDelay(handle);
        
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
            for (int i = 0; i < pendingActions.Count; i++)
            {
                DelayHandle handle = pendingActions[i];
                if (delay.dueTime <= handle.dueTime)
                {
                    pendingActions.Insert(i, delay);
                    return;
                }
            }
            
            pendingActions.Add(delay);
        }
        
        public bool RemoveDelay(DelayHandle handle) => pendingActions.Remove(handle);
        
        public void CheckDelays(double currentTime)
        {
            foreach (DelayHandle delay in pendingActions.ToArray())
            {
                if (delay.dueTime > currentTime)
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
