using NUnit.Framework;
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
                MapTile expectedMapTile,
                Vector3? expectedSpawnPos
            )
            {
                Description = description;
                Tile = tile;
                MapX = mapX;
                MapY = mapY;
                BuildingCategoryParams = buildingCategoryParams;
                ExpectedMapTile = expectedMapTile;
                ExpectedSpawnPos = expectedSpawnPos;
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
                        building: new Building()
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
                        building: new Building()
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
                        height: 0.5f,
                        type: MapTileType.Grass,
                        building: new Building()
                    ),
                    expectedSpawnPos: new Vector3(1.0f, 0.5f, 0.0f)
                ),
            }
        );

        [Test, TestCaseSource("PlaceBuildingOnTileTestCases")]
        public void TestPlaceBuildingOnTile(
            PlaceBuildingOnTileTestCase testCase
        )
        {
            var heightMap = new HeightMap(
                colors: new Color[] {
                    GetGrayColor(0.0f), GetGrayColor(0.0f),
                    GetGrayColor(0.0f), GetGrayColor(0.0f)
                },
                width: 2,
                height: 2
            );
            var dummyMapToWorldMapper = new DummyMapToWorldMapper();

            var game = new Game(
                heightMap: heightMap,
                spawnMapTile: (tileToSpawn, mapX, mapY, worldPos) => {},
                mapToWorldMapper: dummyMapToWorldMapper
            );

            var spawnPositions = new List<Vector3>();
            var tile = testCase.Tile;

            game.PlaceBuildingOnTile(
                tile: testCase.Tile,
                mapX: testCase.MapX,
                mapY: testCase.MapY,
                buildingCategoryParams: testCase.BuildingCategoryParams,
                spawnBuilding: worldPos => {
                    spawnPositions.Add(worldPos);
                }
            );

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
        }

        // get grayscale color with R=G=B and A=1.0f
        private static Color GetGrayColor(float rgb)
        {
            Color color = new Color(rgb, rgb, rgb, a: 1.0f);
            return color;
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


