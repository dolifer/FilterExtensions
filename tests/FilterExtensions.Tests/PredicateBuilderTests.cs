using FilterExtensions.Constants;
using FilterExtensions.Filters;
using Xunit;

namespace FilterExtensions.Tests
{
    public class PredicateBuilderTests
    {
        [Theory]
        [InlineData(Operators.Equal, "x => x = @0")]
        [InlineData(Operators.NotEqual, "x => x != @0")]
        [InlineData(Operators.GreaterThan, "x => x > @0")]
        [InlineData(Operators.GreaterThanOrEqual, "x => x >= @0")]
        [InlineData(Operators.LessThan, "x => x < @0")]
        [InlineData(Operators.LessThanOrEqual, "x => x <= @0")]
        public void Build_Predicate_RootFilter_NoPropertyName(string operatorName, string expectedValue)
        {
            // arrange
            var filter = new Filter
            {
                Operator = operatorName
            };

            // act
            var result = QueryableExtensions.BuildPredicate(filter);

            // arrange
            Assert.Equal(expectedValue, result);
        }

        [Theory]
        [InlineData(Operators.Equal, "x => (x = @0 or x = @1)")]
        [InlineData(Operators.NotEqual, "x => (x != @0 or x != @1)")]
        [InlineData(Operators.GreaterThan, "x => (x > @0 or x > @1)")]
        [InlineData(Operators.GreaterThanOrEqual, "x => (x >= @0 or x >= @1)")]
        [InlineData(Operators.LessThan, "x => (x < @0 or x < @1)")]
        [InlineData(Operators.LessThanOrEqual, "x => (x <= @0 or x <= @1)")]
        public void Build_Predicate_ManyFilter_NoPropertyName(string operatorName, string expectedValue)
        {
            // arrange
            var filter = new Filter
            {
                Operator = operatorName,
                Logic = "or",
                Filters = new []
                {
                    new Filter
                    {
                        Operator = operatorName
                    }
                }
            };

            // act
            var result = QueryableExtensions.BuildPredicate(filter);

            // arrange
            Assert.Equal(expectedValue, result);
        }

        [Theory]
        [InlineData(Operators.Equal, "1", "f = @0")]
        [InlineData(Operators.NotEqual, "1", "f != @0")]
        [InlineData(Operators.GreaterThan, "1", "f > @0")]
        [InlineData(Operators.GreaterThanOrEqual, "1", "f >= @0")]
        [InlineData(Operators.LessThan, "1", "f < @0")]
        [InlineData(Operators.LessThanOrEqual, "1", "f <= @0")]
        public void Build_Predicate_RootFilter_PropertyName(string operatorName, object value, string expectedValue)
        {
            // arrange
            var filter = new Filter
            {
                Property = "f",
                Operator = operatorName,
                Value = value
            };

            // act
            var result = QueryableExtensions.BuildPredicate(filter);

            // arrange
            Assert.Equal(expectedValue, result);
        }

        [Theory]
        [InlineData("and", "(l = @0 and r = @1)")]
        [InlineData("or", "(l = @0 or r = @1)")]
        public void Build_Predicate_NestedFilters(string logic, string expectedValue)
        {
            // arrange
            var filter = new Filter
            {
                Logic = logic,
                Filters = new []
                {
                    new Filter
                    {
                        Property = "l",
                        Operator = Operators.Equal
                    },
                    new Filter
                    {
                        Property = "r",
                        Operator = Operators.Equal
                    }
                }
            };

            // act
            var result = QueryableExtensions.BuildPredicate(filter);

            // arrange
            Assert.Equal(expectedValue, result);
        }

        [Theory]
        [InlineData("and", "(top = @0 and l = @1 and r = @2)")]
        [InlineData("or", "(top = @0 or l = @1 or r = @2)")]
        public void Build_Predicate_Top_And_NestedFilters(string logic, string expectedValue)
        {
            // arrange
            var filter = new Filter
            {
                Property = "top",
                Operator = Operators.Equal,
                Logic = logic,
                Filters = new []
                {
                    new Filter
                    {
                        Property = "l",
                        Operator = Operators.Equal
                    },
                    new Filter
                    {
                        Property = "r",
                        Operator = Operators.Equal
                    }
                }
            };

            // act
            var result = QueryableExtensions.BuildPredicate(filter);

            // arrange
            Assert.Equal(expectedValue, result);
        }
    }
}