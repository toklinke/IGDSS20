﻿using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestMapToWorldMapper
    {
        public readonly struct GetWorldPositionTestCase
        {
            public GetWorldPositionTestCase(
                float minWorldY, float maxWorldY,
                float tileRadius,
                uint mapX, uint mapY,
                MapTile tile,
                Vector3 expectedWorldPosition
            )
            {
                MinWorldY = minWorldY;
                MaxWorldY = maxWorldY;
                TileRadius = tileRadius;
                MapX = mapX;
                MapY = mapY;
                Tile = tile;
                ExpectedWorldPosition = expectedWorldPosition;
            }

            public float MinWorldY { get; }
            public float MaxWorldY { get; }
            public float TileRadius { get; }

            public uint MapX { get; }
            public uint MapY { get; }
            public MapTile Tile { get; }

            public Vector3 ExpectedWorldPosition { get; }
        };

        private static GetWorldPositionTestCase[] GetWorldPositionTestCases
            = new GetWorldPositionTestCase[] {
            // just top left tile
            new GetWorldPositionTestCase(
                minWorldY: 0.0f,
                maxWorldY: 1.0f,
                tileRadius: 1.0f,
                mapX: 0,
                mapY: 0,
                tile: new MapTile(height: 0.42f, type: MapTileType.Grass),
                expectedWorldPosition: new Vector3(1.0f, 0.42f, 1.0f)
            ),
            // Test that height is scaled to [minWorldY, maxWorldY]
            new GetWorldPositionTestCase(
                minWorldY: 10.0f,
                maxWorldY: 100.0f,
                tileRadius: 1.0f,
                mapX: 0,
                mapY: 0,
                tile: new MapTile(height: 0.5f, type: MapTileType.Grass),
                expectedWorldPosition: new Vector3(1.0f, 55.0f, 1.0f)
            ),
            // Test odd row number
            new GetWorldPositionTestCase(
                minWorldY: 0.0f,
                maxWorldY: 1.0f,
                tileRadius: 1.0f,
                mapX: 0,
                mapY: 1,
                tile: new MapTile(height: 0.42f, type: MapTileType.Grass),
                expectedWorldPosition: new Vector3(
                    x: 1.0f + 2.0f / (float)Math.Sqrt(3) * 1.5f,
                    y: 0.42f,
                    z: 2.0f
                )
            ),
            // Test arbitrary tile position
            new GetWorldPositionTestCase(
                minWorldY: 0.0f,
                maxWorldY: 1.0f,
                tileRadius: 5.0f,
                mapX: 1,
                mapY: 2,
                tile: new MapTile(height: 0.5f, type: MapTileType.Grass),
                expectedWorldPosition: new Vector3(
                    x: 5.0f + 2.0f * (5.0f * 2.0f / (float)Math.Sqrt(3) * 1.5f),
                    y: 0.5f,
                    z: 15.0f
                )
            ),
        };

        [Test, TestCaseSource("GetWorldPositionTestCases")]
        public void TestGetWorldPosition(GetWorldPositionTestCase testCase)
        {
            var mapper = new MapToWorldMapper(
                minWorldY: testCase.MinWorldY,
                maxWorldY: testCase.MaxWorldY,
                tileRadius: testCase.TileRadius
            );

            var worldPosition = mapper.GetWorldPosition(
                mapX: testCase.MapX,
                mapY: testCase.MapY,
                tile: testCase.Tile
            );

            Assert.That(
                worldPosition,
                Is.EqualTo(testCase.ExpectedWorldPosition)
            );
        }


        public readonly struct GetWorldSizeTestCase
        {
            public GetWorldSizeTestCase(
                float minWorldY, float maxWorldY,
                float tileRadius,
                uint mapWidth, uint mapHeight,
                Vector3 expectedWorldSize
            )
            {
                MinWorldY = minWorldY;
                MaxWorldY = maxWorldY;
                TileRadius = tileRadius;
                MapWidth = mapWidth;
                MapHeight = mapHeight;
                ExpectedWorldSize = expectedWorldSize;
            }

            public float MinWorldY { get; }
            public float MaxWorldY { get; }
            public float TileRadius { get; }

            public uint MapWidth { get; }
            public uint MapHeight { get; }

            public Vector3 ExpectedWorldSize { get; }
        };

        private static GetWorldSizeTestCase[] GetWorldSizeTestCases
            = new GetWorldSizeTestCase[] {
            // just one tile
            new GetWorldSizeTestCase(
                minWorldY: 0.0f,
                maxWorldY: 1.0f,
                tileRadius: 1.0f,
                mapWidth: 1,
                mapHeight: 1,
                expectedWorldSize: new Vector3(
                    x: 2.0f / (float)Math.Sqrt(3) * 2.0f,
                    y: 1.0f,
                    z: 2.0f
                )
            ),
            // Test Y world size
            new GetWorldSizeTestCase(
                minWorldY: 5.0f,
                maxWorldY: 55.0f,
                tileRadius: 1.0f,
                mapWidth: 1,
                mapHeight: 1,
                expectedWorldSize: new Vector3(
                    x: 2.0f / (float)Math.Sqrt(3) * 2.0f,
                    y: 50.0f,
                    z: 2.0f
                )
            ),
            // Test world width single row
            new GetWorldSizeTestCase(
                minWorldY: 0.0f,
                maxWorldY: 1.0f,
                tileRadius: 5.0f,
                mapWidth: 2,
                mapHeight: 1,
                expectedWorldSize: new Vector3(
                    x: 5.0f * 2.0f / (float)Math.Sqrt(3) * 2.0f,
                    y: 1.0f,
                    z: 20.0f
                )
            ),
            // Test world size with more than one row
            new GetWorldSizeTestCase(
                minWorldY: 0.0f,
                maxWorldY: 1.0f,
                tileRadius: 5.0f,
                mapWidth: 2,
                mapHeight: 2,
                expectedWorldSize: new Vector3(
                    x: 5.0f * 2.0f / (float)Math.Sqrt(3) * 3.5f,
                    y: 1.0f,
                    z: 25.0f
                )
            ),
            // Test world size with more than two rows
            new GetWorldSizeTestCase(
                minWorldY: 0.0f,
                maxWorldY: 1.0f,
                tileRadius: 5.0f,
                mapWidth: 1,
                mapHeight: 4,
                expectedWorldSize: new Vector3(
                    x: 5.0f * 2.0f / (float)Math.Sqrt(3) * 7.0f,
                    y: 1.0f,
                    z: 15.0f
                )
            ),
        };
        [Test, TestCaseSource("GetWorldSizeTestCases")]
        public void TestGetWorldSize(GetWorldSizeTestCase testCase)
        {
            var mapper = new MapToWorldMapper(
                minWorldY: testCase.MinWorldY,
                maxWorldY: testCase.MaxWorldY,
                tileRadius: testCase.TileRadius
            );

            var worldSize = mapper.GetWorldSize(
                mapWidth: testCase.MapWidth,
                mapHeight: testCase.MapHeight
            );

            Assert.That(
                worldSize,
                Is.EqualTo(testCase.ExpectedWorldSize)
            );
        }
    }
}
