namespace LibPulseTune.Codecs.Cd
{
    internal static class CDDA
    {
        // CDDAのセクタ関連の仕様に関する定義
        public const int CB_CDROM_SECTOR = 2048;        // CD-ROMのセクタサイズ
        public const int CB_CDDA_AUDIO = 2352;          // CD-DAが1秒間に読み込むセクタ数。セクタのバイト数は、(SECTOR_SIZE * RAW_SECTOR_SIZE)で計算可能

        // CDDAのPCMフォーマットの定義
        public const uint PCM_SAMPLE_RATE = 44100;                                                                          // 音声のサンプリング周波数
        public const uint PCM_BITS_PER_SAMPLE = 16;                                                                         // 音声の量子化ビット数
        public const uint PCM_CHANNELS = 2;                                                                                 // 音声のチャンネル数
        public const uint PCM_BYTES_PER_SAMPLE = PCM_BITS_PER_SAMPLE / 8;                                                   // サンプルあたりのバイト数
        public const uint PCM_BYTES_PER_MILLISECOND = PCM_BYTES_PER_SAMPLE * (PCM_SAMPLE_RATE / 1000) * PCM_CHANNELS;       // 1ミリ秒あたりのバイト数

        /// <summary>
        /// アドレスからセクタを算出する。
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static uint ComputeSectorFromAddress(byte[] address)
        {
            uint sectors = address[1] * 75u * 60u + address[2] * 75u + address[3];
            uint result = sectors - 150;

            return result;
        }
    }
}
