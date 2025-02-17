using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LibPulseTune.UIControls.Dialogs
{
    public class FolderPickerDialog : IDisposable
    {
        // 非公開フィールド
        private readonly CommonOpenFileDialog dialog;

        // コンストラクタ
        public FolderPickerDialog()
        {
            this.dialog = new CommonOpenFileDialog();
            this.dialog.IsFolderPicker = true;
            
        }

        public IEnumerable<string> FileNames
        {
            get
            {
                return this.dialog.FileNames;
            }
        }

        public string FileName
        {
            get
            {
                return this.dialog.FileName;
            }
        }

        /// <summary>
        /// 複数選択モード
        /// </summary>
        public bool Multiselect
        {
            set
            {
                this.dialog.Multiselect = value;
            }
            get
            {
                return this.dialog.Multiselect;
            }
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            this.dialog.Dispose();
        }

        /// <summary>
        /// ダイアログを表示する。
        /// </summary>
        /// <returns></returns>
        public DialogResult ShowDialog()
        {
            return (DialogResult)this.dialog.ShowDialog();
        }
    }
}
