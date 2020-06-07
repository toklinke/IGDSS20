using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    public class TestGame
    {
        public readonly struct PlaceBuildingOnTileTestCase
        {
            public PlaceBuildingOnTileTestCase(
                string description,
                MapTile tile,
                uint mapX,
                uint mapY,
                BuildingCategoryParams buildingCategoryParams,
                MapTile expectedMapTile = null,
                Vector3? expectedSpawnPos = null,
                int initialMoney = 0,
                int? expectedAvailableMoney = null
            )
            {
                Description = description;
                Tile = tile;
                MapX = mapX;
                MapY = mapY;
                BuildingCategoryParams = buildingCategoryParams;
                ExpectedMapTile = expectedMapTile;
                ExpectedSpawnPos = expectedSpawnPos;
                InitialMoney = initialMoney;
                ExpectedAvailableMoney = expectedAvailableMoney;
            }

            public string Description { get; }

            // The tile where the building should be placed on.
            public MapTile Tile { get; }

            // Map position where the building should be placed on.
            public uint MapX { get; }
            public uint MapY { get; }

            // params of the building to be created.
            public BuildingCategoryParams BuildingCategoryParams { get; }

            // The expected map tile after the placement of the building
            public MapTile ExpectedMapTile { get; }

            // The expected spawn position of the new building.
            public Vector3? ExpectedSpawnPos { get; }

            // The initially available money.
            public int InitialMoney { get; }

            // If not null,
            // check that the given amount of money is available
            // after placing the building.
            public int? ExpectedAvailableMoney { get; }

            public override string ToString()
            {
                return Description;
            }
        }

        private static PlaceBuildingOnTileTestCase[]
            PlaceBuildingOnTileTestCases = (
            new PlaceBuildingOnTileTestCase[] {
                new PlaceBuildingOnTileTestCase(
                    description: "Tile is not compatible",
                    tile: new MapTile(
                        height: 42.0f,
                        type: MapTileType.Grass
                    ),
                    mapX: 0,
                    mapY: 0,
                    buildingCategoryParams: new BuildingCategoryParams(
                        compatibleTileTypes: new MapTileType[] {
                            MapTileType.Forest
                        },
                        // don't care follows
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
                    ),
                    expectedMapTile: new MapTile(
                        height: 42.0f,
                        type: MapTileType.Grass
                    ),
                    expectedSpawnPos: null
                ),
                new PlaceBuildingOnTileTestCase(
                    description: "Tile is not empty",
                    tile: new MapTile(
                        height: 42.0f,
                        type: MapTileType.Grass,
                        building: new Building(
                            // don't care follows
                            upkeepCost: 0,
                            resourceGenerationInterval: 0,
                            outputResource: ResourceType.Wood,
                            outputCount: 0
                        )
                    ),
                    mapX: 0,
                    mapY: 0,
                    buildingCategoryParams: new BuildingCategoryParams(
                        compatibleTileTypes: new MapTileType[] {
                            MapTileType.Grass
                        },
                        // don't care follows
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
                    ),
                    expectedMapTile: new MapTile(
                        height: 42.0f,
                        type: MapTileType.Grass,
                        building: new Building(
                            upkeepCost: 0,
                            resourceGenerationInterval: 0,
                            outputResource: ResourceType.Wood,
                            outputCount: 0
                        )
                    ),
                    expectedSpawnPos: null
                ),
                new PlaceBuildingOnTileTestCase(
                    description: "Spawn building",
                    tile: new MapTile(
                        height: 0.5f,
                        type: MapTileType.Grass
                    ),
                    mapX: 1,
                    mapY: 0,
                    buildingCategoryParams: new BuildingCategoryParams(
                        compatibleTileTypes: new MapTileType[] {
                            MapTileType.Grass
                        },
                        upkeepCost: 10,
                        resourceGenerationInterval: 30.0f,
                        outputResource: ResourceType.Wood,
                        outputCount: 1,
                        // don't care follows
                        buildCostMoney: 0,
                        buildCostPlanks: 0,
                        efficiencyScaleTileType: MapTileType.Grass,
                        efficiencyScaleMinNeighbors: 0,
                        efficiencyScaleMaxNeighbors: 0,
                        inputResources: null
                    ),
                    expectedMapTile: new MapTile(
                        height: 0.5f,
                        type: MapTileType.Grass,
                        building: new Building(
                            upkeepCost: 10,
                            resourceGenerationInterval: 30,
                            outputResource: ResourceType.Wood,
                            outputCount: 1
                        )
                    ),
                    expectedSpawnPos: new Vector3(1.0f, 0.5f, 0.0f)
                ),
                new PlaceBuildingOnTileTestCase(
                    description: "Test that money is spent for building",
                    tile: new MapTile(
                        height: 0.0f,
                        type: MapTileType.Grass
                    ),
                    mapX: 0,
                    mapY: 0,
                    buildingCategoryParams: new BuildingCategoryParams(
                        compatibleTileTypes: new MapTileType[] {
                            MapTileType.Grass
                        },
                        buildCostMoney: 100,
                        // don't care follows
                        upkeepCost: 0,
                        buildCostPlanks: 0,
                        resourceGenerationInterval: 0,
                        outputCount: 0,
                        efficiencyScaleTileType: MapTileType.Grass,
                        efficiencyScaleMinNeighbors: 0,
                        efficiencyScaleMaxNeighbors: 0,
                        inputResources: null,
                        outputResource: ResourceType.Wood
                    ),
                    initialMoney: 1000,
                    expectedAvailableMoney: 900,
                    expectedSpawnPos: new Vector3(0.0f, 0.0f, 0.0f)
                ),
                new PlaceBuildingOnTileTestCase(
                    description: "Test cannot afford to build",
                    tile: new MapTile(
                        height: 0.0f,
                        type: MapTileType.Grass
                    ),
                    mapX: 0,
                    mapY: 0,
                    buildingCategoryParams: new BuildingCategoryParams(
                        compatibleTileTypes: new MapTileType[] {
                            MapTileType.Grass
                        },
                        buildCostMoney: 100,
                        // don't care follows
                        upkeepCost: 0,
                        buildCostPlanks: 0,
                        resourceGenerationInterval: 0,
                        outputCount: 0,
                        efficiencyScaleTileType: MapTileType.Grass,
                        efficiencyScaleMinNeighbors: 0,
                        efficiencyScaleMaxNeighbors: 0,
                        inputResources: null,
                        outputResource: ResourceType.Wood
                    ),
                    initialMoney: 50,
                    expectedAvailableMoney: 50,
                    expectedSpawnPos: null
                ),
            }
        );

        [Test, TestCaseSource("PlaceBuildingOnTileTestCases")]
        public void TestPlaceBuildingOnTile(
            PlaceBuildingOnTileTestCase testCase
        )
        {
            var mapTile = new MapTile(
                type: MapTileType.Water,
                height: 0.0f
            );
            var map = new Map(
                tiles: new MapTile[] {
                    mapTile.Clone(), mapTile.Clone(),
                    mapTile.Clone(), mapTile.Clone()
                },
                width: 2,
                height: 2
            );
            var dummyMapToWorldMapper = new DummyMapToWorldMapper();
            var mapGenerator = new DummyMapGenerator(map);

            var game = new Game(
                mapGenerator: mapGenerator,
                spawnMapTile: (tileToSpawn, mapX, mapY, worldPos) => {},
                mapToWorldMapper: dummyMapToWorldMapper,
                initialMoney: testCase.InitialMoney,
                incomePerEconomyTick: 0, // don't care
                economyTickInterval: 0 // don't care
            );

            var spawnPositions = new List<Vector3>();
            var tile = testCase.Tile.Clone();

            game.PlaceBuildingOnTile(
                tile: tile,
                mapX: testCase.MapX,
                mapY: testCase.MapY,
                buildingCategoryParams: testCase.BuildingCategoryParams,
                spawnBuilding: worldPos => {
                    spawnPositions.Add(worldPos);
                }
            );

            if (testCase.ExpectedMapTile != null)
                Assert.That(
                    tile,
                    Is.EqualTo(testCase.ExpectedMapTile)
                );

            if (testCase.ExpectedSpawnPos == null)
                Assert.That(spawnPositions.Count, Is.EqualTo(0));
            else
            {
                Assert.That(spawnPositions.Count, Is.EqualTo(1));
                Assert.That(
                    spawnPositions[0],
                    Is.EqualTo(testCase.ExpectedSpawnPos)
                );
            }

            if (testCase.ExpectedAvailableMoney != null)
                Assert.That(
                    game.AvailableMoney,
                    Is.EqualTo(testCase.ExpectedAvailableMoney)
                );
        }


        public readonly struct GameTimeTickTestCase
        {
            public GameTimeTickTestCase(
                string description,
                int initialMoney,
                int incomePerEconomyTick,
                int economyTickInterval,
                Action<Game> setupGame,
                Action<Game>[] checksAfterTick,
                Map map = null
            )
            {
                Description = description;
                InitialMoney = initialMoney;
                IncomePerEconomyTick = incomePerEconomyTick;
                EconomyTickInterval = economyTickInterval;
                SetupGame = setupGame;
                ChecksAfterTick = checksAfterTick;
                this.Map = map;
            }

            public string Description { get; }

            // The initially available money.
            public int InitialMoney { get; }

            // The income per economy tick.
            public int IncomePerEconomyTick { get; }

            // The economy tick interval of the game.
            public int EconomyTickInterval { get; }

            // Setup game before calling the tick.
            public Action<Game> SetupGame { get; }

            // A number of checks that should be performed after the tick.
            public Action<Game>[] ChecksAfterTick { get; }

            // The map to be used for the test.
            // If null, a default map consisting of a single tile is used.
            public Map Map { get; }

            public override string ToString()
            {
                return Description;
            }
        };

        private static GameTimeTickTestCase[] GameTimeTickTestCases = (
            new GameTimeTickTestCase[] {
                new GameTimeTickTestCase(
                    description: (
                        "check that income is added in economy tick"
                    ),
                    initialMoney: 1000,
                    incomePerEconomyTick: 100,
                    economyTickInterval: 3,
                    setupGame: game => {
                        game.GameTimeTick();
                        game.GameTimeTick();
                    },
                    checksAfterTick: new Action<Game>[] {
                        game => checkAvailableMoney(game, 1100)
                    }
                ),
                new GameTimeTickTestCase(
                    description: (
                        "upkeep costs are subtracted in economy tick"
                    ),
                    initialMoney: 1000,
                    incomePerEconomyTick: 0,
                    economyTickInterval: 3,
                    map: new Map(
                        tiles: new MapTile[] {
                            new MapTile(
                                height: 0.0f, type: MapTileType.Water
                            ),
                            new MapTile(
                                height: 0.0f, type: MapTileType.Water
                            ),
                            new MapTile(
                                height: 0.0f, type: MapTileType.Water
                            ),
                        },
                        width: 3,
                        height: 1
                    ),
                    setupGame: game => {
                        game.PlaceBuildingOnTile(
                            tile: game.Map.GetTile(x: 0, y: 0),
                            mapX: 0,
                            mapY: 0,
                            buildingCategoryParams: (
                                new BuildingCategoryParams(
                                    compatibleTileTypes: new MapTileType[]
                                    {
                                        MapTileType.Water
                                    },
                                    upkeepCost: 100,
                                    buildCostMoney: 0,
                                    // don't care follows
                                    buildCostPlanks: 0,
                                    resourceGenerationInterval: 0,
                                    outputCount: 0,
                                    efficiencyScaleTileType: MapTileType.Grass,
                                    efficiencyScaleMinNeighbors: 0,
                                    efficiencyScaleMaxNeighbors: 0,
                                    inputResources: null,
                                    outputResource: ResourceType.Wood
                                )
                            ),
                            spawnBuilding: worldPos => {}
                        );
                        game.PlaceBuildingOnTile(
                            tile: game.Map.GetTile(x: 2, y: 0),
                            mapX: 2,
                            mapY: 0,
                            buildingCategoryParams: (
                                new BuildingCategoryParams(
                                    compatibleTileTypes: new MapTileType[]
                                    {
                                        MapTileType.Water
                                    },
                                    upkeepCost: 50,
                                    buildCostMoney: 0,
                                    // don't care follows
                                    buildCostPlanks: 0,
                                    resourceGenerationInterval: 0,
                                    outputCount: 0,
                                    efficiencyScaleTileType: MapTileType.Grass,
                                    efficiencyScaleMinNeighbors: 0,
                                    efficiencyScaleMaxNeighbors: 0,
                                    inputResources: null,
                                    outputResource: ResourceType.Wood
                                )
                            ),
                            spawnBuilding: worldPos => {}
                        );

                        game.GameTimeTick();
                        game.GameTimeTick();
                    },
                    checksAfterTick: new Action<Game>[] {
                        game => checkAvailableMoney(game, 850)
                    }
                ),
                new GameTimeTickTestCase(
                    description: "check multiple economy ticks",
                    initialMoney: 1000,
                    incomePerEconomyTick: 100,
                    economyTickInterval: 3,
                    setupGame: game => {
                        game.GameTimeTick();
                        game.GameTimeTick();
                        game.GameTimeTick();

                        game.GameTimeTick();
                        game.GameTimeTick();
                    },
                    checksAfterTick: new Action<Game>[] {
                        game => checkAvailableMoney(game, 1200)
                    }
                )
            }
        );

        [Test, TestCaseSource("GameTimeTickTestCases")]
        public void TestGameTimeTick(GameTimeTickTestCase testCase)
        {
            var map = testCase.Map ?? new Map(
                tiles: new MapTile[] {
                    new MapTile(height: 0.0f, type: MapTileType.Water)
                },
                width: 1,
                height: 1
            );
            var mapGenerator = new DummyMapGenerator(map);
            var mapToWorldMapper = new DummyMapToWorldMapper();
            var game = new Game(
                mapGenerator: mapGenerator,
                spawnMapTile: (tile, mapX, mapY, worldPos) => {},
                mapToWorldMapper: mapToWorldMapper,
                initialMoney: testCase.InitialMoney,
                incomePerEconomyTick: testCase.IncomePerEconomyTick,
                economyTickInterval: testCase.EconomyTickInterval
            );

            testCase.SetupGame(game);

            game.GameTimeTick();

            foreach (var check in testCase.ChecksAfterTick)
            {
                check(game);
            }
        }

        private static void checkAvailableMoney(
            Game game, int expectedAvailableMoney
        )
        {
            Assert.That(
                game.AvailableMoney,
                Is.EqualTo(expectedAvailableMoney)
            );
        }

        // Map generator that returns a given map.
        private class DummyMapGenerator : IMapGenerator
        {
            private Map ResultMap;

            public DummyMapGenerator(Map resultMap)
            {
                ResultMap = resultMap;
            }

            public Map GenerateMap()
            {
                return ResultMap;
            }
        }

        // Maps map pos (x, y) => world pos (x, tile.Height, y)
        private class DummyMapToWorldMapper : IMapToWorldMapper
        {
            public Vector3 GetWorldPosition(
                uint mapX, uint mapY, MapTile tile
            )
            {
                var worldPos = new Vector3(
                    x: mapX,
                    y: tile.Height,
                    z: mapY
                );
                return worldPos;
            }

            public Vector3 GetWorldSize(uint mapWidth, uint mapHeight)
            {
                var worldSize = new Vector3(
                    x: mapWidth,
                    y: 1.0f,
                    z: mapHeight
                );
                return worldSize;
            }
        }
    }
}


