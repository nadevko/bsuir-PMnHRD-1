﻿using System.Text.Json;
using DeepEqual;
using DeepEqual.Syntax;
using PMnHRD1.App.Models;

namespace PMnHRD1.Tests.Services;

[TestClass]
public class Data
{
    [TestInitialize]
    public void SetUp()
    {
        _options = App.Services.Data.Options;
    }

    private JsonSerializerOptions? _options;

    [TestMethod]
    [DynamicData(nameof(TestPairs), DynamicDataSourceType.Method)]
    public void Read_ValidJSON_WithoutExceptions(string file, Suite expected)
    {
        file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", file);
        Assert.IsTrue(File.Exists(file));

        var actual = App.Services.Data.LoadFile<Suite>(file);

        actual
            .WithDeepEqual(expected)
            .IgnoreUnmatchedProperties()
            .WithCustomComparison(new IgnoreOrderComparison())
            .Assert();
    }

    private class IgnoreOrderComparison : IComparison
    {
        public bool CanCompare(Type type1, Type type2)
        {
            return typeof(IEnumerable<IQuestion>).IsAssignableFrom(type1)
                && typeof(IEnumerable<IQuestion>).IsAssignableFrom(type2);
        }

        public (ComparisonResult result, IComparisonContext context) Compare(
            IComparisonContext context,
            object value1,
            object value2
        )
        {
            var list1 = ((IEnumerable<IQuestion>)value1).OrderBy(q => q.Text).ToList();
            var list2 = ((IEnumerable<IQuestion>)value2).OrderBy(q => q.Text).ToList();

            var comparisonResult = list1.IsDeepEqual(list2)
                ? ComparisonResult.Pass
                : ComparisonResult.Fail;

            return (comparisonResult, context);
        }
    }

    private static IEnumerable<object[]> TestPairs() =>
        [
            [
                "camelCase.json",
                new Suite
                {
                    Name = "Examples of tests",
                    Description = "Every suite and test must have all those fields",
                    Id = 1,
                    Externals = [],
                },
            ],
            [
                "costs.json",
                new Suite
                {
                    Name = "Examples of tests",
                    Description = "Every suite and test must have all those fields",
                    Id = 1,
                    Externals = ["Literature and web references of the suite"],
                    Tests =
                    [
                        new TestCosts
                        {
                            Name = "Test with results based on costs",
                            Description = "Result will be choosen by sum of answers costs",
                            Id = 0,
                            Externals = ["Literature and web references of the suite"],
                            Costs = [-2, -1, 1, 2],
                            Answers =
                            [
                                "First answers has first cost",
                                "User can choose only one answer",
                                "Answers will be written on buttons",
                                "Number of answers must be equal to number of costs",
                            ],
                            Questions =
                            [
                                new QuestionCosts { Text = "Use default costs and answers" },
                                new QuestionCosts { Text = "Override costs", Costs = [1, 2, 3, 4] },
                                new QuestionCosts
                                {
                                    Text = "Override answers",
                                    Answers = ["First", "Second", "Third", "Fourth"],
                                },
                                new QuestionCosts
                                {
                                    Text =
                                        "Override costs and answers. Now you can change number of them",
                                    Costs = [1, 2, 3],
                                    Answers = ["First", "Second", "Third"],
                                },
                                new QuestionCosts
                                {
                                    Text = "Eqivalent to question with Text field only",
                                },
                            ],
                            Results = new Stack<ResultCosts>(
                                [
                                    new ResultCosts
                                    {
                                        Text = "This result will be shown if from <= sum < to",
                                        From = -2,
                                        To = 0,
                                    },
                                    new ResultCosts
                                    {
                                        Text =
                                            "If range limit not passed, program assumes it is infinity: 0 <= sum < inf",
                                        From = 0,
                                        To = int.MaxValue,
                                    },
                                ]
                            ),
                        },
                    ],
                },
            ],
        ];
}
