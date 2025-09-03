using System.IO;
using Unity.Collections;

namespace AnimationCraft.Save
{
    public static class ChunkSerializer
    {
        public static void WriteRLE(BinaryWriter bw, NativeArray<byte> data)
        {
            int n = data.Length;
            int i = 0;
            while (i < n)
            {
                byte val = data[i];
                int run = 1;
                while (i + run < n && data[i + run] == val && run < 65535)
                    run++;
                bw.Write(val);
                bw.Write((ushort)run);
                i += run;
            }
        }

        public static void ReadRLE(BinaryReader br, NativeArray<byte> data)
        {
            int i = 0;
            while (i < data.Length && br.BaseStream.Position < br.BaseStream.Length)
            {
                byte val = br.ReadByte();
                ushort run = br.ReadUInt16();
                for (int k = 0; k < run && i < data.Length; k++)
                    data[i++] = val;
            }
        }
    }
}
