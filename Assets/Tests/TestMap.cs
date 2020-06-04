using NUnit.Framework;
using System.Collections.Generic;

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
}
