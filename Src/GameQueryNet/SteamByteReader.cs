using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameQueryNet
{
    public class SteamByteReader : IDisposable
    {
        private byte[] _bytes;
        private int _extractIndex = 0;

        public SteamByteReader(byte[] bytes)
        {
            _bytes = bytes;
        }

        public byte[] Bytes
        {
            get { return _bytes; }
        }

        public byte[] ExtractedBytesLeft
        {
            get { return _bytes.Skip(_extractIndex).ToArray(); }
        }

        private byte[] ExtractBytes(int amount)
        {
            var extractBytes = ExtractedBytesLeft.Take(amount).ToArray();
            _extractIndex += amount;

            return extractBytes;
        }

        private byte[] GetBytes(int startIndex, int amount)
        {
            return Bytes.Skip(startIndex).Take(amount).ToArray();
        }

        /// <summary>
        /// Gets the most significant bit of the bytes
        /// see here for more methods or info: http://stackoverflow.com/questions/4854207/get-a-specific-bit-from-byte
        /// </summary>
        public bool GetMostSignificantBit(byte[] bytes)
        {
            var bitArray = new System.Collections.BitArray(bytes);
            return bitArray.Get(0); // zero based index 
        }

        /// <summary>
        /// Steam doesn't have a length for string we have to determine that our self
        /// </summary>
        /// <returns>Index of the string delimiter</returns>
        private int GetIndexOfString(IList<byte> bytes, byte delimiter)
        {
            return bytes.ToList().IndexOf(delimiter);
        }

        public T ExtractString<T>(byte delimiter = 0x00)
        {
            var index = GetIndexOfString(ExtractedBytesLeft, delimiter);

            if (index == -1)
            {
                index = ExtractedBytesLeft.Length;
            }

            var result = ExtractBytes(index);
            _extractIndex += 1;

            return ConvertToType<T>(result);
        }

        public T GetString<T>(int startIndex, byte delimiter)
        {
            var index = GetIndexOfString(Bytes, delimiter);

            if (index == -1)
            {
                index = Bytes.Length;
            }

            var result = GetBytes(startIndex, index);

            return ConvertToType<T>(result);
        }

        public T ExtractShort<T>()
        {
            var result = ExtractBytes(2);

            return ConvertToType<T>(result);
        }

        public T GetShort<T>(int startIndex)
        {
            var result = GetBytes(startIndex, 2);

            return ConvertToType<T>(result);
        }

        public T ExtractLong<T>()
        {
            var result = ExtractBytes(4);

            return ConvertToType<T>(result);
        }

        public T GetLong<T>(int startIndex)
        {
            var result = GetBytes(startIndex, 4);

            return ConvertToType<T>(result);
        }

        public T ExtractLongLong<T>()
        {
            var result = ExtractBytes(8);

            return ConvertToType<T>(result);
        }

        public T GetLongLong<T>(int startIndex)
        {
            var result = GetBytes(startIndex, 8);

            return ConvertToType<T>(result);
        }

        public T ExtractFloat<T>()
        {
            var result = ExtractBytes(4);

            return ConvertToType<T>(result);
        }

        public T GetFloat<T>(int startIndex)
        {
            var result = GetBytes(startIndex, 4);

            return ConvertToType<T>(result);
        }

        public T ExtractByte<T>()
        {
            var result = ExtractBytes(1);

            return ConvertToType<T>(result);
        }

        public T GetByte<T>(int startIndex)
        {
            var result = GetBytes(startIndex, 1);

            return ConvertToType<T>(result);
        }

        private dynamic ConvertToType<T>(byte[] value)
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

                    result = Convert.ToInt32(BitConverter.ToInt16(value, 0));
                }
                else
                {
                    result = Convert.ToInt32(BitConverter.ToInt32(value, 0));
                }

                return result;
            }

            if (type == typeof (float))
            {
                return BitConverter.ToSingle(value, 0);
            }

            if (type == typeof (ulong))
            {
                return BitConverter.ToUInt64(value, 0);
            }

            return Convert.ChangeType(value, typeof(T));
        }

        public void Dispose()
        {
            _bytes = null;
        }
    }
}
