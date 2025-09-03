// Minimal FastNoiseLite-like noise (not full feature)
// Public domain style simplified implementation for MVP
using System;
using UnityEngine;

namespace FastNoiseLite
{
    public class FastNoiseLite
    {
        int seed;
        float frequency = 0.01f;

        public FastNoiseLite(int seed)
        {
            this.seed = seed;
        }

        public void SetFrequency(float f) => frequency = f;

        public float GetNoise(float x, float y)
        {
            // Simple 2D value noise with bilinear interpolation
            x *= frequency; y *= frequency;
            int xi = Mathf.FloorToInt(x);
            int yi = Mathf.FloorToInt(y);
            float xf = x - xi;
            float yf = y - yi;
            float v00 = Hash2D(xi, yi);
            float v10 = Hash2D(xi + 1, yi);
            float v01 = Hash2D(xi, yi + 1);
            float v11 = Hash2D(xi + 1, yi + 1);
            float u = Smooth(xf);
            float v = Smooth(yf);
            float a = Mathf.Lerp(v00, v10, u);
            float b = Mathf.Lerp(v01, v11, u);
            return Mathf.Lerp(a, b, v) * 2f - 1f; // [-1,1]
        }

        static float Smooth(float t) => t * t * (3f - 2f * t);

        float Hash2D(int x, int y)
        {
            uint h = (uint)(x * 374761393 + y * 668265263 + seed * 2654435761);
            h = (h ^ (h >> 13)) * 1274126177u;
            return (h & 0x00FFFFFF) / 16777215f; // [0,1]
        }
    }
}
