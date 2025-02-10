using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PulseTune
{
    internal class OptionCollection
    {
        // 非公開フィールド
        private readonly Dictionary<string, string> options;

        // コンストラクタ
        public OptionCollection()
        {
            this.options = new Dictionary<string, string>();
        }

        /// <summary>
        /// 指定されたパスのファイルから設定を読み込む。
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            using (var reader = new StreamReader(path, Encoding.UTF8))
            {
                while (reader.Peek() > -1)
                {
                    string line = reader.ReadLine();
                    var tokens = line.Split(new char[] { '=' }, 2);
                    var key = tokens[0];
                    var value = tokens[1];

                    if (this.options.ContainsKey(key))
                    {
                        this.options[key] = value;
                    }
                    else
                    {
                        this.options.Add(key, value);
                    }
                }

                // 後始末
                reader.Close();
            }
        }

        /// <summary>
        /// 指定されたパスのファイルに設定を保存する。
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (var writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                foreach (var key in this.options.Keys)
                {
                    writer.WriteLine($"{key}={this.options[key]}");
                }

                writer.Flush();
                writer.Close();
            }
        }

        /// <summary>
        /// 指定されたキーの設定が存在するかどうか判定する。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return this.options.ContainsKey(key);
        }

        /// <summary>
        /// 設定を追加する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue(string key, object value)
        {
            if (this.options.ContainsKey(key))
            {
                this.options[key] = value.ToString();
            }
            else
            {
                this.options.Add(key, value.ToString());
            }
        }

        public dynamic GetValue(string key, object defaultValue, Type valueType)
        {
            string value = null;

            if (Contains(key))
            {
                value = this.options[key];
            }
            else
            {
                return defaultValue;
            }

            if (valueType.IsEnum)
            {
                return Enum.Parse(valueType, value);
            }

            if (valueType == typeof(string))
            {
                return value;
            }
            else if (valueType == typeof(bool))
            {
                return Convert.ToBoolean(value);
            }
            else if (valueType == typeof(byte))
            {
                return Convert.ToByte(value);
            }
            else if (valueType == typeof(sbyte))
            {
                return Convert.ToSByte(value);
            }
            else if (valueType == typeof(short))
            {
                return Convert.ToInt16(value);
            }
            else if (valueType == typeof(int))
            {
                return Convert.ToInt32(value);
            }
            else if (valueType == typeof(long))
            {
                return Convert.ToInt64(value);
            }
            else if (valueType == typeof(ushort))
            {
                return Convert.ToUInt16(value);
            }
            else if (valueType == typeof(uint))
            {
                return Convert.ToUInt32(value);
            }
            else if (valueType == typeof(ulong))
            {
                return Convert.ToUInt64(value);
            }
            else if (valueType == typeof(float))
            {
                return Convert.ToSingle(value);
            }
            else if (valueType == typeof(double))
            {
                return Convert.ToDouble(value);
            }

            return value;
        }

        /// <summary>
        /// 設定を取得する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public dynamic GetValue<T>(string key, T defaultValue)
        {
            return GetValue(key, defaultValue, typeof(T));
        }
    }
}
