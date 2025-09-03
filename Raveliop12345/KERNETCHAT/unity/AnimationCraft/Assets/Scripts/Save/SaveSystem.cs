using System.IO;
using AnimationCraft.Chunks;
using Unity.Collections;

namespace AnimationCraft.Save
{
    public class SaveSystem
    {
        readonly string worldId;
        readonly int seed;

        public SaveSystem(string gameId, string worldId, int seed)
        {
            this.worldId = worldId;
            this.seed = seed;
            WorldIndex.EnsureWorld(worldId, seed);
        }

        string ChunkFile(ChunkCoord cc)
        {
            var name = $"{cc.x}_{cc.y}_{cc.z}.bin";
            return Path.Combine(WorldIndex.ChunksPath(worldId), name);
        }

        public bool TryLoadChunk(ChunkCoord cc, NativeArray<byte> dst)
        {
            var path = ChunkFile(cc);
            if (!File.Exists(path)) return false;
            using var fs = File.OpenRead(path);
            using var br = new BinaryReader(fs);
            ushort version = br.ReadUInt16();
            if (version != 1) return false;
            int size = br.ReadInt32();
            if (size != dst.Length) return false;
            ChunkSerializer.ReadRLE(br, dst);
            return true;
        }

        public void SaveChunk(ChunkCoord cc, NativeArray<byte> src)
        {
            var path = ChunkFile(cc);
            using var fs = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);
            using var bw = new BinaryWriter(fs);
            bw.Write((ushort)1); // version
            bw.Write(src.Length);
            ChunkSerializer.WriteRLE(bw, src);
        }
    }
}
