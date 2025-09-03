using System;
using System.IO;
using UnityEngine;

namespace AnimationCraft.Save
{
    [Serializable]
    public class WorldInfo
    {
        public int version = 1;
        public string worldId;
        public int seed;
        public string createdAt;
        public string lastPlayed;
    }

    public static class WorldIndex
    {
        public static string SavesRoot => Path.Combine(Application.persistentDataPath, "Saves/ANIMATION_CRAFT");

        public static string WorldPath(string worldId) => Path.Combine(SavesRoot, worldId);
        public static string ChunksPath(string worldId) => Path.Combine(WorldPath(worldId), "chunks");
        public static string WorldJson(string worldId) => Path.Combine(WorldPath(worldId), "world.json");

        public static void EnsureWorld(string worldId, int seed)
        {
            Directory.CreateDirectory(ChunksPath(worldId));
            var p = WorldJson(worldId);
            WorldInfo info;
            if (File.Exists(p))
            {
                info = JsonUtility.FromJson<WorldInfo>(File.ReadAllText(p));
            }
            else
            {
                info = new WorldInfo
                {
                    worldId = worldId,
                    seed = seed,
                    createdAt = DateTime.UtcNow.ToString("o"),
                    lastPlayed = DateTime.UtcNow.ToString("o")
                };
            }
            info.lastPlayed = DateTime.UtcNow.ToString("o");
            File.WriteAllText(p, JsonUtility.ToJson(info, true));
        }

        public static string[] ListWorlds()
        {
            if (!Directory.Exists(SavesRoot)) return Array.Empty<string>();
            var dirs = Directory.GetDirectories(SavesRoot);
            for (int i = 0; i < dirs.Length; i++)
            {
                dirs[i] = Path.GetFileName(dirs[i]);
            }
            return dirs;
        }

        public static void DeleteWorld(string worldId)
        {
            var path = WorldPath(worldId);
            if (Directory.Exists(path)) Directory.Delete(path, true);
        }
    }
}
