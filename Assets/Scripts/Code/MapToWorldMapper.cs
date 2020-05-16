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
    }

    // minimum Y world space coordinate for placing tiles
    public float MinWorldY { get; }
    // maximum Y world space coordinate for placing tiles
    public float MaxWorldY { get; }
    // Radius of the hexagonal grid tiles
    public float TileRadius { get; }

    // Get world position of a tile
    // that is located at map position (mapX, mapY).
    public Vector3 GetWorldPosition(uint mapX, uint mapY, MapTile tile)
    {
        float worldX = TileRadius + mapX * TileRadius * 2.0f;
        float worldY = mapNumberRange(
            number: tile.Height,
            fromLow: 0.0f, fromHigh: 1.0f,
            toLow: MinWorldY, toHigh: MaxWorldY
        );
        float worldZ = TileRadius + mapY * TileRadius * 2.0f;
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
