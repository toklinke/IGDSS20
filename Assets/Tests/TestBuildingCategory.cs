using NUnit.Framework;

namespace Tests
{
    public class TestBuildingCategoryParams
    {
        public readonly struct IsCompatibleTileTypeTestCase
        {
            public IsCompatibleTileTypeTestCase(
                string description,
                MapTileType[] compatibleTileTypes,
                MapTileType tileType,
                bool expectedIsCompatible
            )
            {
                Description = description;
                CompatibleTileTypes = compatibleTileTypes;
                TileType = tileType;
                ExpectedIsCompatible = expectedIsCompatible;
            }

            public string Description { get; }

            // All tile types where the building category can be placed.
            public MapTileType[] CompatibleTileTypes { get; }
            // The type of the map tile.
            public MapTileType TileType { get; }
            // Whether the tile type is expected to be compatible.
            public bool ExpectedIsCompatible { get; }

            public override string ToString()
            {
                return Description;
            }
        }

        private static IsCompatibleTileTypeTestCase[]
            IsCompatibleTileTypeTestCases = (
            new IsCompatibleTileTypeTestCase[] {
                new IsCompatibleTileTypeTestCase(
                    description: "No compatible types",
                    compatibleTileTypes: new MapTileType[] {},
                    tileType: MapTileType.Forest,
                    expectedIsCompatible: false
                ),
                new IsCompatibleTileTypeTestCase(
                    description: "Is compatible with sole type",
                    compatibleTileTypes: new MapTileType[] {
                        MapTileType.Forest
                    },
                    tileType: MapTileType.Forest,
                    expectedIsCompatible: true
                ),
                new IsCompatibleTileTypeTestCase(
                    description: "Is not compatible with sole type",
                    compatibleTileTypes: new MapTileType[] {
                        MapTileType.Forest
                    },
                    tileType: MapTileType.Grass,
                    expectedIsCompatible: false
                ),
                new IsCompatibleTileTypeTestCase(
                    description: (
                        "Is compatible with one of compatible types"
                    ),
                    compatibleTileTypes: new MapTileType[] {
                        MapTileType.Grass, MapTileType.Forest
                    },
                    tileType: MapTileType.Forest,
                    expectedIsCompatible: true
                ),
                new IsCompatibleTileTypeTestCase(
                    description: (
                        "Is not compatible with any of compatible types"
                    ),
                    compatibleTileTypes: new MapTileType[] {
                        MapTileType.Grass, MapTileType.Stone
                    },
                    tileType: MapTileType.Forest,
                    expectedIsCompatible: false
                ),
            }
        );

        [Test, TestCaseSource("IsCompatibleTileTypeTestCases")]
        public void TestIsCompatibleTileType(
            IsCompatibleTileTypeTestCase testCase
        )
        {
            var buildingCategoryParams = new BuildingCategoryParams(
                compatibleTileTypes: testCase.CompatibleTileTypes,
                // don't care for rest of parameters.
                upkeepCost: 0,
                buildCostMoney: 0,
                buildCostPlanks: 0,
                resourceGenerationInterval: 0,
                outputCount: 0,
                efficiencyScaleTileType: MapTileType.Grass,
                efficiencyScaleMinNeighbors: 0,
                efficiencyScaleMaxNeighbors: 0,
                inputResources: null,
                outputResource: ResourceType.Wood
            );

            var isCompatible = buildingCategoryParams.IsCompatibleTileType(
                testCase.TileType
            );

            Assert.That(
                isCompatible,
                Is.EqualTo(testCase.ExpectedIsCompatible)
            );
        }
    }
}

