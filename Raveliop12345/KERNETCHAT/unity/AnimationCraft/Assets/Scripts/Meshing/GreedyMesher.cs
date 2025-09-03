using AnimationCraft.Core;
using AnimationCraft.Voxel;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace AnimationCraft.Meshing
{
    [BurstCompile]
    public struct GreedyMesherJob : IJob
    {
        [ReadOnly] public NativeArray<byte> voxels;
        public MeshData meshData;

        public void Execute()
        {
            int sx = Constants.ChunkSizeX;
            int sy = Constants.ChunkSizeY;
            int sz = Constants.ChunkSizeZ;

            for (int d = 0; d < 3; d++)
            {
                int u = (d + 1) % 3;
                int v = (d + 2) % 3;
                int[] size = { sx, sy, sz };
                int[] q = { 0, 0, 0 };
                q[d] = 1;

                int maskWidth = size[u];
                int maskHeight = size[v];
                var idMask = new NativeArray<byte>(maskWidth * maskHeight, Allocator.Temp);
                var backMask = new NativeArray<byte>(maskWidth * maskHeight, Allocator.Temp);

                int[] x = { 0, 0, 0 };
                for (x[d] = -1; x[d] < size[d];)
                {
                    int n = 0;
                    for (x[v] = 0; x[v] < size[v]; x[v]++)
                    {
                        for (x[u] = 0; x[u] < size[u]; x[u]++)
                        {
                            byte a = 0, b = 0;
                            if (x[d] >= 0)
                            {
                                int xi = x[0]; int yi = x[1]; int zi = x[2];
                                a = At(xi, yi, zi, sx, sy, sz);
                            }
                            if (x[d] < size[d] - 1)
                            {
                                int xi = x[0] + q[0]; int yi = x[1] + q[1]; int zi = x[2] + q[2];
                                b = At(xi, yi, zi, sx, sy, sz);
                            }

                            bool aSolid = a != 0 && BlockRegistry.IsOpaque((BlockId)a);
                            bool bSolid = b != 0 && BlockRegistry.IsOpaque((BlockId)b);

                            if (aSolid == bSolid)
                            {
                                idMask[n] = 0;
                                backMask[n] = 0;
                            }
                            else if (aSolid)
                            {
                                idMask[n] = a;
                                backMask[n] = 0;
                            }
                            else
                            {
                                idMask[n] = b;
                                backMask[n] = 1;
                            }
                            n++;
                        }
                    }

                    x[d]++;
                    n = 0;
                    for (int j = 0; j < maskHeight; j++)
                    {
                        for (int i = 0; i < maskWidth;)
                        {
                            byte id = idMask[n];
                            if (id == 0)
                            {
                                i++; n++;
                                continue;
                            }

                            int w = 1;
                            while (i + w < maskWidth && idMask[n + w] == id)
                                w++;

                            int h = 1;
                            bool done = false;
                            while (j + h < maskHeight && !done)
                            {
                                for (int k = 0; k < w; k++)
                                {
                                    if (idMask[n + k + h * maskWidth] != id)
                                    {
                                        done = true; break;
                                    }
                                }
                                if (!done) h++;
                            }

                            EmitQuad(d, x, u, v, i, j, w, h, id, backMask[n] == 1);

                            for (int l = 0; l < h; l++)
                                for (int k = 0; k < w; k++)
                                    idMask[n + k + l * maskWidth] = 0;

                            i += w;
                            n += w;
                        }
                        n += maskWidth - maskWidth; // keep n aligned per row
                    }
                }

                idMask.Dispose();
                backMask.Dispose();
            }
        }

        byte At(int x, int y, int z, int sx, int sy, int sz)
        {
            if ((uint)x >= (uint)sx || (uint)y >= (uint)sy || (uint)z >= (uint)sz) return 0;
            return voxels[x + sx * (z + sz * y)];
        }

        void EmitQuad(int d, int[] x, int u, int v, int i, int j, int w, int h, byte id, bool back)
        {
            var props = BlockRegistry.Get((BlockId)id);
            int sx = Constants.ChunkSizeX;
            int sy = Constants.ChunkSizeY;
            int sz = Constants.ChunkSizeZ;

            int[] du = { 0, 0, 0 };
            int[] dv = { 0, 0, 0 };
            du[u] = w;
            dv[v] = h;

            int[] q = { 0, 0, 0 };
            q[d] = back ? 0 : 1;

            float3 p0 = new float3(x[0], x[1], x[2]);
            p0[u] = i;
            p0[v] = j;

            float3 p1 = p0; p1[u] += du[u]; p1[v] += dv[v];
            float3 p2 = p0; p2[v] += dv[v];
            float3 p3 = p0; p3[u] += du[u];

            float3 normal;
            if (d == 0) normal = new float3(back ? -1 : 1, 0, 0);
            else if (d == 1) normal = new float3(0, back ? -1 : 1, 0);
            else normal = new float3(0, 0, back ? -1 : 1);

            int vi = meshData.positions.Length;
            if (back)
            {
                meshData.positions.Add(p0);
                meshData.positions.Add(p2);
                meshData.positions.Add(p1);
                meshData.positions.Add(p0);
                meshData.positions.Add(p1);
                meshData.positions.Add(p3);
            }
            else
            {
                meshData.positions.Add(p0);
                meshData.positions.Add(p1);
                meshData.positions.Add(p2);
                meshData.positions.Add(p0);
                meshData.positions.Add(p3);
                meshData.positions.Add(p1);
            }

            for (int t = 0; t < 6; t++)
            {
                meshData.normals.Add(normal);
                meshData.colors.Add(props.color);
                meshData.indices.Add(vi + t);
            }
        }
    }
}
