using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestMap
    {
        // Test that ForEachTile() calls the given action for each tile
        // with correct (x, y) position.
        [Test]
        public void TestForEachTile()
        {
            MapTile[] mapTiles = new MapTile[] {
                // first row
                new MapTile(height: 0.0f, type: MapTileType.Water),
                new MapTile(height: 1.0f, type: MapTileType.Sand),
                new MapTile(height: 2.0f, type: MapTileType.Grass),
                // second row
                new MapTile(height: 3.0f, type: MapTileType.Forest),
                new MapTile(height: 4.0f, type: MapTileType.Stone),
                new MapTile(height: 5.0f, type: MapTileType.Mountain),
            };

            Map map = new Map(
                tiles: mapTiles, width: 3, height: 2
            );

            var actionCalls = new List<(uint x, uint y, MapTile tile)>();

            map.ForEachTile((x, y, tile) => actionCalls.Add((x, y, tile)));

            var expectedCalls = new (uint x, uint y, MapTile tile)[] {
                // first row
                (0, 0, new MapTile(height: 0.0f, type: MapTileType.Water)),
                (1, 0, new MapTile(height: 1.0f, type: MapTileType.Sand)),
                (2, 0, new MapTile(height: 2.0f, type: MapTileType.Grass)),
                // second row
                (0, 1, new MapTile(height: 3.0f, type: MapTileType.Forest)),
                (1, 1, new MapTile(height: 4.0f, type: MapTileType.Stone)),
                (2, 1, new MapTile(height: 5.0f, type: MapTileType.Mountain)),
            };

            Assert.That(actionCalls, Is.EqualTo(expectedCalls));
        }
    }

    public class TestMapGenerator
    {
        public readonly struct GenerateMapFromHeightMapTestCase
        {
            public GenerateMapFromHeightMapTestCase(
                string description,
                uint heightMapWidth,
                uint heightMapHeight,
                Color[] heightMapColors,
                uint? expectedWidth = null,
                uint? expectedHeight = null,
                MapTileType[] expectedTileTypes = null,
                float[] expectedTileHeights = null
            )
            {
                Description = description;
                HeightMapWidth = heightMapWidth;
                HeightMapHeight = heightMapHeight;
                HeightMapColors = heightMapColors;
                ExpectedWidth = expectedWidth;
                ExpectedHeight = expectedHeight;
                ExpectedTileTypes = expectedTileTypes;
                ExpectedTileHeights = expectedTileHeights;
            }

            public string Description { get; }

            public uint HeightMapWidth { get; }
            public uint HeightMapHeight { get; }
            public Color[] HeightMapColors { get; }

            // Expected width of generated map
            public uint? ExpectedWidth { get; }
            // Expected height of generated map
            public uint? ExpectedHeight { get; }

            // Expected map tile types in row-column order.
            public MapTileType[] ExpectedTileTypes { get; }
            // Expected map tile heights in row-column order.
            public float[] ExpectedTileHeights { get; }

            public override string ToString()
            {
                return Description;
            }
        }

        private static GenerateMapFromHeightMapTestCase[]
            GenerateMapFromHeightMapTestCases = (
            new GenerateMapFromHeightMapTestCase[] {
                new GenerateMapFromHeightMapTestCase(
                    description: "Test size of generated map",
                    heightMapWidth: 2,
                    heightMapHeight: 3,
                    heightMapColors: new Color[6],
                    expectedWidth: 2,
                    expectedHeight: 3
                ),
                new GenerateMapFromHeightMapTestCase(
                    description: "Test that all tile types are generated",
                    heightMapWidth: 2,
                    heightMapHeight: 3,
                    heightMapColors: new Color[] {
                        GetGrayColor(0.0f), GetGrayColor(0.1f),
                        GetGrayColor(0.3f), GetGrayColor(0.5f),
                        GetGrayColor(0.7f), GetGrayColor(0.9f),
                    },
                    expectedTileTypes: new MapTileType[] {
                        MapTileType.Water, MapTileType.Sand,
                        MapTileType.Grass, MapTileType.Forest,
                        MapTileType.Stone, MapTileType.Mountain
                    }
                ),
                new GenerateMapFromHeightMapTestCase(
                    description: (
                        "Test gray values near lower limit " +
                        "for all tile types"
                    ),
                    heightMapWidth: 2,
                    heightMapHeight: 3,
                    heightMapColors: new Color[] {
                        GetGrayColor(0.0f), GetGrayColor(0.01f),
                        GetGrayColor(0.21f), GetGrayColor(0.41f),
                        GetGrayColor(0.61f), GetGrayColor(0.81f),
                    },
                    expectedTileTypes: new MapTileType[] {
                        MapTileType.Water, MapTileType.Sand,
                        MapTileType.Grass, MapTileType.Forest,
                        MapTileType.Stone, MapTileType.Mountain
                    }
                ),
                new GenerateMapFromHeightMapTestCase(
                    description: (
                        "Test gray values upper limit for all tile types"
                    ),
                    heightMapWidth: 2,
                    heightMapHeight: 3,
                    heightMapColors: new Color[] {
                        GetGrayColor(0.0f), GetGrayColor(0.2f),
                        GetGrayColor(0.4f), GetGrayColor(0.6f),
                        GetGrayColor(0.8f), GetGrayColor(1.0f),
                    },
                    expectedTileTypes: new MapTileType[] {
                        MapTileType.Water, MapTileType.Sand,
                        MapTileType.Grass, MapTileType.Forest,
                        MapTileType.Stone, MapTileType.Mountain
                    }
                ),
                new GenerateMapFromHeightMapTestCase(
                    description: (
                        "Test that height is set correctly for tiles"
                    ),
                    heightMapWidth: 2,
                    heightMapHeight: 2,
                    heightMapColors: new Color[] {
                        GetGrayColor(0.0f), GetGrayColor(0.2f),
                        GetGrayColor(0.7f), GetGrayColor(1.0f),
                    },
                    expectedTileHeights: new float[] {
                        0.0f, 0.2f,
                        0.7f, 1.0f
                    }
                ),
            }
        );

        // Test that GenerateMapFromHeightMap() generates
        // the correct map
        [Test, TestCaseSource("GenerateMapFromHeightMapTestCases")]
        public void TestGenerateMapFromHeightMap(
            GenerateMapFromHeightMapTestCase testCase
        )
        {
            MapGenerator mapGenerator = new MapGenerator();
            Color[] colors = new Color[6];

            Map map = mapGenerator.GenerateMapFromHeightMap(
                heightMapColors: testCase.HeightMapColors,
                heightMapWidth: testCase.HeightMapWidth,
                heightMapHeight: testCase.HeightMapHeight
            );

            if (testCase.ExpectedWidth.HasValue)
                Assert.That(map.Width, Is.EqualTo(testCase.ExpectedWidth));
            if (testCase.ExpectedHeight.HasValue)
                Assert.That(
                    map.Height, Is.EqualTo(testCase.ExpectedHeight)
                );

            if (testCase.ExpectedTileTypes != null)
            {
                var tileTypes = new List<MapTileType>();
                map.ForEachTile((x, y, tile) => tileTypes.Add(tile.Type));
                Assert.That(
                    tileTypes.ToArray(),
                    Is.EqualTo(testCase.ExpectedTileTypes)
                );
            }

            if (testCase.ExpectedTileHeights != null)
            {
                var tileHeights = new List<float>();
                map.ForEachTile(
                    (x, y, tile) => tileHeights.Add(tile.Height)
                );
                Assert.That(
                    tileHeights.ToArray(),
                    Is.EqualTo(testCase.ExpectedTileHeights)
                );
            }
        }

        // get grayscale color with R=G=B and A=1.0f
        private static Color GetGrayColor(float rgb)
        {
            Color color = new Color(rgb, rgb, rgb, a: 1.0f);
            return color;
        }
    }
}
