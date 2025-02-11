using NAudio.Wave;
using System;
using System.Runtime.CompilerServices;

namespace LibPulseTune.Helpers
{
    internal unsafe static class BufferHelper
    {
        /// <summary>
        /// PCM <-> Float の相互変換に用いる係数のテーブル
        /// </summary>
        private static readonly long[] mTable = new long[5] { 0, 1 << 7, 1 << 16, 1 << 23, (long)(1 << 31) + 1 };

        /// <summary>
        /// 指定されたバッファの指定されたオフセット以降にサンプルを書き込む。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="sample"></param>
        public static void WriteSampleToBuffer(byte[] buffer, ref int offset, float sample, WaveFormat originalWaveFormat)
        {
            if (originalWaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
            {
                fixed (void* pBuffer = &buffer[offset])
                {
                    Unsafe.Write(pBuffer, sample);
                    offset += sizeof(float);
                }

                return;
            }

            int bytesPerSample = originalWaveFormat.BitsPerSample >> 3;
            long m = mTable[bytesPerSample];

            if (bytesPerSample == 1)
            {
                var pcm = (long)(sample * 128);
                pcm += m;
                buffer[offset++] = (byte)pcm;
            }
            else
            {
                fixed (void* pBuffer = &buffer[offset])
                {
                    var pcm = (long)(sample * m);
                    Buffer.MemoryCopy(&pcm, pBuffer, buffer.LongLength, bytesPerSample);
                    offset += bytesPerSample;
                }
            }
        }

        /// <summary>
        /// 指定されたバッファの指定されたオフセット以降から次の1サンプルを読み込む。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static float ReadSampleFromBuffer(byte[] buffer, ref int offset, WaveFormat originalWaveFormat)
        {
            int bytesPerSample = originalWaveFormat.BitsPerSample >> 3;

            if (originalWaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
            {
                float result = 0;
                fixed (byte* ptr = &buffer[offset])
                {
                    result = Unsafe.Read<float>(ptr);
                }

                offset += bytesPerSample;
                return result;
            }
            
            long m = mTable[bytesPerSample];
            long sample = 0;

            switch (bytesPerSample)
            {
                case 1:
                    sample = buffer[offset++] - m;
                    break;
                case 2:
                    fixed (byte* ptr = &buffer[offset])
                    {
                        sample = Unsafe.Read<short>(ptr);
                    }
                    break;
                case 3:
                    sample = (sbyte)buffer[offset + 2] << 16 | (buffer[offset + 1] << 8) | buffer[offset];
                    break;
                case 4:
                    m = int.MaxValue;
                    fixed (byte* ptr = &buffer[offset])
                    {
                        sample = Unsafe.Read<int>(ptr);
                    }
                    break;
            }

            offset += bytesPerSample;
            return sample / (float)m;
        }
    }
}
