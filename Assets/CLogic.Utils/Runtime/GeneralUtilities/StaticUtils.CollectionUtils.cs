using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace CLogic.Utils
{
	public partial class StaticUtils
	{
		/// <summary>
		/// Formats an en into a better string format by showing its contents
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable">The enumerable to format</param>
		/// <returns>A string with the array and it's contents</returns>
		public static string Format<T>(this IEnumerable<T> enumerable) => $"{enumerable.GetType()} {{{string.Join(", ", enumerable)}}}";

		/// <summary>
		/// Checks a list of objects for duplicate items, the object must implement IEquatable
		/// </summary>
		/// <typeparam name="T">The object type implementing IEquatable</typeparam>
		/// <param name="objectList">The list to check</param>
		/// <param name="foundMaches">The amount of matches found</param>
		/// <returns>True if duplicates are found</returns>
		public static bool CheckObjectListDuplicate<T>(List<T> objectList, out int foundMaches) where T : IEquatable<T>
		{
			if (objectList.IsNullOrEmpty())
			{
				foundMaches = 0;
				return false;
			}

			List<T> dupeDataList = new();

			dupeDataList = objectList.FindAll((objectData) =>
			{
				int matchCount = 0;

				foreach (var data in objectList)
				{
					if (objectData == null || data == null)
						continue;

					if (objectData.Equals(data))
						matchCount++;
				}

				return matchCount > 1;
			});

			foundMaches = dupeDataList.Count;

			return dupeDataList.Count > 1;
		}

		/// <summary>
		/// Checks if the <paramref name="collection"/> is null or empty
		/// </summary>
		/// <param name="collection">The collection to check</param>
		/// <returns>True if is null or empty, false if not</returns>
		public static bool IsNullOrEmpty(this ICollection collection) => collection == null || collection.Count == 0;

		/// <summary>
		/// Gets a random element from a collection
		/// </summary>
		/// <typeparam name="T">The type of the collection</typeparam>
		/// <param name="collection">The collection to take the random item from</param>
		/// <returns>A random element from the collection</returns>
		public static T RandomElement<T>(this IList<T> collection) => collection[Random.Range(0, collection.Count)];

		/// <summary>
		/// Gets the coordinates of a two dimensional array
		/// </summary>
		/// <typeparam name="T">Array type</typeparam>
		/// <param name="matrix">The 2D array</param>
		/// <param name="value">The array value you want to get the coordinates from</param>
		/// <returns>A tuple containing the coordinates of the array</returns>
		public static (int row, int column) CoordinatesOf<T>(this T[,] matrix, T value)
		{
			int row = matrix.GetLength(0);
			int column = matrix.GetLength(1);

			for (int x = 0; x < row; ++x)
			{
				for (int y = 0; y < column; ++y)
				{
					if (matrix[x, y].Equals(value))
						return (x, y);
				}
			}

			return (-1, -1);
		}

		/// <summary>
		/// Enables a single object from a list of objects
		/// </summary>
		/// <param name="objects">Array of objects to toggle from</param>
		/// <param name="index">Object index to toggle on</param>
		public static void Toggle(this GameObject[] objects, int index)
		{
			foreach (var @object in objects)
				@object.SetActive(false);

			objects[index].SetActive(true);
		}
	}
}
