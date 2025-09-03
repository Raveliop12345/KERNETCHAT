using UnityEngine;

namespace AnimationCraft.Chunks
{
    public static class ChunkRenderer
    {
        public static void ApplyMesh(GameObject go, Mesh mesh)
        {
            var mf = go.GetComponent<MeshFilter>();
            var mc = go.GetComponent<MeshCollider>();
            mesh.RecalculateBounds();
            mf.sharedMesh = mesh;
            mc.sharedMesh = mesh;
        }
    }
}
