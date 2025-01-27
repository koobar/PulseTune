namespace LibPulseTune.Plugin.Sdk
{
    public struct APILevel
    {
        public uint Level;
        public uint Revision;

        // コンストラクタ
        public APILevel(uint level, uint revision)
        {
            this.Level = level;
            this.Revision = revision;
        }

        /// <summary>
        /// 指定されたAPIレベルが、このAPIレベルと互換性があるかどうかを判定します。
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool IsCompatible(APILevel level)
        {
            // APIレベルのレベル番号が一致していなければ、与えられたAPIレベルのレベルが新しくても互換性なしとみなす。
            // （APIレベルのレベル番号が変わるほどの更新では、APIの廃止などを行う可能性があるため）
            if (level.Level == this.Level)
            {
                // 与えられたレベルのリビジョンより、このレベルのリビジョンのほうが新しければ互換性ありとみなす。
                // （リビジョンが変わっても、レベル番号が同じであれば、古いAPIの廃止は保留するため）
                if (level.Revision <= this.Revision)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
