using System;
using System.Linq;
using Xunit;

namespace Brultech.ECM1240.Tests
{
    public class HelperTests
    {
        [Fact]
        public void Test_GetBigEndianUShort()
        {
            for (ushort x = 0; x < ushort.MaxValue; x++)
            {
                var bigEndianBytes = BitConverter.GetBytes(x).AsEnumerable().Reverse().ToArray();
                var y = bigEndianBytes.GetBigEndianUShort(0);
                Assert.Equal(x, y);
            }

        }
    }
}
