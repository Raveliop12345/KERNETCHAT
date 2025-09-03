using System.Collections.Generic;
using UnityEngine;

namespace AnimationCraft.Chunks
{
    public class ChunkPool
    {
        readonly Stack<GameObject> pool = new Stack<GameObject>();
        readonly Transform parent;
        readonly Material material;

        public ChunkPool(Transform parent, Material material)
        {
            this.parent = parent;
            this.material = material;
        }

        public GameObject Acquire()
        {
            if (pool.Count > 0)
            {
                var go = pool.Pop();
                go.SetActive(true);
                return go;
            }
            var obj = new GameObject("Chunk");
            obj.transform.parent = parent;
            var mf = obj.AddComponent<MeshFilter>();
            var mr = obj.AddComponent<MeshRenderer>();
            mr.sharedMaterial = material;
            obj.AddComponent<MeshCollider>();
            obj.AddComponent<ChunkFader>();
            return obj;
        }

        public void Release(GameObject go)
        {
            go.SetActive(false);
            go.transform.SetParent(parent);
            var mf = go.GetComponent<MeshFilter>();
            if (mf.sharedMesh) { Object.DestroyImmediate(mf.sharedMesh); }
            var mc = go.GetComponent<MeshCollider>();
            mc.sharedMesh = null;
            pool.Push(go);
        }
    }
}
