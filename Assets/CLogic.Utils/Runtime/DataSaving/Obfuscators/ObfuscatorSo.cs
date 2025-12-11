using CLogic.Core.DataSaving.Obfuscation;
using UnityEngine;
namespace CLogic.Utils.Runtime.DataSaving.Obfuscator
{
    public abstract class ObfuscatorSo : ScriptableObject, IDataObfuscator
    {

        public abstract byte[] Obfuscate(string jsonData);
        public abstract string DeObfuscate(byte[] obfuscatedData);
    }
}
