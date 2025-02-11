using System;
using System.Runtime.InteropServices;

namespace LibPulseTune.Helpers
{
    internal static class WinApiHelper
    {
        /// <summary>
        /// DLLから指定された名前の関数の関数ポインタを取得し、delegateに変換して返す。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="functionName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T LoadFunction<T>(IntPtr pDll, string functionName)
        {
            // 関数ポインタを取得
            IntPtr functionPtr = WinApi.GetProcAddress(pDll, functionName);

            if (functionPtr == IntPtr.Zero)
            {
                throw new Exception($"指定されたハンドルのDLL内に、関数 {functionName} が見つかりませんでした。");
            }

            // 関数ポインタをdelegateに変換する
            return (T)(object)Marshal.GetDelegateForFunctionPointer(functionPtr, typeof(T));
        }
    }
}
