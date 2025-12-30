using System;
using System.IO;
using UnityEngine;
using CLogic.Core.DataSaving;
using CLogic.Core.DataSaving.Obfuscation;
using CLogic.Utils.Runtime.DataSaving.Obfuscator;

namespace CLogic.Utils.DataSaving.Sections
{
	public enum SavingMode
	{
		Manual,
		Instant,
		Dirty
	}
	
	public abstract class BaseDataSectionSo : ScriptableObject, IDataSection
	{
		public abstract IDataObfuscator GetObfuscator();
		public abstract IDataPath GetDataPath();
		public abstract string GetSectionId();
		public abstract bool AllowHumanEdits { get; set; }

		public SavingMode savingMode = SavingMode.Instant;
	}

	public class PersistentPath : IDataPath
	{
		private string path;

		public string GetPath() => path;

		public PersistentPath SetPath(string newPath)
		{
			path = newPath;
			return this;
		}
	}

	[CreateAssetMenu(fileName = "Data Section", menuName = "CLogic/Data Saving/Data Section")]
	public class DataSectionSo : BaseDataSectionSo
	{
		public string sectionId;

		public string relativePath;
		public string fileName;

		[field: SerializeField]
		public override bool AllowHumanEdits { get; set; } = true;
		
		public ObfuscatorSo obfuscator;

		private readonly PersistentPath persistentPath = new();

		protected virtual void Reset()
		{
			sectionId = "default";
			relativePath = "%PERSISTENT_DATA%/CLogic/Testing Saves";
			fileName = "game_data.json";
		}

		public override IDataPath GetDataPath()
		{
			char dirSeparator = Path.DirectorySeparatorChar;
			string expandedPath = Environment.ExpandEnvironmentVariables(relativePath).Replace('\\', dirSeparator).Replace('/', dirSeparator);

			string finalPath = Path.Combine(expandedPath, fileName);

			return persistentPath.SetPath(finalPath);
		}
		public override IDataObfuscator GetObfuscator()
		{
			return obfuscator == null ? new StringObfuscator() : obfuscator;
		}
		public override string GetSectionId() => sectionId;
		
	}
}
