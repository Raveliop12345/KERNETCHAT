using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace AnimationCraft.Meshing
{
    public struct MeshData
    {
        public NativeList<float3> positions;
        public NativeList<float3> normals;
        public NativeList<Color32> colors;
        public NativeList<int> indices;

        public MeshData(int initialCapacity, Allocator allocator)
        {
            positions = new NativeList<float3>(initialCapacity, allocator);
            normals = new NativeList<float3>(initialCapacity, allocator);
            colors = new NativeList<Color32>(initialCapacity, allocator);
            indices = new NativeList<int>(initialCapacity * 6, allocator);
        }

        public void Dispose()
        {
            if (positions.IsCreated) positions.Dispose();
            if (normals.IsCreated) normals.Dispose();
            if (colors.IsCreated) colors.Dispose();
            if (indices.IsCreated) indices.Dispose();
        }

        public Mesh ToMesh()
        {
            var mesh = new Mesh();
            mesh.indexFormat = IndexFormat.UInt32;
            var verts = new Vector3[positions.Length];
            var nors = new Vector3[normals.Length];
            var cols = new Color32[colors.Length];
            for (int i = 0; i < positions.Length; i++) verts[i] = positions[i];
            for (int i = 0; i < normals.Length; i++) nors[i] = normals[i];
            for (int i = 0; i < colors.Length; i++) cols[i] = colors[i];
            var tris = new int[indices.Length];
            for (int i = 0; i < indices.Length; i++) tris[i] = indices[i];
            mesh.SetVertices(verts);
            mesh.SetNormals(nors);
            mesh.SetColors(cols);
            mesh.SetTriangles(tris, 0);
            return mesh;
        }
    }
}
