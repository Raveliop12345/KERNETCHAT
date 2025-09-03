using UnityEngine;
using FNL = FastNoiseLite.FastNoiseLite;

namespace AnimationCraft.Generation
{
    public class NoiseProvider
    {
        readonly int seed;
        readonly float scale;
        readonly int octaves;
        readonly float lacunarity;
        readonly float persistence;
        readonly float amplitude;
        readonly FNL noise;

        public NoiseProvider(int seed, float scale, int octaves, float lacunarity, float persistence, float amplitude)
        {
            this.seed = seed;
            this.scale = Mathf.Max(0.0001f, scale);
            this.octaves = Mathf.Max(1, octaves);
            this.lacunarity = Mathf.Max(1f, lacunarity);
            this.persistence = Mathf.Clamp01(persistence);
            this.amplitude = amplitude;
            noise = new FNL(seed);
            noise.SetFrequency(1f);
        }

        public int SampleHeight(int x, int z)
        {
            float freq = 1f / scale;
            float amp = 1f;
            float sum = 0f;
            float max = 0f;
            for (int i = 0; i < octaves; i++)
            {
                float n = noise.GetNoise(x * freq, z * freq);
                sum += n * amp;
                max += amp;
                amp *= persistence;
                freq *= lacunarity;
            }
            float hn = (sum / max) * 0.5f + 0.5f; // [0,1]
            float h = hn * amplitude + 32f; // base height
            return Mathf.Clamp(Mathf.FloorToInt(h), 1, 255);
        }
    }
}
