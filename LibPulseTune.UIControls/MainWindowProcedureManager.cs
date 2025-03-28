using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LibPulseTune.UIControls
{
    /// <summary>
    /// メインウィンドウのウィンドウプロシージャで処理するメッセージの登録と登録解除を行うためのクラス
    /// </summary>
    public static class MainWindowProcMgr
    {
        // 非公開フィールド
        private static Dictionary<int, List<Action<IntPtr, IntPtr>>> messages = new Dictionary<int, List<Action<IntPtr, IntPtr>>>();

        /// <summary>
        /// 指定されたウィンドウメッセージに対応するアクションをすべて取得する。
        /// </summary>
        /// <param name="message">ウィンドウメッセージ</param>
        /// <returns>対応するアクション</returns>
        public static IEnumerable<Action<IntPtr, IntPtr>> GetActions(int message)
        {
            if (!messages.ContainsKey(message))
            {
                return null;
            }

            return messages[message];
        }

        /// <summary>
        /// 指定されたウィンドウメッセージに対応するアクションをすべて取得する。
        /// </summary>
        /// <param name="message">ウィンドウメッセージ</param>
        /// <returns>対応するアクション</returns>
        public static IEnumerable<Action<IntPtr, IntPtr>> GetActions(Message message)
        {
            return GetActions(message.Msg);
        }

        /// <summary>
        /// 登録されたアクションのうち、指定されたウィンドウメッセージに対応するアクションの数を取得する。
        /// </summary>
        /// <param name="message">ウィンドウメッセージ</param>
        /// <returns>対応するアクションの数</returns>
        public static int GetRegisteredActionCount(int message)
        {
            if (messages.ContainsKey(message))
            {
                return messages[message].Count;
            }

            return 0;
        }

        /// <summary>
        /// 登録されたアクションのうち、指定されたウィンドウメッセージに対応するアクションの数を取得する。
        /// </summary>
        /// <param name="message">ウィンドウメッセージ</param>
        /// <returns>対応するアクションの数</returns>
        public static int GetRegisteredActionCount(Message message)
        {
            return GetRegisteredActionCount(message.Msg);
        }

        /// <summary>
        /// 指定されたアクションを、指定されたウィンドウメッセージに対応するアクションとして登録する。
        /// </summary>
        /// <param name="message">ウィンドウメッセージ</param>
        /// <param name="action">アクション</param>
        public static void RegisterAction(int message, Action<IntPtr, IntPtr> action)
        {
            if (!messages.ContainsKey(message))
            {
                messages.Add(message, new List<Action<IntPtr, IntPtr>>());
            }

            if (messages[message].Contains(action))
            {
                return;
            }

            messages[message].Add(action);
        }

        /// <summary>
        /// 指定されたウィンドウメッセージに対応するアクションとして登録された、指定されたアクションの登録を解除する。
        /// </summary>
        /// <param name="message">ウィンドウメッセージ</param>
        /// <param name="action">登録を解除するアクション</param>
        public static void UnregisterAction(int message, Action<IntPtr, IntPtr> action)
        {
            if (!messages.ContainsKey(message))
            {
                return;
            }

            messages[message].Remove(action);
            
            if (messages[message].Count == 0)
            {
                messages.Remove(message);
            }
        }

        /// <summary>
        /// 指定されたウィンドウメッセージに対応するアクションとして登録された、指定されたアクションの登録を解除する。
        /// </summary>
        /// <param name="message">ウィンドウメッセージ</param>
        /// <param name="action">登録を解除するアクション</param>
        public static void UnregisterAction(Message message, Action<IntPtr, IntPtr> action)
        {
            UnregisterAction(message.Msg, action);
        }
    }
}
