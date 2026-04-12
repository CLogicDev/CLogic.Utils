using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace CLogic.Utils
{
    public static partial class StaticUtils
    {
        /// <summary>
        /// Tries to get a component from the parent of the object
        /// </summary>
        /// <typeparam name="T">The type of the component</typeparam>
        /// <param name="component">Target Component</param>
        /// <param name="parentComponent">The desired component</param>
        /// <returns>True if the component was found, False otherwise</returns>
        public static bool TryGetComponentInParent<T>(this Component component, out T parentComponent) where T : Component
        {
            foreach (Transform parent in GetParents(component.transform))
            {
                if (parent.TryGetComponent(out T foundComponent))
                {
                    parentComponent = foundComponent;
                    return true;
                }
            }
            
            parentComponent = null;
            return false;
        }
        
        /// <summary>
        /// Tries to get a component from the children of the object
        /// </summary>
        /// <typeparam name="T">The type of the component</typeparam>
        /// <param name="component">Target Component</param>
        /// <param name="childComponent">The desired component</param>
        /// <returns>True if the component was found, False otherwise</returns>
        public static bool TryGetComponentInChildren<T>(this Component component, out T childComponent) where T : Component
        {
            foreach (Transform child in component.transform)
            {
                if (child.TryGetComponent(out T foundComponent))
                {
                    childComponent = foundComponent;
                    return true;
                }
            }
            
            childComponent = null;
            return false;
        }
        
        /// <summary>
        /// Gets all the parents of a transform
        /// </summary>
        /// <param name="child">The child whom to query for its parents</param>
        /// <returns>An list of the parents of the child from closet to furthest</returns>
        public static List<Transform> GetParents(this Transform child)
        {
            List<Transform> parents = new();
            Transform parent = child.parent;
            
            while (parent != null)
            {
                parents.Add(parent);
                parent = parent.parent;
            }
            
            return parents;
        }
        
        /// <summary>
        /// Execute logic once
        /// </summary>
        /// <param name="logic">The logic to execute</param>
        /// <param name="wasExecuted">Set the referenced bool to false to execute again</param>
        public static void DoOnce(Action logic, ref bool wasExecuted)
        {
            if (!wasExecuted)
            {
                wasExecuted = true;
                logic?.Invoke();
            }
        }
        
        public static T TryUntil<T>(Func<int, T> generator, Predicate<T> condition, int start = 0, int maxTries = 1000)
        {
            int currentTries = start;
            bool passed = false;
            
            T generated = default;
            
            maxTries = start + maxTries;
            while (!passed)
            {
                if (maxTries == currentTries)
                    return default;
                
                generated = generator(currentTries);
                passed = condition(generated);
                currentTries++;
            }
            
            return generated;
        }
        
        /// <summary>
        /// Moves a UI element to the mouse position
        /// </summary>
        /// <param name="transform">The RectTransform to move</param>
        /// <param name="canvas">The canvas the RectTransform is part of</param>
        /// <param name="mousePosition">The position of the mouse</param>
        /// <param name="offset">Offset of the final position</param>
        public static void MoveUIToMouse(Transform transform, Canvas canvas, Vector2 mousePosition, Vector2 offset)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mousePosition, canvas.worldCamera, out Vector2 movePos);
            
            transform.position = canvas.transform.TransformPoint(movePos + offset);
        }
        
        public static FileStream CreateIncrementalFile(string directoryPath, string fileName, Func<int, string> generator = null, int start = 1, int maxTries = 1000)
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException(directoryPath);
            
            if (!File.Exists(Path.Combine(directoryPath, fileName)))
                return File.Create(Path.Combine(directoryPath, fileName));
            
            generator ??= GetFileName;
            
            string finalFileName = TryUntil(generator, s => !File.Exists(Path.Combine(directoryPath, s)), start, maxTries);
            
            if (string.IsNullOrEmpty(finalFileName))
                throw new Exception("Maximum tries exceeded");
            
            return File.Create(Path.Combine(directoryPath, finalFileName));
            
            string GetFileName(int increment)
            {
                string name = Path.GetFileNameWithoutExtension(fileName);
                string extension = Path.GetExtension(fileName);
                
                return $"{name} {increment}{extension}";
            }
        }
        
        public static FileStream CreateIncrementalFile(string fullPath, Func<int, string> generator = null, int start = 1, int maxTries = 1000) => CreateIncrementalFile(Path.GetDirectoryName(fullPath), Path.GetFileName(fullPath), generator, start, maxTries);
    }
}
