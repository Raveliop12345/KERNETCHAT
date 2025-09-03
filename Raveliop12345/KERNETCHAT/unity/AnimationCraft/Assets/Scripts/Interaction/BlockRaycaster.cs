using AnimationCraft.Core;
using UnityEngine;

namespace AnimationCraft.Interaction
{
    public struct BlockHit
    {
        public Vector3Int block;
        public Vector3Int normal;
        public Vector3 point;
        public bool valid;
    }

    public static class BlockRaycaster
    {
        public static BlockHit Raycast(Camera cam, float maxDist)
        {
            var ray = cam.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2));
            return Raycast(ray.origin, ray.direction, maxDist);
        }

        public static BlockHit Raycast(Vector3 origin, Vector3 dir, float maxDist)
        {
            dir.Normalize();
            Vector3 pos = origin;
            Vector3Int ip = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
            Vector3 deltaDist = new Vector3(
                Mathf.Abs(1f / (dir.x == 0 ? 1e-6f : dir.x)),
                Mathf.Abs(1f / (dir.y == 0 ? 1e-6f : dir.y)),
                Mathf.Abs(1f / (dir.z == 0 ? 1e-6f : dir.z))
            );

            Vector3Int step = new Vector3Int(
                dir.x >= 0 ? 1 : -1,
                dir.y >= 0 ? 1 : -1,
                dir.z >= 0 ? 1 : -1
            );

            Vector3 sideDist = new Vector3(
                ((dir.x >= 0 ? (ip.x + 1) - pos.x : pos.x - ip.x)) * deltaDist.x,
                ((dir.y >= 0 ? (ip.y + 1) - pos.y : pos.y - ip.y)) * deltaDist.y,
                ((dir.z >= 0 ? (ip.z + 1) - pos.z : pos.z - ip.z)) * deltaDist.z
            );

            float dist = 0f;
            Vector3Int normal = Vector3Int.zero;
            for (int i = 0; i < 256 && dist <= maxDist; i++)
            {
                if (sideDist.x < sideDist.y)
                {
                    if (sideDist.x < sideDist.z)
                    {
                        ip.x += step.x; dist = sideDist.x; sideDist.x += deltaDist.x; normal = new Vector3Int(-step.x, 0, 0);
                    }
                    else
                    {
                        ip.z += step.z; dist = sideDist.z; sideDist.z += deltaDist.z; normal = new Vector3Int(0, 0, -step.z);
                    }
                }
                else
                {
                    if (sideDist.y < sideDist.z)
                    {
                        ip.y += step.y; dist = sideDist.y; sideDist.y += deltaDist.y; normal = new Vector3Int(0, -step.y, 0);
                    }
                    else
                    {
                        ip.z += step.z; dist = sideDist.z; sideDist.z += deltaDist.z; normal = new Vector3Int(0, 0, -step.z);
                    }
                }

                if (dist > maxDist) break;
                // We cannot know occupancy here; WorldStreamer handles it.
                return new BlockHit { block = ip, normal = normal, point = origin + dir * dist, valid = true };
            }
            return new BlockHit { valid = false };
        }
    }
}
