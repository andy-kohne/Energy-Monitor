using System;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Brultech.ECM1240.Tests
{
    public class ByteBufferTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private static byte[] tenBytes;
        private static byte[] fiftybytes;
        private static byte[] twohundredbytes;


        public ByteBufferTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            tenBytes = Enumerable.Range(1, 10).Select(Convert.ToByte).ToArray();
            fiftybytes = Enumerable.Range(1, 50).Select(Convert.ToByte).ToArray();
            twohundredbytes = Enumerable.Range(1, 200).Select(Convert.ToByte).ToArray();

        }
        [Fact]
        public void InitializedToZero()
        {
            var b = new ByteBuffer(256);
            Assert.Equal(0, b.Size);
        }

        [Fact]
        public void AddTen()
        {
            var b = new ByteBuffer(256);
            b.Enqueue(tenBytes, 0, 10);
            Assert.Equal(10, b.Size);
        }


        [Fact]
        public void AddTenGetTen()
        {
            var b = new ByteBuffer(256);
            b.Enqueue(tenBytes, 0, 10);

            var result = b.Get(10);

            Assert.Equal(10, result.Length);
            Assert.True(tenBytes.SequenceEqual(result));
        }


        [Fact]
        public void AddTenDequeueTen()
        {
            var b = new ByteBuffer(256);
            b.Enqueue(tenBytes, 0, 10);
            Assert.Equal(10, b.Size);

            var result = new byte[10];
            b.Dequeue( result, 10);

            Assert.Equal(10, result.Length);
            Assert.True(tenBytes.SequenceEqual(result));
            Assert.Equal(0, b.Size);
        }

        [Fact]
        public void AddTenDequeueFive()
        {
            var b = new ByteBuffer(256);
            b.Enqueue(tenBytes, 0, 10);
            Assert.Equal(10, b.Size);

            var result = new byte[5];
            b.Dequeue( result, 5);
            Assert.Equal(5, b.Size);
            b.Dequeue( result, 5);

            Assert.Equal(5, result.Length);
            Assert.True(tenBytes.Skip(5).SequenceEqual(result));
            Assert.Equal(0, b.Size);
        }


        [Fact]
        public void TestWrap()
        {
            var b = new ByteBuffer(256);
            b.Enqueue(Enumerable.Range(0,250).Select(Convert.ToByte).ToArray(), 0, 250);
            
            Assert.Equal(250, b.Size);

            var result = new byte[240];
            b.Dequeue(result, 240);
            Assert.True(Enumerable.Range(0, 240).Select(Convert.ToByte).SequenceEqual(result));

            Assert.Equal(10, b.Size);

            b.Enqueue(Enumerable.Range(0,10).Select(Convert.ToByte).ToArray(), 0, 10);
            Assert.Equal(20, b.Size);

            _testOutputHelper.WriteLine(b._internalBuffer.Aggregate("", (s, b1) => s + "," + b1).Substring(1));

            var result2 = new byte[20];
            b.Dequeue(result2, 20);

            _testOutputHelper.WriteLine(result2.Aggregate("", (s, b1) => s + "," + b1).Substring(1));
            Assert.True(Enumerable.Range(240, 10).Concat(Enumerable.Range(0,10)).Select(Convert.ToByte).SequenceEqual(result2));

            Assert.Equal(0, b.Size);
        }



        [Fact]
        public void Add260()
        {
            var b = new ByteBuffer(256);
            var source = new byte[260];
            Assert.Throws<InternalBufferOverflowException>(() => b.Enqueue(source, 0, 260));
        }


        [Fact]
        public void Add260GetTen()
        {
            var b = new ByteBuffer(256);
            b.Enqueue(Enumerable.Range(0, 252).Select(Convert.ToByte).ToArray(), 0, 252);
            var temp = new byte[250];
            b.Dequeue(temp, 250);
            b.Enqueue(tenBytes, 0, 10);
            //Assert.Equal(256, b.Size);

            _testOutputHelper.WriteLine($"head: {b._head}");
            _testOutputHelper.WriteLine($"tail: {b._tail}");
            _testOutputHelper.WriteLine(b._internalBuffer.Aggregate("", (s, b1) => s + "," + b1).Substring(1));

            var result = new byte[12];
            b.Dequeue(result, 12);

            //var result = b.Get(10);
            _testOutputHelper.WriteLine(result.Aggregate("", (s, b1) => s + "," + b1).Substring(1));
            //Assert.Equal(14, result.Length);
            Assert.True(Enumerable.Range(250, 2).Concat(Enumerable.Range(1, 10)).Select(Convert.ToByte).SequenceEqual(result));
        }

        [Fact]
        public void Wrap_Indexed()
        {
            var b = new ByteBuffer(256);
            b.Enqueue(Enumerable.Range(0, 252).Select(Convert.ToByte).ToArray(), 0, 252);
            var temp = new byte[250];
            b.Dequeue( temp, 250);
            b.Enqueue(tenBytes, 0, 10);

            Assert.Equal(250, b[0]);
            Assert.Equal(251, b[1]);
            Assert.Equal(1, b[2]);
            Assert.Equal(2, b[3]);
            Assert.Equal(3, b[4]);
            Assert.Equal(4, b[5]);
            Assert.Equal(5, b[6]);
            Assert.Equal(6, b[7]);
            //Assert.Equal(256, b.Size);

            _testOutputHelper.WriteLine($"head: {b._head}");
            _testOutputHelper.WriteLine($"tail: {b._tail}");
            _testOutputHelper.WriteLine(b._internalBuffer.Aggregate("", (s, b1) => s + "," + b1).Substring(1));

            var result = new byte[12];
            b.Dequeue( result, 12);

            //var result = b.Get(10);
            _testOutputHelper.WriteLine(result.Aggregate("", (s, b1) => s + "," + b1).Substring(1));
            //Assert.Equal(14, result.Length);
            Assert.True(Enumerable.Range(250, 2).Concat(Enumerable.Range(1, 10)).Select(Convert.ToByte).SequenceEqual(result));
        }


        [Fact]
        public void Add256GetTen()
        {
            var b = new ByteBuffer(256);
            b.Enqueue(twohundredbytes, 0, 200);
            b.Enqueue(fiftybytes, 0, 50);
            b.Enqueue(tenBytes, 0, 6);
            Assert.Equal(256, b.Size);

            _testOutputHelper.WriteLine($"head: {b._head}");
            _testOutputHelper.WriteLine($"tail: {b._tail}");
            _testOutputHelper.WriteLine(b._internalBuffer.Aggregate("", (s, b1) => s + "," + b1).Substring(1));
            var result = b.Get(10);
            _testOutputHelper.WriteLine(result.Aggregate("", (s, b1) => s + "," + b1).Substring(1));
            Assert.Equal(10, result.Length);
            Assert.True(Enumerable.Range(1, 10).Select(Convert.ToByte).SequenceEqual(result));
        }


    }
}
