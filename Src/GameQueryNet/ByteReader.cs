using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameQueryNet
{
    public class ByteReader
    {
        /// <summary>
        /// Steam doesn't have a length for string we have to determine that our self
        /// </summary>
        /// <returns>Index of the string delimiter</returns>
        public int GetIndexOfString(IList<byte> bytes)
        {
            return bytes.ToList().IndexOf(0x00);
        }

        public T ExtractString<T>(ref IList<byte> bytes, bool removedUsed = true)
        {
            var index = GetIndexOfString(bytes);
            var result = bytes.Take(index).ToArray();

            if (removedUsed)
            {
                bytes = bytes.Skip(index + 1).ToArray();
            }

            return ConvertToType<T>(result);
        }

        public T ExtractShort<T>(ref IList<byte> bytes)
        {
            var result = bytes.Take(2).ToArray();
            bytes = bytes.Skip(2).ToArray();

            return ConvertToType<T>(result);
        }

        /// <summary>
        /// Gets the most significant bit of the bytes
        /// see here for more methods or info: http://stackoverflow.com/questions/4854207/get-a-specific-bit-from-byte
        /// </summary>
        public bool GetMostSignificantBit(byte[] bytes)
        {
            var bitArray = new System.Collections.BitArray(bytes);
            return bitArray.Get(0); /* 0 based index */
        }

        public T ExtractLong<T>(ref IList<byte> bytes)
        {
            var result = bytes.Take(4).ToArray();
            bytes = bytes.Skip(4).ToArray();

            return ConvertToType<T>(result);
        }

        public T ExtractByte<T>(ref IList<byte> bytes)
        {
            var result = bytes.First();
            bytes = bytes.Skip(1).ToArray();

            return ConvertToType<T>(new[] { result });
        }

        public T ConvertToType<T>(byte[] value)
        {
            var type = typeof(T);

            if (type == typeof(string))
            {
                object result = Encoding.UTF8.GetString(value);
                return (T)result;
            }

            if (type == typeof(int))
            {
                object result = null;

                if (value.Length == 1)
                {
                    result = (int)value[0];
                }
                else if (value.Length == 2)
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(value);
                    }

                    result = Convert.ToInt32(BitConverter.ToInt16(value, 0));
                }
                else
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(value);
                    }

                    result = Convert.ToInt32(BitConverter.ToInt32(value, 0));
                }

                return (T)result;
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }

        public int ConvertToInt(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return BitConverter.ToInt16(bytes, 0);
        }
    }
}
