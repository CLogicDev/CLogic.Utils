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
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        #if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        #endif
        public static void SetupCallers()
        {
            SetupCallersCore();
        }
        
        internal static void SetupCallersCore(bool forceEditor = false)
        {
            IDelayCaller previousCaller = delayCaller;
            if (Application.isPlaying && !forceEditor)
            {
                delayCaller = new DelayCallerRuntime();
                if (previousCaller != null)
                {
                    delayCaller.Scheduler.AddDelays(previousCaller.Scheduler.pendingActions, -UnityEditor.EditorApplication.timeSinceStartup);
                    previousCaller.Dispose();;
                }
            }
            #if UNITY_EDITOR
            else
            {
                delayCaller = new DelayCallerEditor();
                if (previousCaller != null)
                {
                    double delayOffset = UnityEditor.EditorApplication.timeSinceStartup - Time.realtimeSinceStartupAsDouble;
                    delayCaller.Scheduler.AddDelays(previousCaller.Scheduler.pendingActions, delayOffset);
                    previousCaller.Dispose();;
                }
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
    
    internal interface IDelayCaller : IDisposable
    {
        public DelayScheduler Scheduler { get; set; }
        
        public DelayHandle AddDelay(Action callback, float delaySeconds);
        
        public bool RemoveDelay(DelayHandle handle);
    }
    
    internal class DelayCallerRuntime : IDelayCaller
    {
        public DelayScheduler Scheduler { get; set; } = new();
        
        public DelayCallerRuntime()
        {
            PlayerLoopInterface.InsertSystemBefore(typeof(DelayCallerRuntime), Update, typeof(Update.ScriptRunBehaviourUpdate));
            Application.quitting += HandlePlayModeExit;
        }
        private void HandlePlayModeExit()
        {
            if(!Application.isEditor)
                return;
            
            StaticUtils.SetupCallersCore(true);
        }
        
        public DelayHandle AddDelay(Action callback, float delaySeconds)
        {
            var handle = new DelayHandle(callback, Time.realtimeSinceStartupAsDouble + delaySeconds);
            Scheduler.AddDelay(handle);
            
            return handle;
        }
        public bool RemoveDelay(DelayHandle handle) => Scheduler.RemoveDelay(handle);
        
        private void Update()
        {
            Scheduler.CheckDelays(Time.realtimeSinceStartupAsDouble);
        }
        
        public void Dispose()
        {
            PlayerLoopInterface.TryRemoveSystem(typeof(DelayCallerRuntime));
        }
    }
    
    #if UNITY_EDITOR
    internal class DelayCallerEditor : IDelayCaller
    {
        public DelayScheduler Scheduler { get; set; }= new();
        
        public DelayCallerEditor()
        {
            UnityEditor.EditorApplication.update += Update;
        }
        
        public DelayHandle AddDelay(Action callback, float delaySeconds)
        {
            var handle = new DelayHandle(callback, UnityEditor.EditorApplication.timeSinceStartup + delaySeconds);
            Scheduler.AddDelay(handle);
            
            return handle;
        }
        
        public bool RemoveDelay(DelayHandle handle) => Scheduler.RemoveDelay(handle);
        
        private void Update()
        {
            Scheduler.CheckDelays(UnityEditor.EditorApplication.timeSinceStartup);
        }
        
        public void Dispose()
        {
            UnityEditor.EditorApplication.update -= Update;
        }
    }
    #endif
    internal class DelayScheduler
    {
        public List<DelayHandle> pendingActions = new();
        
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
        
        public void AddDelays(List<DelayHandle> handles, double timeOffsets)
        {
            handles.ForEach(handle => handle.dueTime += timeOffsets);
            pendingActions.AddRange(handles);
        }
    }
}
