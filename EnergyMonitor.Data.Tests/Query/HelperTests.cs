using EnergyMonitor.Data.Query;
using System;
using System.Collections.Generic;
using Xunit;

namespace EnergyMonitor.Data.Tests.Query
{
    public class HelperTests
    {
        [Theory, MemberData(nameof(FindSplit_TestCases))]
        public void Test_FindSplit(DateTime referenceDate, DateTime a, DateTime b, decimal? expected)
        {
            var result = referenceDate.FindSplit(a, b);
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> FindSplit_TestCases => new List<object[]>
        {
            new object[]
            {
                new DateTime(2022, 1, 1,6,0,0),
                new DateTime(2022, 1, 1,7,0,0),
                new DateTime(2022, 1, 1, 9,0,0),
                null
            },

            new object[]
            {
                new DateTime(2022, 1, 1,8,0,0),
                new DateTime(2022, 1, 1,7,0,0),
                new DateTime(2022, 1, 1, 9,0,0),
                0.5M
            },

            new object[]
            {
                new DateTime(2022, 1, 1,1,0,0),
                new DateTime(2022, 1, 1,0,0,0),
                new DateTime(2022, 1, 1, 10,0,0),
                0.1M
            },

            new object[]
            {
                new DateTime(2022, 07, 1),
                new DateTime(2022, 1, 1),
                new DateTime(2022, 12, 31, 23, 59, 59, 999),
                0.4958904109746286913677901031M
            },
        };


        [Theory, MemberData(nameof(FindMidPoint_Uint_TestCases))]
        public void Test_FindMidPointUint(decimal split, uint a, uint b, uint expected)
        {
            var result = split.FindMidPoint(a, b);
            Assert.Equal(expected, result);
        }
        public static IEnumerable<object[]> FindMidPoint_Uint_TestCases => new List<object[]>
        {
            new object[] { .5m, 1, 3, 2 },
            new object[] { .1m, 0, 10, 1 },
            new object[] { .1m, 10, 0, 9 },

        };


        [Theory, MemberData(nameof(FindMidPoint_Ulong_TestCases))]
        public void Test_FindMidPointUlong(decimal split, ulong a, ulong b, ulong expected)
        {
            var result = split.FindMidPoint(a, b);
            Assert.Equal(expected, result);
        }
        public static IEnumerable<object[]> FindMidPoint_Ulong_TestCases => new List<object[]>
        {
            new object[] { .5m, 1, 3, 2 },
            new object[] { .1m, 0, 10, 1 },
            new object[] { .1m, 10, 0, 9 },

        };
    }
}
