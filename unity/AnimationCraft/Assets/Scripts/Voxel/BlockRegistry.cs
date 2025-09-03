using System.Collections.Generic;
using UnityEngine;

namespace AnimationCraft.Voxel
{
    public struct BlockProps
    {
        public bool opaque;
        public Color32 color;
    }

    public static class BlockRegistry
    {
        static readonly Dictionary<BlockId, BlockProps> props = new Dictionary<BlockId, BlockProps>
        {
            { BlockId.Air, new BlockProps{ opaque = false, color = new Color32(0,0,0,0) } },
            { BlockId.Bedrock, new BlockProps{ opaque = true, color = new Color32(0x22,0x22,0x22,0xFF) } },
            { BlockId.Stone, new BlockProps{ opaque = true, color = new Color32(0x88,0x88,0x88,0xFF) } },
            { BlockId.Dirt, new BlockProps{ opaque = true, color = new Color32(0x6B,0x4E,0x16,0xFF) } },
            { BlockId.Grass, new BlockProps{ opaque = true, color = new Color32(0x3C,0xAB,0x3C,0xFF) } },
            { BlockId.Sand, new BlockProps{ opaque = true, color = new Color32(0xD8,0xD1,0x9B,0xFF) } }
        };

        public static BlockProps Get(BlockId id)
        {
            return props[id];
        }

        public static bool IsOpaque(BlockId id)
        {
            if (id == BlockId.Air) return false;
            return props[id].opaque;
        }
    }
}
