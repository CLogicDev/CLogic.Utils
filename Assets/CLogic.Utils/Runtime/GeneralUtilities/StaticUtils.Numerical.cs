using System;
using UnityEngine;

namespace CLogic.Utils
{
	public static partial class StaticUtils
	{
		/// <summary>
		/// Checks if the value is in between two values
		/// </summary>
		/// <param name="current">The current value</param>
		/// <param name="firstValue">The first value</param>
		/// <param name="secondValue">The second value</param>
		/// <returns>True if the int is more then the first value and less then the second value</returns>
		public static bool IsInBetween(this int current, int firstValue, int secondValue) => current > firstValue && current < secondValue;

		/// <summary>
		/// Checks if the value is one of the two or in between the two values
		/// </summary>
		/// <param name="current">The current value</param>
		/// <param name="firstValue">The first value</param>
		/// <param name="secondValue">The second value</param>
		/// <returns>True if the value is more then or equal the first value and less then or equal the second value</returns>
		public static bool IsInBetweenEqual(this int current, int firstValue, int secondValue) => current >= firstValue && current <= secondValue;

		/// <summary>
		/// Checks if the value is in between two values
		/// </summary>
		/// <param name="current">The current value</param>
		/// <param name="firstValue">The first value</param>
		/// <param name="secondValue">The second value</param>
		/// <returns>True if the value is more then the first value and less then the second value</returns>
		public static bool IsInBetween(this float current, float firstValue, float secondValue) => current > firstValue && current < secondValue;

		/// <summary>
		/// Checks if the value is one of the two or in between the two values
		/// </summary>
		/// <param name="current">The current value</param>
		/// <param name="firstValue">The first value</param>
		/// <param name="secondValue">The second value</param>
		/// <returns>True if the value is more then or equal the first value and less then or equal the second value</returns>
		public static bool IsInBetweenEqual(this float current, float firstValue, float secondValue) => current >= firstValue && current <= secondValue;

		/// <summary>
		/// Checks if the value is equal or close enough to a value. Use this to check for equality on floats modified via deltaTime.
		/// </summary>
		/// <param name="current">The current value</param>
		/// <param name="value">The value to check</param>
		/// <param name="frameTime">The current frame time</param>
		/// <returns>True if the value is equal or close enough</returns>
		public static bool IsFrameEqual(this float current, float value, float frameTime)
		{
			var prevFrameValue = current - frameTime;
			var nextFrameValue = current + frameTime;

			return IsInBetweenEqual(value, prevFrameValue, nextFrameValue);
		}

		/// <summary>
		/// Gets the closest number to the target from a collection of values
		/// </summary>
		/// <param name="collection">The collection to get the values from</param>
		/// <param name="target">The target to find the closest number to</param>
		/// <returns>The closest number to the target</returns>
		public static int GetClosestNumber(this int[] collection, int target)
		{
			float[] floatArray = new float[collection.Length];
			Array.Copy(collection, floatArray, collection.Length);

			return Mathf.RoundToInt(GetClosestNumber(floatArray, target));
		}

		/// <summary>
		/// Gets the closest number to the target from a collection of values
		/// </summary>
		/// <param name="collection">The collection to get the values from</param>
		/// <param name="target">The target to find the closest number to</param>
		/// <returns>The closest number to the target</returns>
		public static float GetClosestNumber(this float[] collection, float target)
		{
			float closest = collection[0];
			float closestDifference = Mathf.Abs(closest - target);

			foreach (var number in collection)
			{
				float currentDifference = Mathf.Abs(number - target);
				if (currentDifference < closestDifference)
				{
					closest = number;
					closestDifference = currentDifference;
				}
			}

			return closest;
		}

		/// <summary>
		/// Adds a value to all vector axis
		/// </summary>
		/// <param name="vector">The vector to add the value to</param>
		/// <param name="addend">The value to add</param>
		/// <returns>A vector with the value added to all axis</returns>
		public static Vector2 Add(this Vector2 vector, float addend) => vector + new Vector2(addend, addend);

		/// <summary>
		/// Adds a value to all vector axis
		/// </summary>
		/// <param name="vector">The vector to add the value to</param>
		/// <param name="addend">The value to add</param>
		/// <returns>A vector with the value added to all axis</returns>
		public static Vector3 Add(this Vector3 vector, float addend) => vector + new Vector3(addend, addend, addend);

		/// <summary>
		/// Subtracts a value from all vector axis
		/// </summary>
		/// <param name="vector">The vector to subtract the value from</param>
		/// <param name="subtrahend">The value to subtract</param>
		/// <returns>A vector with the value subtracted from all axis</returns>
		public static Vector2 Subtract(this Vector2 vector, float subtrahend) => vector - new Vector2(subtrahend, subtrahend);

		/// <summary>
		/// Subtracts a value from all vector axis
		/// </summary>
		/// <param name="vector">The vector to subtract the value from</param>
		/// <param name="subtrahend">The value to subtract</param>
		/// <returns>A vector with the value subtracted from all axis</returns>
		public static Vector3 Subtract(this Vector3 vector, float subtrahend) => vector - new Vector3(subtrahend, subtrahend, subtrahend);

		/// <summary>
		/// Returns the floor for each of the X Y vector values
		/// </summary>
		/// <param name="vector">The vector to floor</param>
		/// <returns>A floored vector</returns>
		public static Vector2 Floor(this Vector2 vector)
		{
			vector.x = Mathf.Floor(vector.x);
			vector.y = Mathf.Floor(vector.y);

			return vector;
		}

		/// <summary>
		/// Returns the floor for each of the X Y Z vector values
		/// </summary>
		/// <param name="vector">The vector to floor</param>
		/// <returns>A floored vector</returns>
		public static Vector3 Floor(this Vector3 vector)
		{
			vector.x = Mathf.Floor(vector.x);
			vector.y = Mathf.Floor(vector.y);
			vector.z = Mathf.Floor(vector.z);

			return vector;
		}

		/// <summary>
		/// Returns the ceil for each of the X Y vector values
		/// </summary>
		/// <param name="vector">The vector to ceil</param>
		/// <returns>A ceiled vector</returns>
		public static Vector2 Ceil(this Vector2 vector)
		{
			vector.x = Mathf.Ceil(vector.x);
			vector.y = Mathf.Ceil(vector.y);

			return vector;
		}

		/// <summary>
		/// Returns the ceil for each of the X Y Z vector values
		/// </summary>
		/// <param name="vector">The vector to ceil</param>
		/// <returns>A ceiled vector</returns>
		public static Vector3 Ceil(this Vector3 vector)
		{
			vector.x = Mathf.Ceil(vector.x);
			vector.y = Mathf.Ceil(vector.y);
			vector.z = Mathf.Ceil(vector.z);

			return vector;
		}
	}
}
