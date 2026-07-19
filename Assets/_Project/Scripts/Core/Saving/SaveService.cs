using System;
using System.IO;
using UnityEngine;

namespace FarmFarmer.Core
{
    // Local-first, atomic write (write temp, then rename) per CLAUDE.md architecture intents.
    public static class SaveService
    {
        // v2 (2026-07-19): added SaveData.gems + HeroSaveState.highestStageReached. Additive
        // only -- JsonUtility defaults missing fields, so v1 saves load without a migration step
        // (highestStageReached backfill happens in RosterService.Hydrate).
        public const int CurrentSchemaVersion = 2;

        private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

        public static void Save(SaveData data)
        {
            data.schemaVersion = CurrentSchemaVersion;
            data.savedAtUtc = DateTime.UtcNow.ToString("o");

            var json = JsonUtility.ToJson(data, true);
            var tempPath = SavePath + ".tmp";
            File.WriteAllText(tempPath, json);

            if (File.Exists(SavePath))
            {
                File.Replace(tempPath, SavePath, null);
            }
            else
            {
                File.Move(tempPath, SavePath);
            }
        }

        public static SaveData Load()
        {
            if (!File.Exists(SavePath)) return new SaveData();

            var json = File.ReadAllText(SavePath);
            var data = JsonUtility.FromJson<SaveData>(json);
            return data ?? new SaveData();
        }
    }
}
