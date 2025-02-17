using System.IO;
using System.Text;

namespace LibPulseTune.Codecs.Utils
{
    internal static class FourCC
    {
        private static bool IsMatchNextBytes(this BinaryReader reader, byte[] data)
        {
            for (int i = 0; i < 4; i++)
            {
                if (data[i] != reader.ReadByte())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 指定された名前のチャンクに移動する。
        /// </summary>
        /// <param name="chunkName"></param>
        /// <returns></returns>
        public static bool MoveToChunk(this BinaryReader reader, string chunkName)
        {
            var data = Encoding.ASCII.GetBytes(chunkName);
            reader.BaseStream.Position = 0;

            while (reader.BaseStream.Position <= reader.BaseStream.Length)
            {
                if (reader.IsMatchNextBytes(data))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
