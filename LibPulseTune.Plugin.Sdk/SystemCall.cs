using System;

namespace LibPulseTune.Plugin.Sdk
{
    public class SystemCall
    {
        // 非公開フィールド
        private readonly bool isFunction;
        private readonly int parameterCount;
        private readonly Action proc_p0;
        private readonly Action<object> proc_p1;
        private readonly Action<object, object> proc_p2;
        private readonly Action<object, object, object> proc_p3;
        private readonly Action<object, object, object, object> proc_p4;
        private readonly Func<object> func_p0;
        private readonly Func<object, object> func_p1;
        private readonly Func<object, object, object> func_p2;
        private readonly Func<object, object, object, object> func_p3;
        private readonly Func<object, object, object, object, object> func_p4;

        #region コンストラクタ

        public SystemCall(Action action)
        {
            this.proc_p0 = action;
            this.parameterCount = 0;
            this.isFunction = false;
        }

        public SystemCall(Action<object> action)
        {
            this.proc_p1 = action;
            this.parameterCount = 1;
            this.isFunction = false;
        }

        public SystemCall(Action<object, object> action)
        {
            this.proc_p2 = action;
            this.parameterCount = 2;
            this.isFunction = false;
        }

        public SystemCall(Action<object, object, object> action)
        {
            this.proc_p3 = action;
            this.parameterCount = 3;
            this.isFunction = false;
        }

        public SystemCall(Action<object, object, object, object> action)
        {
            this.proc_p4 = action;
            this.parameterCount = 4;
            this.isFunction = false;
        }

        public SystemCall(Func<object> func)
        {
            this.func_p0 = func;
            this.parameterCount = 0;
            this.isFunction = true;
        }

        public SystemCall(Func<object, object> func)
        {
            this.func_p1 = func;
            this.parameterCount = 1;
            this.isFunction = true;
        }

        public SystemCall(Func<object, object, object> func)
        {
            this.func_p2 = func;
            this.parameterCount = 2;
            this.isFunction = true;
        }

        public SystemCall(Func<object, object, object, object> func)
        {
            this.func_p3 = func;
            this.parameterCount = 3;
            this.isFunction = true;
        }

        public SystemCall(Func<object, object, object, object, object> func)
        {
            this.func_p4 = func;
            this.parameterCount = 4;
            this.isFunction = true;
        }

        #endregion

        public object Call(params object[] parameters)
        {
            if (parameters.Length != this.parameterCount)
            {
                throw new ArgumentException($"システムコールが要求する引数の数と、実際に与えられた引数の数が一致しません。要求された引数の数 {this.parameterCount} 個に対して、{parameters.Length} 個の引数が与えられました。");
            }

            if (this.isFunction)
            {
                switch (this.parameterCount)
                {
                    case 0:
                        return this.func_p0();
                    case 1:
                        return this.func_p1(parameters[0]);
                    case 2:
                        return this.func_p2(parameters[0], parameters[1]);
                    case 3:
                        return this.func_p3(parameters[0], parameters[1], parameters[2]);
                    case 4:
                        return this.func_p4(parameters[0], parameters[1], parameters[2], parameters[3]);
                    default:
                        throw new ArgumentException("システムコールの引数が多すぎます。");
                }
            }
            else
            {
                switch (this.parameterCount)
                {
                    case 0:
                        this.proc_p0();
                        break;
                    case 1:
                        this.proc_p1(parameters[0]);
                        break;
                    case 2:
                        this.proc_p2(parameters[0], parameters[1]);
                        break;
                    case 3:
                        this.proc_p3(parameters[0], parameters[1], parameters[2]);
                        break;
                    case 4:
                        this.proc_p4(parameters[0], parameters[1], parameters[2], parameters[3]);
                        break;
                    default:
                        throw new ArgumentException("システムコールの引数が多すぎます。");
                }

                return null;
            }
        }
    }
}
