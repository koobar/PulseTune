using System;
using System.Threading;
using Windows.Foundation;

namespace LibPulseTune.Metadata.Utils
{
    internal static class AsyncUtils
    {
        /// <summary>
        /// 指定されたWinRTの非同期メソッドを同期的に呼び出す。
        /// </summary>
        /// <typeparam name="ReturnType">戻り値の型</typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static ReturnType CallAsyncMethod<ReturnType>(Func<IAsyncOperation<ReturnType>> func)
        {
            var result = func();
            bool ok = false;

            while (true)
            {
                if (result.Status == AsyncStatus.Completed)
                {
                    ok = true;
                    break;
                }
                else if (result.Status == AsyncStatus.Error)
                {
                    break;
                }

                Thread.Sleep(1);
            }

            if (ok)
            {
                return result.GetResults();
            }

            return default(ReturnType);
        }

        /// <summary>
        /// 指定されたWinRTの非同期メソッドを同期的に呼び出す。
        /// </summary>
        /// <typeparam name="ReturnType">戻り値の型</typeparam>
        /// <typeparam name="ParameterType1">引数1の型</typeparam>
        /// <param name="func"></param>
        /// <param name="p1"></param>
        /// <returns></returns>
        public static ReturnType CallAsyncMethod<ReturnType, ParameterType1>(Func<ParameterType1, IAsyncOperation<ReturnType>> func, ParameterType1 p1)
        {
            var result = func(p1);
            bool ok = false;

            while (true)
            {
                if (result.Status == AsyncStatus.Completed)
                {
                    ok = true;
                    break;
                }
                else if (result.Status == AsyncStatus.Error)
                {
                    break;
                }

                Thread.Sleep(1);
            }

            if (ok)
            {
                return result.GetResults();
            }

            return default(ReturnType);
        }

        /// <summary>
        /// 指定されたWinRTの非同期メソッドを同期的に呼び出す。
        /// </summary>
        /// <typeparam name="ReturnType">戻り値の型</typeparam>
        /// <typeparam name="ParameterType1">引数1の型</typeparam>
        /// <typeparam name="ParameterType2">引数2の型</typeparam>
        /// <param name="func"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static ReturnType CallAsyncMethod<ReturnType, ParameterType1, ParameterType2>(Func<ParameterType1, ParameterType2, IAsyncOperation<ReturnType>> func, ParameterType1 p1, ParameterType2 p2)
        {
            var result = func(p1, p2);
            bool ok = false;

            while (true)
            {
                if (result.Status == AsyncStatus.Completed)
                {
                    ok = true;
                    break;
                }
                else if (result.Status == AsyncStatus.Error)
                {
                    break;
                }

                Thread.Sleep(1);
            }

            if (ok)
            {
                return result.GetResults();
            }

            return default(ReturnType);
        }

        /// <summary>
        /// 指定されたWinRTの非同期メソッドを同期的に呼び出す。
        /// </summary>
        /// <typeparam name="ReturnType">戻り値の型</typeparam>
        /// <typeparam name="ParameterType1">引数1の型</typeparam>
        /// <typeparam name="ParameterType2">引数2の型</typeparam>
        /// <typeparam name="ParameterType3">引数3の型</typeparam>
        /// <param name="func"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static ReturnType CallAsyncMethod<ReturnType, ParameterType1, ParameterType2, ParameterType3>(Func<ParameterType1, ParameterType2, ParameterType3, IAsyncOperation<ReturnType>> func, ParameterType1 p1, ParameterType2 p2, ParameterType3 p3)
        {
            var result = func(p1, p2, p3);
            bool ok = false;

            while (true)
            {
                if (result.Status == AsyncStatus.Completed)
                {
                    ok = true;
                    break;
                }
                else if (result.Status == AsyncStatus.Error)
                {
                    break;
                }

                Thread.Sleep(1);
            }

            if (ok)
            {
                return result.GetResults();
            }

            return default(ReturnType);
        }
    }
}
