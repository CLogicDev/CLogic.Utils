using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace CLogic.Utils
{
	public partial class StaticUtils
	{
		/// <summary>
		/// Generates a random integer within a range
		/// </summary>
		/// <param name="min">Minimum range inclusive</param>
		/// <param name="max">Maximum range inclusive</param>
		/// <returns>A random interger</returns>
		public static int RandomIntInclusive(int min, int max) => Random.Range(min, max + 1);

		/// <summary>
		/// Generates a random number thats always different from the last chosen number
		/// </summary>
		/// <param name="min">Minimum range inclusive</param>
		/// <param name="max">Minimum range exclusive</param>
		/// <param name="lastChosenNumber">Reference to set the last chosen number</param>
		/// <returns>A random different number</returns>
		public static int RandomRangeNoDupe(int min, int max, ref int lastChosenNumber)
		{
			int generatedNumber = lastChosenNumber;

			while (generatedNumber == lastChosenNumber)
				generatedNumber = Random.Range(min, max);

			lastChosenNumber = generatedNumber;

			return generatedNumber;
		}

		/// <summary>
		/// Generates a random Vector3 within a range
		/// </summary>
		/// <param name="min">Minimum range inclusive</param>
		/// <param name="max">Maximum range inclusive</param>
		/// <param name="excludeX">Don't generate X value</param>
		/// <param name="excludeY">Don't generate Y value</param>
		/// <param name="excludeZ">Don't generate Z value</param>
		/// <returns>A random Vector3</returns>
		public static Vector3 RandomVector(float min, float max, bool excludeX = false, bool excludeY = false, bool excludeZ = false) => new(excludeX ? 0f : Random.Range(min, max), excludeY ? 0f : Random.Range(min, max), excludeZ ? 0f : Random.Range(min, max));

		/// <summary>
		/// Generates a random number between the X and Y values of Vector2, inclusive
		/// </summary>
		/// <param name="vector">The Vector2 to get the range from</param>
		/// <returns>A random float</returns>
		public static float RandomFromVector2(Vector2 vector) => Random.Range(vector.x, vector.y);

		/// <summary>
		/// Generates a random point on a navmesh
		/// </summary>
		/// <param name="origin">The origin to generate the random position from</param>
		/// <param name="range">How far from the origin to generate the point</param>
		/// <param name="areaMask">Navmesh surface area mask</param>
		/// <returns>A Vector3 with the random position</returns>
		public static Vector3 RandomNavmeshPoint(Vector3 origin, float range, int areaMask)
		{
			Vector3 randomDirection = Random.insideUnitSphere * range;

			randomDirection += origin;

			NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, range, areaMask);

			return navHit.position;
		}

		/// <summary>
		/// Returns a random point within a box collider
		/// </summary>
		/// <param name="boxCollider">The box collider to get the random point from</param>
		/// <returns>A Vector3 of the point</returns>
		public static Vector3 GetRandomPoint(this BoxCollider boxCollider) => new(Random.Range(boxCollider.bounds.min.x, boxCollider.bounds.max.x), Random.Range(boxCollider.bounds.min.y, boxCollider.bounds.max.y), Random.Range(boxCollider.bounds.min.z, boxCollider.bounds.max.z));

		/// <summary>
		/// Returns a random point within a sphere collider
		/// <break>
		/// </summary>
		/// <param name="sphereCollider">The sphere collider to get the random point from</param>
		/// <returns>A Vector3 of the point</returns>
		public static Vector3 GetRandomPoint(this SphereCollider sphereCollider)
		{
			Vector3 center = sphereCollider.center;
			float radius = sphereCollider.radius;

			Vector3 randomDirection = Random.insideUnitSphere;
			Vector3 randomPoint = center + randomDirection * radius;

			return randomPoint;
		}

		/// <summary>
		/// Returns a random point within a mesh collider
		/// </summary>
		/// <remarks>Expensive to compute, aviod using it in frequently updated code</remarks>
		/// <param name="meshCollider">The mesh collider to get the random point from</param>
		/// <returns>A Vector3 of the point</returns>
		public static Vector3 GetRandomPoint(this MeshCollider meshCollider)
		{
			Mesh mesh = meshCollider.sharedMesh;

			int[] triangles = mesh.triangles;
			Vector3[] vertices = mesh.vertices;

			// Choose a random triangle in the mesh
			int randomIndex = Random.Range(0, triangles.Length / 3);

			// Get the vertices of the triangle
			Vector3 vertex1 = vertices[triangles[randomIndex * 3]];
			Vector3 vertex2 = vertices[triangles[randomIndex * 3 + 1]];
			Vector3 vertex3 = vertices[triangles[randomIndex * 3 + 2]];

			// Choose a random point in the triangle
			float barycentricCoord1 = Random.Range(0f, 1f);
			float barycentricCoord2 = Random.Range(0f, 1f);

			if (barycentricCoord1 + barycentricCoord2 > 1)
			{
				barycentricCoord1 = 1 - barycentricCoord1;
				barycentricCoord2 = 1 - barycentricCoord2;
			}

			float barycentricCoord3 = 1 - barycentricCoord1 - barycentricCoord2;
			Vector3 randomPoint = barycentricCoord1 * vertex1 + barycentricCoord2 * vertex2 + barycentricCoord3 * vertex3;

			return meshCollider.transform.TransformPoint(randomPoint);
		}
		
		[Serializable]
		public struct WeightedNumber
		{
			/// <summary>
			/// The number that will be returned
			/// </summary>
			public int number;

			/// <summary>
			/// The probability of that number to be returned
			/// </summary>
			public float probability;

			public WeightedNumber(int number, float probability)
			{
				this.number = number;
				this.probability = probability;
			}
		}
		
		/// <summary>
		/// Returns a number based on a probability
		/// </summary>
		/// <param name="weightedNumbers">The numbers and probabilities</param>
		/// <returns></returns>
		public static int WeightedRandom(params WeightedNumber[] weightedNumbers)
		{
			//Get total probability
			float totalProbability = 0;
			foreach (var number in weightedNumbers)
			{
				totalProbability += number.probability;
			}

			//Normalize probabilities
			for (int i = 0; i < weightedNumbers.Length; i++)
			{
				weightedNumbers[i].probability /= totalProbability;
			}

			float randomPoint = Random.value;
			for (int i = 0; i < weightedNumbers.Length; i++)
			{
				if (randomPoint < weightedNumbers[i].probability)
				{
					return weightedNumbers[i].number;
				}
				else
				{
					randomPoint -= weightedNumbers[i].probability;
				}
			}
			return weightedNumbers[^1].number;
		}
	}
}
