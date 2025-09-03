using AnimationCraft.Core;
using AnimationCraft.Voxel;
using Unity.Collections;
using UnityEngine;

namespace AnimationCraft.Chunks
{
    public class Chunk
    {
        public ChunkCoord coord;
        public GameObject go;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        public MeshCollider meshCollider;
        public bool dirty;
        public NativeArray<byte> voxels;

        public Chunk(ChunkCoord coord, GameObject go)
        {
            this.coord = coord;
            this.go = go;
            meshFilter = go.GetComponent<MeshFilter>();
            meshRenderer = go.GetComponent<MeshRenderer>();
            meshCollider = go.GetComponent<MeshCollider>();
            var count = Constants.ChunkSizeX * Constants.ChunkSizeY * Constants.ChunkSizeZ;
            voxels = new NativeArray<byte>(count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
            dirty = true;
        }

        public void Dispose()
        {
            if (voxels.IsCreated) voxels.Dispose();
        }

        public static int Index(int x, int y, int z)
        {
            return x + Constants.ChunkSizeX * (z + Constants.ChunkSizeZ * y);
        }

        public byte Get(int x, int y, int z)
        {
            return voxels[Index(x, y, z)];
        }

        public void Set(int x, int y, int z, byte id)
        {
            voxels[Index(x, y, z)] = id;
            dirty = true;
        }

        public Vector3 WorldPosition
        {
            get
            {
                return new Vector3(
                    coord.x * Constants.ChunkSizeX,
                    coord.y * Constants.ChunkSizeY,
                    coord.z * Constants.ChunkSizeZ
                );
            }
        }
    }
}
