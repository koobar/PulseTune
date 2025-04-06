namespace PulseTune
{
    public static class FormatCodes
    {
        // 0から始まるフォーマットコードは、オーディオトラックのコーデックの種類とする。
        public const uint CODE_AAC = 0x00000001;                // Advances Audio Coding
        public const uint CODE_FLAC = 0x00000002;               // Free Lossless Audio Codec
        public const uint CODE_MP2 = 0x00000003;                // MP2
        public const uint CODE_MP3 = 0x00000004;                // MP3
        public const uint CODE_M4A = 0x00000005;                // M4A
        public const uint CODE_WMA = 0x00000006;                // Windows Media Audio
        public const uint CODE_AIFF = 0x00000007;               // AIFF
        public const uint CODE_VORBIS = 0x00000008;             // OGG Vorbis
        public const uint CODE_OPUS = 0x00000009;               // Opus
        public const uint CODE_WAV = 0x0000000A;                // WAV
        public const uint CODE_CDA = 0x0000000B;                // Audio CD Track
        public const uint CODE_APE = 0x0000000C;                // Monkey's Audio
        public const uint CODE_WV = 0x0000000D;                 // WavPack
        public const uint CODE_ZPX = 0x0000000E;                // ZilophiX

        // 1から始まるフォーマットコードは、プレイリストファイルの種類とする。
        public const uint CODE_M3U = 0x10000001;
    }
}
