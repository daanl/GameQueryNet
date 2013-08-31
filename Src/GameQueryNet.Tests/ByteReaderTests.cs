using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace GameQueryNet.Tests
{
    [TestFixture]
    public class ByteReaderTests
    {
        [Test]
        public void ExtractString_Returns_Values()
        {
            var delimiter = Encoding.UTF8.GetBytes("|").First();
            var value = "Hoi alles| goed?";
            var bytes = Encoding.UTF8.GetBytes(value);

            using (var reader = new SteamByteReader(bytes))
            {
                var first = reader.ExtractString<string>(delimiter);
                var seccond = reader.ExtractString<string>(delimiter);

                Assert.AreEqual("Hoi alles", first);
                Assert.AreEqual(" goed?", seccond);
            }
        }

        [Test]
        public void ExtractShort_Returns_Values()
        {
            short[] values = { 12, 13 };
            var bytes = new List<byte>();

            foreach (var value in values)
            {
                bytes.AddRange(BitConverter.GetBytes(value));
            }

            using (var reader = new SteamByteReader(bytes.ToArray()))
            {
                var first = reader.ExtractShort<int>();
                var seccond = reader.ExtractShort<int>();

                Assert.AreEqual(12, first);
                Assert.AreEqual(13, seccond);
            }
        }

        [Test]
        public void ExtractLong_Returns_Values()
        {
            int[] values = { 12, 13 };
            var bytes = new List<byte>();

            foreach (var value in values)
            {
                bytes.AddRange(BitConverter.GetBytes(value));
            }

            using (var reader = new SteamByteReader(bytes.ToArray()))
            {
                var first = reader.ExtractLong<int>();
                var seccond = reader.ExtractLong<int>();

                Assert.AreEqual(12, first);
                Assert.AreEqual(13, seccond);
            }
        }

        [Test]
        public void ExtractByte_Returns_Values()
        {
            byte[] bytes = { 1, 22 };

            using (var reader = new SteamByteReader(bytes))
            {
                var first = reader.ExtractByte<int>();
                var seccond = reader.ExtractByte<int>();

                Assert.AreEqual(1, first);
                Assert.AreEqual(22, seccond);
            }
        }

        [Test]
        public void ExtractFloat_Returns_Values()
        {
            var values = new[] { 12.25f, 13.40f };
            var bytes = new List<byte>();

            foreach (var value in values)
            {
                bytes.AddRange(BitConverter.GetBytes(value));
            }

            using (var reader = new SteamByteReader(bytes.ToArray()))
            {
                var first = reader.ExtractFloat<float>();
                var seccond = reader.ExtractFloat<float>();

                Assert.AreEqual(12.25f, first);
                Assert.AreEqual(13.40f, seccond);
            }
        }

        [Test]
        public void ExtractLongLong_Returns_Values()
        {
            var values = new ulong[] { 1234567890123456789, 9876543210987654321 };
            var bytes = new List<byte>();

            foreach (var value in values)
            {
                bytes.AddRange(BitConverter.GetBytes(value));
            }

            using (var reader = new SteamByteReader(bytes.ToArray()))
            {
                var first = reader.ExtractLongLong<ulong>();
                var seccond = reader.ExtractLongLong<ulong>();

                Assert.AreEqual(1234567890123456789, first);
                Assert.AreEqual(9876543210987654321, seccond);
            }
        }

        [Test]
        public void GetString_Returns_Values()
        {
            var delimiter = Encoding.UTF8.GetBytes("|").First();
            var value = "Hoi alles| goed?";
            var bytes = Encoding.UTF8.GetBytes(value);

            using (var reader = new SteamByteReader(bytes))
            {
                var first = reader.GetString<string>(0, delimiter);
                var seccond = reader.GetString<string>(11, delimiter);

                Assert.AreEqual("Hoi alles", first);
                Assert.AreEqual("goed?", seccond);
            }
        }

        [Test]
        public void GetShort_Returns_Values()
        {
            short[] values = { 12, 13 };
            var bytes = new List<byte>();

            foreach (var value in values)
            {
                bytes.AddRange(BitConverter.GetBytes(value));
            }

            using (var reader = new SteamByteReader(bytes.ToArray()))
            {
                var first = reader.GetShort<int>(0);
                var seccond = reader.GetShort<int>(2);

                Assert.AreEqual(12, first);
                Assert.AreEqual(13, seccond);
            }
        }

        [Test]
        public void GetLong_Returns_Values()
        {
            int[] values = { 12, 13 };
            var bytes = new List<byte>();

            foreach (var value in values)
            {
                bytes.AddRange(BitConverter.GetBytes(value));
            }

            using (var reader = new SteamByteReader(bytes.ToArray()))
            {
                var first = reader.GetLong<int>(0);
                var seccond = reader.GetLong<int>(4);

                Assert.AreEqual(12, first);
                Assert.AreEqual(13, seccond);
            }
        }

        [Test]
        public void GetByte_Returns_Values()
        {
            byte[] bytes = { 1, 22 };

            using (var reader = new SteamByteReader(bytes))
            {
                var first = reader.GetByte<int>(0);
                var seccond = reader.GetByte<int>(1);

                Assert.AreEqual(1, first);
                Assert.AreEqual(22, seccond);
            }
        }

        [Test]
        public void GetFloat_Returns_Values()
        {
            var values = new[] { 12.25f, 13.40f };
            var bytes = new List<byte>();

            foreach (var value in values)
            {
                bytes.AddRange(BitConverter.GetBytes(value));
            }

            using (var reader = new SteamByteReader(bytes.ToArray()))
            {
                var first = reader.GetFloat<float>(0);
                var seccond = reader.GetFloat<float>(4);

                Assert.AreEqual(12.25f, first);
                Assert.AreEqual(13.40f, seccond);
            }
        }

        [Test]
        public void GetLongLong_Returns_Values()
        {
            var values = new ulong[] { 1234567890123456789, 9876543210987654321 };
            var bytes = new List<byte>();

            foreach (var value in values)
            {
                bytes.AddRange(BitConverter.GetBytes(value));
            }

            using (var reader = new SteamByteReader(bytes.ToArray()))
            {
                var first = reader.GetLongLong<ulong>(0);
                var seccond = reader.GetLongLong<ulong>(8);

                Assert.AreEqual(1234567890123456789, first);
                Assert.AreEqual(9876543210987654321, seccond);
            }
        }
    }
}
