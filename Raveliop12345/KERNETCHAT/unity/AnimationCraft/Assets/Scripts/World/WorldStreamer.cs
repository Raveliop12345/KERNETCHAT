using System.Collections.Generic;
using AnimationCraft.Chunks;
using AnimationCraft.Core;
using AnimationCraft.Generation;
using AnimationCraft.Meshing;
using AnimationCraft.Save;
using AnimationCraft.Voxel;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace AnimationCraft.World
{
    public class WorldStreamer : MonoBehaviour
    {
        public Transform player;
        public int viewRadius = Constants.DefaultViewRadius;
        public int maxJobsPerFrame = 2;
        public bool enableSave = true;

        Material chunkMaterial;
        ChunkPool pool;
        readonly Dictionary<ChunkCoord, Chunk> active = new Dictionary<ChunkCoord, Chunk>();
        readonly List<MeshTask> tasks = new List<MeshTask>();

        NoiseProvider noise;
        SaveSystem save;

        Vector3 lastPlayerPos;
        readonly List<ChunkCoord> ringOrder = new List<ChunkCoord>();

        struct MeshTask
        {
            public Chunk chunk;
            public JobHandle handle;
            public MeshData meshData;
        }

        void Awake()
        {
            if (!player)
            {
                var p = GameObject.FindWithTag("Player");
                if (p) player = p.transform;
            }
            if (!chunkMaterial)
            {
                var shader = Shader.Find("AnimationCraft/DitherLitVertexColor");
                if (!shader) shader = Shader.Find("AnimationCraft/UnlitVertexColor");
                chunkMaterial = new Material(shader);
            }
            var parent = new GameObject("Chunks").transform;
            parent.SetParent(transform);
            pool = new ChunkPool(parent, chunkMaterial);

            var settings = new WorldSettings();
            settings.viewRadius = WorldSession.ViewRadius;
            settings.seed = WorldSession.Seed == 0 ? UnityEngine.Random.Range(int.MinValue, int.MaxValue) : WorldSession.Seed;
            noise = new NoiseProvider(settings.seed, settings.scale, settings.octaves, settings.lacunarity, settings.persistence, settings.amplitude);
            save = new SaveSystem("ANIMATION_CRAFT", WorldSession.WorldId, settings.seed);
            viewRadius = settings.viewRadius;
            BuildRingOrder(viewRadius);
        }

        void BuildRingOrder(int r)
        {
            ringOrder.Clear();
            for (int dz = -r; dz <= r; dz++)
                for (int dx = -r; dx <= r; dx++)
                {
                    int dist2 = dx * dx + dz * dz;
                    if (dist2 <= r * r) ringOrder.Add(new ChunkCoord(dx, 0, dz));
                }
            ringOrder.Sort((a, b) => (a.x * a.x + a.z * a.z).CompareTo(b.x * b.x + b.z * b.z));
        }

        void Start()
        {
            lastPlayerPos = Vector3.positiveInfinity;
        }

        void Update()
        {
            if (!player) return;
            var p = player.position;
            if ((p - lastPlayerPos).sqrMagnitude > (Constants.ChunkSizeX * 0.5f) * (Constants.ChunkSizeX * 0.5f))
            {
                lastPlayerPos = p;
                UpdateVisibleChunks();
            }

            for (int i = tasks.Count - 1; i >= 0; i--)
            {
                var t = tasks[i];
                if (t.handle.IsCompleted)
                {
                    t.handle.Complete();
                    MeshApplier.Apply(t.chunk.go, t.meshData);
                    t.meshData.Dispose();
                    tasks.RemoveAt(i);
                }
            }
        }

        void UpdateVisibleChunks()
        {
            int pcx = Mathf.FloorToInt(player.position.x / Constants.ChunkSizeX);
            int pcz = Mathf.FloorToInt(player.position.z / Constants.ChunkSizeZ);
            var wanted = new HashSet<ChunkCoord>();
            foreach (var c in ringOrder)
            {
                var cc = new ChunkCoord(c.x + pcx, 0, c.z + pcz);
                wanted.Add(cc);
                if (!active.ContainsKey(cc))
                {
                    LoadChunk(cc);
                }
            }

            var toRemove = new List<ChunkCoord>();
            foreach (var kv in active)
            {
                if (!wanted.Contains(kv.Key)) toRemove.Add(kv.Key);
            }
            foreach (var cc in toRemove)
            {
                UnloadChunk(cc);
            }
        }

        void LoadChunk(ChunkCoord cc)
        {
            var go = pool.Acquire();
            go.name = $"Chunk {cc.x},{cc.y},{cc.z}";
            go.transform.position = new Vector3(
                cc.x * Constants.ChunkSizeX,
                cc.y * Constants.ChunkSizeY,
                cc.z * Constants.ChunkSizeZ);

            var chunk = new Chunk(cc, go);
            active.Add(cc, chunk);

            if (enableSave && save.TryLoadChunk(cc, chunk.voxels))
            {
                EnqueueMeshing(chunk);
                return;
            }

            GenerateChunk(chunk);
            if (enableSave) save.SaveChunk(cc, chunk.voxels);
            EnqueueMeshing(chunk);
        }

        void UnloadChunk(ChunkCoord cc)
        {
            if (!active.TryGetValue(cc, out var chunk)) return;
            chunk.Dispose();
            pool.Release(chunk.go);
            active.Remove(cc);
        }

        void GenerateChunk(Chunk chunk)
        {
            int sx = Constants.ChunkSizeX;
            int sy = Constants.ChunkSizeY;
            int sz = Constants.ChunkSizeZ;

            for (int z = 0; z < sz; z++)
            {
                int wz = chunk.coord.z * sz + z;
                for (int x = 0; x < sx; x++)
                {
                    int wx = chunk.coord.x * sx + x;
                    int height = noise.SampleHeight(wx, wz);
                    for (int y = 0; y < sy; y++)
                    {
                        byte id = (byte)BlockId.Air;
                        int wy = chunk.coord.y * sy + y;
                        if (wy == 0) id = (byte)BlockId.Bedrock;
                        else if (wy < height - 4) id = (byte)BlockId.Stone;
                        else if (wy < height - 1) id = (byte)BlockId.Dirt;
                        else if (wy == height) id = (byte)BlockId.Grass;
                        if (height <= 2 && wy <= height) id = (byte)BlockId.Sand;
                        chunk.Set(x, y, z, id);
                    }
                }
            }
        }

        void EnqueueMeshing(Chunk chunk)
        {
            var md = new MeshData(1024, Allocator.TempJob);
            var job = new GreedyMesherJob
            {
                voxels = chunk.voxels,
                meshData = md
            };
            var handle = job.Schedule();
            tasks.Add(new MeshTask { chunk = chunk, handle = handle, meshData = md });
        }

        public void SetBlock(Vector3Int worldPos, byte id)
        {
            var cc = WorldToChunk(worldPos, out int lx, out int ly, out int lz);
            if (!active.TryGetValue(cc, out var chunk)) return;
            chunk.Set(lx, ly, lz, id);
            if (enableSave) save.SaveChunk(cc, chunk.voxels);
            EnqueueMeshing(chunk);
            if (lx == 0) RemeshNeighbor(new ChunkCoord(cc.x - 1, cc.y, cc.z));
            else if (lx == Constants.ChunkSizeX - 1) RemeshNeighbor(new ChunkCoord(cc.x + 1, cc.y, cc.z));
            if (lz == 0) RemeshNeighbor(new ChunkCoord(cc.x, cc.y, cc.z - 1));
            else if (lz == Constants.ChunkSizeZ - 1) RemeshNeighbor(new ChunkCoord(cc.x, cc.y, cc.z + 1));
            if (ly == 0) RemeshNeighbor(new ChunkCoord(cc.x, cc.y - 1, cc.z));
            else if (ly == Constants.ChunkSizeY - 1) RemeshNeighbor(new ChunkCoord(cc.x, cc.y + 1, cc.z));
        }

        public void SetViewRadius(int r)
        {
            r = Mathf.Clamp(r, 1, 12);
            if (r == viewRadius) return;
            viewRadius = r;
            BuildRingOrder(viewRadius);
            UpdateVisibleChunks();
        }

        ChunkCoord WorldToChunk(Vector3Int w, out int lx, out int ly, out int lz)
        {
            int cx = Mathf.FloorToInt((float)w.x / Constants.ChunkSizeX);
            int cy = Mathf.FloorToInt((float)w.y / Constants.ChunkSizeY);
            int cz = Mathf.FloorToInt((float)w.z / Constants.ChunkSizeZ);
            lx = w.x - cx * Constants.ChunkSizeX; if (lx < 0) lx += Constants.ChunkSizeX;
            ly = w.y - cy * Constants.ChunkSizeY; if (ly < 0) ly += Constants.ChunkSizeY;
            lz = w.z - cz * Constants.ChunkSizeZ; if (lz < 0) lz += Constants.ChunkSizeZ;
            return new ChunkCoord(cx, cy, cz);
        }

        void RemeshNeighbor(ChunkCoord cc)
        {
            if (active.TryGetValue(cc, out var n)) EnqueueMeshing(n);
        }

        public int ActiveChunkCount => active.Count;
        public int PendingJobs => tasks.Count;
    }
}
