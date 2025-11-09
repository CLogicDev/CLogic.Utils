using System;
using System.IO;

namespace CLogic.Utils
{
	public static partial class StaticUtils
	{
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
