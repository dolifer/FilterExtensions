using System.Linq;
using FilterExtensions.Constants;
using FilterExtensions.Filters;
using Xunit;

namespace FilterExtensions.Tests
{
    public class FilterTests
    {
        [Theory]
        [MemberData(nameof(GetFilterCases))]
        public void Can_Filter_Array(Filter filter, int[] expected)
        {
            // arrange
            var source = new[] {1, 2, 3}.AsQueryable();

            // act
            var result = source.Where(filter).ToArray();

            // assert
            Assert.Equal(expected, result);
        }

        public static TheoryData<Filter, int[]> GetFilterCases()
        {
            var data = new TheoryData<Filter, int[]>
            {
                {
                    null,
                    new []{ 1, 2, 3}
                },
                {
                    new Filter
                    {
                        Operator = Operators.Equal,
                        Value = 1
                    },
                    new[] {1}
                },
                {
                    new Filter
                    {
                        Operator = Operators.LessThan,
                        Value = 2
                    },
                    new[] {1}
                },
                {
                    new Filter
                    {
                        Operator = Operators.GreaterThan,
                        Value = 1
                    },
                    new[] {2,3}
                },
                {
                    new Filter
                    {
                        Operator = Operators.LessThanOrEqual,
                        Value = 2
                    },
                    new[] {1, 2}
                },
                {
                    new Filter
                    {
                        Operator = Operators.GreaterThanOrEqual,
                        Value = 2
                    },
                    new[] {2,3}
                },
                {
                    new Filter
                    {
                        Operator = Operators.GreaterThanOrEqual,
                        Value = 2,
                        Logic = "or",
                        Filters = new []
                        {
                            new Filter
                            {
                                Operator = Operators.Equal,
                                Value = 3
                            }
                        }
                    },
                    new[] {2,3}
                },
                {
                    new Filter
                    {
                        Operator = Operators.GreaterThan,
                        Value = 1,
                        Logic = "and",
                        Filters = new []
                        {
                            new Filter
                            {
                                Operator = Operators.LessThanOrEqual,
                                Value = 3
                            }
                        }
                    },
                    new[] {2, 3}
                }
            };

            return data;
        }
    }
}