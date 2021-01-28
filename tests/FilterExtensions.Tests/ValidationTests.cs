using System;
using FilterExtensions.Filters;
using Xunit;

namespace FilterExtensions.Tests
{
    public class ValidationTests
    {
        [Theory]
        [MemberData(nameof(GetValidationTestCases))]
        public void Throws_ArgumentException_Logic(string paramName, Filter filter)
        {
            // act & assert
            var exception = Assert.Throws<ArgumentException>(() => QueryableExtensions.BuildPredicate(filter));

            Assert.Equal(paramName, exception.ParamName);
        }

        public static TheoryData<string, Filter> GetValidationTestCases()
        {
            var data = new TheoryData<string, Filter>
            {
                {"filter", null},
                {nameof(Filter.Operator), new Filter {Property = "f1"}},
                {nameof(Filter.Logic), new Filter {Filters = new[] {new Filter()}}},
            };

            return data;
        }
    }
}