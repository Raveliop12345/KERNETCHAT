using UnityEngine;

namespace AnimationCraft.Meshing
{
    public static class MeshApplier
    {
        public static void Apply(UnityEngine.GameObject go, MeshData md)
        {
            var mesh = md.ToMesh();
            var mf = go.GetComponent<MeshFilter>();
            var mc = go.GetComponent<MeshCollider>();
            mf.sharedMesh = mesh;
            mc.sharedMesh = mesh;
        }
    }
}
