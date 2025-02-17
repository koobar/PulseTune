using System.IO;

namespace LibPulseTune.Codecs.Utils
{
    internal static class BigEndian
    {
        #region ビッグエンディアンの変換用メソッド

        /// <summary>
        /// ビッグエンディアンのバイト配列を16ビット整数に変換する。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static ushort ToUInt16(byte[] buffer, int offset)
        {
            return (ushort)((buffer[offset + 0] << 8) | buffer[offset + 1]);
        }

        /// <summary>
        /// ビッグエンディアンのバイト配列を符号付き16ビット整数に変換する。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static short ToInt16(byte[] buffer, int offset)
        {
            return (short)((buffer[offset + 0] << 8) | buffer[offset + 1]);
        }

        /// <summary>
        /// ビッグエンディアンのバイト配列を32ビット整数に変換する。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static uint ToUInt32(byte[] buffer, int offset)
        {
            return (uint)((buffer[offset + 0] << 24) | (buffer[offset + 1] << 16) | (buffer[offset + 2] << 8) | buffer[offset + 3]);
        }

        /// <summary>
        /// ビッグエンディアンのバイト配列を符号付き32ビット整数に変換する。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static int ToInt32(byte[] buffer, int offset)
        {
            return (buffer[offset + 0] << 24) | (buffer[offset + 1] << 16) | (buffer[offset + 2] << 8) | buffer[offset + 3];
        }

        #endregion

        #region BinaryReaderのビッグエンディアン用の拡張メソッド

        /// <summary>
        /// ストリームからビッグエンディアンの16ビット整数を読み込む。
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static ushort ReadBigEndianUInt16(this BinaryReader reader)
        {
            return ToUInt16(reader.ReadBytes(sizeof(ushort)), 0);
        }

        /// <summary>
        /// ストリームからビッグエンディアンの16ビット符号付き整数を読み込む。
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static short ReadBigEndianInt16(this BinaryReader reader)
        {
            return ToInt16(reader.ReadBytes(sizeof(short)), 0);
        }

        /// <summary>
        /// ストリームからビッグエンディアンの32ビット整数を読み込む。
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static uint ReadBigEndianUInt32(this BinaryReader reader)
        {
            return ToUInt32(reader.ReadBytes(sizeof(uint)), 0);
        }

        /// <summary>
        /// ストリームからビッグエンディアンの32ビット符号付き整数を読み込む。
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static int ReadBigEndianInt32(this BinaryReader reader)
        {
            return ToInt32(reader.ReadBytes(sizeof(uint)), 0);
        }

        #endregion
    }
}
