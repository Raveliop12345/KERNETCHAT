using UnityEngine;

namespace AnimationCraft.Core
{
    [System.Serializable]
    public class WorldSettings
    {
        public string worldId = "AnimationCraft-World";
        public int seed = 0;
        public float scale = 0.01f;
        public float amplitude = 32f;
        public int octaves = 4;
        public float lacunarity = 2f;
        public float persistence = 0.5f;
        public int viewRadius = Constants.DefaultViewRadius;
    }
}
