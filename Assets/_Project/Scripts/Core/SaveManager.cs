using System.IO;
using UnityEngine;

namespace TimeAttackBlock
{
    public static class SaveManager
    {
        private const string FileName = "save.json";

        [System.Serializable]
        public struct SaveData
        {
            public int bestScore;
            public string selectedSkin;
            public int playCount;
            public float totalPlayTime;
            public int maxCombo;
            public int rerollCount;
        }

        public static SaveData Load()
        {
            var path = Path.Combine(Application.persistentDataPath, FileName);
            if (!File.Exists(path)) return new SaveData();
            return JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        }

        public static void Save(SaveData data)
        {
            var path = Path.Combine(Application.persistentDataPath, FileName);
            File.WriteAllText(path, JsonUtility.ToJson(data));
        }
    }
}