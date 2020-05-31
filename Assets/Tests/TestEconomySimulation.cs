using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests
{
    public class TestEconomySimulation
    {
        public readonly struct SpendMoneyTestCase
        {
            public SpendMoneyTestCase(
                string description,
                int initialMoney,
                int spendAmount,
                int expectedAvailableMoney,
                Exception expectedException = null
            )
            {
                Description = description;
                InitialMoney = initialMoney;
                SpendAmount = spendAmount;
                ExpectedAvailableMoney = expectedAvailableMoney;
                ExpectedException = expectedException;
            }

            public string Description { get; }

            // The initial amount of money that is available.
            public int InitialMoney { get; }
            // The amount of money to be spent.
            public int SpendAmount { get; }
            // Expected available money after spending the given amount.
            public int ExpectedAvailableMoney { get; }

            // The exception that is expected to be thrown by SpendMoney()
            public Exception ExpectedException { get; }

            public override string ToString()
            {
                return Description;
            }
        }

        private static SpendMoneyTestCase[] SpendMoneyTestCases = (
            new SpendMoneyTestCase[] {
                new SpendMoneyTestCase(
                    description: "Test spending some amount of money",
                    initialMoney: 100,
                    spendAmount: 25,
                    expectedAvailableMoney: 75
                ),
                new SpendMoneyTestCase(
                    description: "Test spending some negative amount of money",
                    initialMoney: 100,
                    spendAmount: -25,
                    expectedException: new ArgumentOutOfRangeException(
                        paramName: "amount",
                        message: "Cannot spend negative amount of money."
                    ),
                    expectedAvailableMoney: 100
                ),
                new SpendMoneyTestCase(
                    description: "Test spending zero money",
                    initialMoney: 100,
                    spendAmount: 0,
                    expectedAvailableMoney: 100
                ),
                new SpendMoneyTestCase(
                    description: "Test spending more money than available",
                    initialMoney: 100,
                    spendAmount: 101,
                    expectedException: new ArgumentOutOfRangeException(
                        paramName: "amount",
                        message: "Cannot spend more money than available."
                    ),
                    expectedAvailableMoney: 100
                ),
                new SpendMoneyTestCase(
                    description: "Test spending the whole available money",
                    initialMoney: 100,
                    spendAmount: 100,
                    expectedAvailableMoney: 0
                ),
            }
        );

        [Test, TestCaseSource("SpendMoneyTestCases")]
        public void TestSpendMoney(SpendMoneyTestCase testCase)
        {
            var economySimulation = new EconomySimulation(
                initialMoney: testCase.InitialMoney,
                getIncome: null,
                getUpkeepCosts: null
            );

            TestDelegate spendMoney = () => {
                economySimulation.SpendMoney(testCase.SpendAmount);
            };

            if (testCase.ExpectedException == null)
            {
                spendMoney();
            }
            else
            {
                Assert.Throws(
                    Is.InstanceOf(testCase.ExpectedException.GetType())
                        .And.Message.EqualTo(
                            testCase.ExpectedException.Message
                        )
                        .And.Property("Data").EqualTo(
                            testCase.ExpectedException.Data
                        ),
                    spendMoney
                );
            }

            Assert.That(
                economySimulation.AvailableMoney,
                Is.EqualTo(testCase.ExpectedAvailableMoney)
            );
        }


        public readonly struct CanAffordTestCase
        {
            public CanAffordTestCase(
                string description,
                int initialMoney,
                int amount,
                bool? expectedCanAfford = null,
                Exception expectedException = null
            )
            {
                Description = description;
                InitialMoney = initialMoney;
                Amount = amount;
                ExpectedCanAfford = expectedCanAfford;
                ExpectedException = expectedException;
            }

            public string Description { get; }

            // The initial amount of money that is available.
            public int InitialMoney { get; }
            // The amount of money that should be spent.
            public int Amount { get; }
            // The expected return value of CanAfford().
            public bool? ExpectedCanAfford { get; }

            // The exception that is expected to be thrown by CanAfford()
            public Exception ExpectedException { get; }

            public override string ToString()
            {
                return Description;
            }
        }

        private static CanAffordTestCase[] CanAffordTestCases = (
            new CanAffordTestCase[] {
                new CanAffordTestCase(
                    description: (
                        "Test can afford to spend some amount of money"
                    ),
                    initialMoney: 100,
                    amount: 25,
                    expectedCanAfford: true
                ),
                new CanAffordTestCase(
                    description: (
                        "Test cannot afford to spend " +
                        "some negative amount of money"
                    ),
                    initialMoney: 100,
                    amount: -25,
                    expectedException: new ArgumentOutOfRangeException(
                        paramName: "amount",
                        message: "Cannot spend negative amount of money."
                    )
                ),
                new CanAffordTestCase(
                    description: "Test can afford to spend zero money",
                    initialMoney: 100,
                    amount: 0,
                    expectedCanAfford: true
                ),
                new CanAffordTestCase(
                    description: (
                        "Test cannot afford to spend " +
                        "more money than available"
                    ),
                    initialMoney: 100,
                    amount: 101,
                    expectedCanAfford: false
                ),
                new CanAffordTestCase(
                    description: (
                        "Test can afford to spend " +
                        "the whole available money"
                    ),
                    initialMoney: 100,
                    amount: 100,
                    expectedCanAfford: true
                ),
            }
        );

        [Test, TestCaseSource("CanAffordTestCases")]
        public void TestCanAfford(CanAffordTestCase testCase)
        {
            var economySimulation = new EconomySimulation(
                initialMoney: testCase.InitialMoney,
                getIncome: null,
                getUpkeepCosts: null
            );

            if (testCase.ExpectedException == null)
            {
                Assert.That(
                    economySimulation.CanAfford(testCase.Amount),
                    Is.EqualTo(testCase.ExpectedCanAfford)
                );
            }
            else
            {
                Assert.Throws(
                    Is.InstanceOf(testCase.ExpectedException.GetType())
                        .And.Message.EqualTo(
                            testCase.ExpectedException.Message
                        )
                        .And.Property("Data").EqualTo(
                            testCase.ExpectedException.Data
                        ),
                    () => {
                        economySimulation.CanAfford(testCase.Amount);
                    }
                );
            }
        }


        public readonly struct TickTestCase
        {
            public TickTestCase(
                string description,
                int initialMoney,
                int expectedAvailableMoney,
                Func<int> getIncome = null,
                Func<List<int>> getUpkeepCosts = null,
                Exception expectedException = null
            )
            {
                Description = description;
                InitialMoney = initialMoney;
                GetIncome = getIncome;
                GetUpkeepCosts = getUpkeepCosts;
                ExpectedAvailableMoney = expectedAvailableMoney;
                ExpectedException = expectedException;
            }

            public string Description { get; }

            // The initial amount of money that is available.
            public int InitialMoney { get; }
            // Function that provides the income for a simulation cycle.
            public Func<int> GetIncome { get; }
            // Function that provides the upkeep costs
            // for a simulation cycle.
            public Func<List<int>> GetUpkeepCosts { get; }
            // Expected available money after simulation cycle.
            public int ExpectedAvailableMoney { get; }

            // The exception that is expected to be thrown by Tick()
            public Exception ExpectedException { get; }

            public override string ToString()
            {
                return Description;
            }
        }

        private static TickTestCase[] TickTestCases = (
            new TickTestCase[] {
                new TickTestCase(
                    description: "Test income",
                    initialMoney: 100,
                    getIncome: () => 25,
                    expectedAvailableMoney: 125
                ),
                new TickTestCase(
                    description: "Test negative income",
                    initialMoney: 100,
                    getIncome: () => -25,
                    expectedException: new ArgumentOutOfRangeException(
                        paramName: "amount",
                        message: "Cannot have negative income."
                    ),
                    expectedAvailableMoney: 100
                ),
                new TickTestCase(
                    description: "Test zero income",
                    initialMoney: 100,
                    getIncome: () => 0,
                    expectedAvailableMoney: 100
                ),
                new TickTestCase(
                    description: "Test no upkeep costs",
                    initialMoney: 100,
                    getUpkeepCosts: () => new List<int> { },
                    expectedAvailableMoney: 100
                ),
                new TickTestCase(
                    description: "Test upkeep costs",
                    initialMoney: 100,
                    getUpkeepCosts: () => new List<int> { 10, 20},
                    expectedAvailableMoney: 70
                ),
                new TickTestCase(
                    description: (
                        "Test upkeep costs more money than available"
                    ),
                    initialMoney: 100,
                    getUpkeepCosts: () => new List<int> { 100, 1},
                    expectedAvailableMoney: 0
                ),
            }
        );

        [Test, TestCaseSource("TickTestCases")]
        public void TickMoney(TickTestCase testCase)
        {
            var economySimulation = new EconomySimulation(
                initialMoney: testCase.InitialMoney,
                getIncome: testCase.GetIncome,
                getUpkeepCosts: testCase.GetUpkeepCosts
            );

            TestDelegate tick = () => {
                economySimulation.Tick();
            };

            if (testCase.ExpectedException == null)
            {
                tick();
            }
            else
            {
                Assert.Throws(
                    Is.InstanceOf(testCase.ExpectedException.GetType())
                        .And.Message.EqualTo(
                            testCase.ExpectedException.Message
                        )
                        .And.Property("Data").EqualTo(
                            testCase.ExpectedException.Data
                        ),
                    tick
                );
            }

            Assert.That(
                economySimulation.AvailableMoney,
                Is.EqualTo(testCase.ExpectedAvailableMoney)
            );
        }
    }
}
