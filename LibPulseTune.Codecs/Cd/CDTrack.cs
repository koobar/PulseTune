namespace LibPulseTune.Codecs.Cd
{
    public class CDTrack
    {
        // 非公開フィールド
        private readonly uint trackNumber;
        private readonly uint startSector;
        private readonly uint endSector;

        // コンストラクタ
        public CDTrack(uint trackNumber, uint start, uint end)
        {
            this.trackNumber = trackNumber;
            this.startSector = start;
            this.endSector = end;
        }

        /// <summary>
        /// トラック番号
        /// </summary>
        public uint TrackNumber
        {
            get
            {
                return this.trackNumber;
            }
        }

        /// <summary>
        /// 開始セクタを取得する。
        /// </summary>
        /// <returns></returns>
        public uint GetStartSector()
        {
            return this.startSector;
        }

        /// <summary>
        /// 終了セクタを取得する。
        /// </summary>
        /// <returns></returns>
        public uint GetEndSector()
        {
            return this.endSector;
        }
    }
}
