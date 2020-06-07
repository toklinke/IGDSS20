using NUnit.Framework;
using System;

namespace Tests
{
    public class TestWarehouse
    {
        public readonly struct WarehouseTestCase
        {
            public WarehouseTestCase(
                string description,
                Action<Warehouse> setupWarehouse = null,
                Action<Warehouse> performOperation = null,
                Action<Warehouse> checkAfterOperation = null,
                Exception expectedException = null
            )
            {
                Description = description;
                SetupWarehouse = setupWarehouse;
                PerformOperation = performOperation;
                CheckAfterOperation = checkAfterOperation;
                ExpectedException = expectedException;
            }

            public string Description { get; }

            public Action<Warehouse> SetupWarehouse { get; }
            public Action<Warehouse> PerformOperation { get; }
            public Action<Warehouse> CheckAfterOperation { get; }

            // The exception that is expected to be thrown
            // by the performed warehouse operation
            public Exception ExpectedException { get; }

            public override string ToString()
            {
                return Description;
            }
        }

        private static WarehouseTestCase[] WarehouseTestCases = (
            new WarehouseTestCase[] {
                new WarehouseTestCase(
                    description: "Test warehouse initially empty",
                    checkAfterOperation: warehouse => {
                        var resourceTypes = Enum.GetValues(
                            typeof(ResourceType)
                        );
                        foreach(ResourceType type in resourceTypes)
                        {
                            CheckAvailableAmount(warehouse, type, 0);
                        }
                    }
                ),
                new WarehouseTestCase(
                    description: "Test storing resources",
                    performOperation: warehouse => {
                        warehouse.Store(
                            type: ResourceType.Wood, amount: 2
                        );
                        warehouse.Store(
                            type: ResourceType.Plank, amount: 7
                        );
                    },
                    checkAfterOperation: warehouse => {
                        CheckAvailableAmount(
                            warehouse, ResourceType.Wood, 2
                        );
                        CheckAvailableAmount(
                            warehouse, ResourceType.Plank, 7
                        );
                    }
                ),
                new WarehouseTestCase(
                    description: "Test storing more resources",
                    setupWarehouse: warehouse => {
                        warehouse.Store(
                            type: ResourceType.Wood, amount: 2
                        );
                    },
                    performOperation: warehouse => {
                        warehouse.Store(
                            type: ResourceType.Wood, amount: 7
                        );
                    },
                    checkAfterOperation: warehouse => {
                        CheckAvailableAmount(
                            warehouse, ResourceType.Wood, 9
                        );
                    }
                ),
                new WarehouseTestCase(
                    description: "Test pick some resources",
                    setupWarehouse: warehouse => {
                        warehouse.Store(
                            type: ResourceType.Wood, amount: 7
                        );
                    },
                    performOperation: warehouse => {
                        warehouse.Pick(
                            type: ResourceType.Wood, amount: 2
                        );
                    },
                    checkAfterOperation: warehouse => {
                        CheckAvailableAmount(
                            warehouse, ResourceType.Wood, 5
                        );
                    }
                ),
                new WarehouseTestCase(
                    description: (
                        "Test more than enough resources are available"
                    ),
                    setupWarehouse: warehouse => {
                        warehouse.Store(
                            type: ResourceType.Wood, amount: 2
                        );
                    },
                    checkAfterOperation: warehouse => {
                        CheckIsAvailable(
                            warehouse, ResourceType.Wood, 1, true
                        );
                    }
                ),
                new WarehouseTestCase(
                    description: (
                        "Test just enough resources are available"
                    ),
                    setupWarehouse: warehouse => {
                        warehouse.Store(
                            type: ResourceType.Wood, amount: 2
                        );
                    },
                    checkAfterOperation: warehouse => {
                        CheckIsAvailable(
                            warehouse, ResourceType.Wood, 2, true
                        );
                    }
                ),
                new WarehouseTestCase(
                    description: (
                        "Test not enough resources are available"
                    ),
                    setupWarehouse: warehouse => {
                        warehouse.Store(
                            type: ResourceType.Wood, amount: 2
                        );
                    },
                    checkAfterOperation: warehouse => {
                        CheckIsAvailable(
                            warehouse, ResourceType.Wood, 3, false
                        );
                    }
                ),
            }
        );

        [Test, TestCaseSource("WarehouseTestCases")]
        public void TestWarehouseOperation(WarehouseTestCase testCase)
        {
            var warehouse = new Warehouse();
            if (testCase.SetupWarehouse != null)
                testCase.SetupWarehouse(warehouse);

            TestDelegate performOperation = () => {
                if (testCase.PerformOperation != null)
                    testCase.PerformOperation(warehouse);
            };

            if (testCase.ExpectedException == null)
            {
                performOperation();
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
                    performOperation
                );
            }

            testCase.CheckAfterOperation(warehouse);
        }

        private static void CheckAvailableAmount(
            Warehouse warehouse, ResourceType type, int expectedAmount
        )
        {
            var availableAmount = warehouse.GetAvailableAmount(type);
            Assert.That(availableAmount, Is.EqualTo(expectedAmount));
        }

        private static void CheckIsAvailable(
            Warehouse warehouse,
            ResourceType type,
            int amount,
            bool expectedIsAvailable
        )
        {
            var isAvailable = warehouse.IsAvailable(type, amount);
            Assert.That(isAvailable, Is.EqualTo(expectedIsAvailable));
        }
    }
}
