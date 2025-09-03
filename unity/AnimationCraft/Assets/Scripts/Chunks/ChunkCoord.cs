using System;

namespace AnimationCraft.Chunks
{
    [Serializable]
    public struct ChunkCoord : IEquatable<ChunkCoord>
    {
        public int x;
        public int y;
        public int z;

        public ChunkCoord(int x, int y, int z)
        {
            this.x = x; this.y = y; this.z = z;
        }

        public bool Equals(ChunkCoord other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public override bool Equals(object obj)
        {
            return obj is ChunkCoord other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked { return (x * 73856093) ^ (y * 19349663) ^ (z * 83492791); }
        }

        public override string ToString()
        {
            return $"({x},{y},{z})";
        }
    }
}
