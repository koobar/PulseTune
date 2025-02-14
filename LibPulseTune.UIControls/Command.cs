using System;

namespace LibPulseTune.UIControls
{
    public class Command
    {
        // 非公開フィールド
        private bool canExecute;
        private Action<object, object> action;

        // イベント
        public event EventHandler ExecutableChanged;

        // コンストラクタ
        public Command(Action<object, object> action)
        {
            this.action = action;
            this.canExecute = true;
        }

        /// <summary>
        /// コマンドが実行可能であるかどうか
        /// </summary>
        public bool CanExecute
        {
            set
            {
                this.canExecute = value;
                this.ExecutableChanged?.Invoke(this, EventArgs.Empty);
            }
            get
            {
                return this.canExecute;
            }
        }

        /// <summary>
        /// コマンドを実行する。
        /// </summary>
        public void Execute(object arg1 = null, object arg2 = null)
        {
            if (!this.canExecute)
            {
                return;
            }

            this.action(arg1, arg2);
        }
    }
}
