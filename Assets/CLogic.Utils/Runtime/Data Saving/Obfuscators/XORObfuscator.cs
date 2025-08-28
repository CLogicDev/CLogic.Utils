using CLogic.Core.DataSaving.Obfuscation;
using UnityEngine;
namespace CLogic.Utils.Runtime.DataSaving.Obfuscator
{
    [CreateAssetMenu(fileName = "XOR Obfuscator", menuName = "CLogic/Data Saving/Obfuscator/XOR Obfuscator")]
    public class XORObfuscator : ObfuscatorSo
    {

        Core.DataSaving.Obfuscation.XORObfuscator obfuscator = new ();
        
        public override byte[] Obfuscate(string jsonData)
        {
            return obfuscator.Obfuscate(jsonData);
        }
        public override string DeObfuscate(byte[] obfuscatedData)
        {
            return obfuscator.DeObfuscate(obfuscatedData);
        }
    }
}
