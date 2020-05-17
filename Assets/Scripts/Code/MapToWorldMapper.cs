using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Maps map tile (x, y) coordinates to world space (x, y, z) coordinates.
// World space coordinates are defined
// so that the anchor of each tile is the center of its top face.
public class MapToWorldMapper
{
    // Configure mapper.
    public MapToWorldMapper(
        float minWorldY, float maxWorldY, float tileRadius
    )
    {
        MinWorldY = minWorldY;
        MaxWorldY = maxWorldY;
        TileRadius = tileRadius;
        // assume regular hexagon
        TileOuterRadius = (float)(tileRadius * 2.0 / Math.Sqrt(3));
    }

    // minimum Y world space coordinate for placing tiles
    public float MinWorldY { get; }
    // maximum Y world space coordinate for placing tiles
    public float MaxWorldY { get; }
    // Radius of the hexagonal grid tiles
    public float TileRadius { get; }

    private float TileOuterRadius;

    // Get world position of a tile
    // that is located at map position (mapX, mapY).
    public Vector3 GetWorldPosition(uint mapX, uint mapY, MapTile tile)
    {
        // map Y axis -> world X axis
        float worldX = TileRadius + mapY * TileOuterRadius * 1.5f;

        float worldY = mapNumberRange(
            number: tile.Height,
            fromLow: 0.0f, fromHigh: 1.0f,
            toLow: MinWorldY, toHigh: MaxWorldY
        );

        // map X axis -> world Z axis
        bool isEvenRow = ((mapY % 2) == 0);
        float zOffset = isEvenRow ? TileRadius : 2.0f * TileRadius;
        float worldZ = zOffset + mapX * TileRadius * 2.0f;

        var worldPos = new Vector3(worldX, worldY, worldZ);
        return worldPos;
    }

    private float mapNumberRange(
        float number,
        float fromLow, float fromHigh, float toLow, float toHigh
    )
    {
        float mappedNumber;
        mappedNumber = (
            (number - fromLow) / (fromHigh - fromLow) *
            (toHigh - toLow) + toLow
        );
        return mappedNumber;
    }
}
