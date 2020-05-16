using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MapTileType
{
    Water,
    Sand,
    Grass,
    Forest,
    Stone,
    Mountain
}


public struct MapTile
{
    public MapTile(float height, MapTileType type)
    {
        Height = height;
        Type = type;
    }

    public float Height { get; }
    public MapTileType Type { get; }

    public override string ToString()
    {
        string result = string.Format(
            "MapTile(height: {0}, type: {1})", Height, Type
        );
        return result;
    }
}


// A game map consisting of a rectangular grid of hexagonal tiles.
// Each tile is assigned an (x, y) position in the grid.
// Position (0, 0) is at the top left of the map when viewed from above.
public class Map
{
    public Map(MapTile[] tiles, uint width, uint height)
    {
        _tiles = tiles;
        Width = width;
        Height = height;
    }

    public uint Width { get; }
    public uint Height { get; }

    // Iterate over all tiles with their map positions (x, y).
    // action: Called for each tile with map position and tile.
    public void ForEachTile(Action<uint, uint, MapTile> action)
    {
        for(uint y = 0; y < Height; ++y)
        {
            for(uint x = 0; x < Width; ++x)
            {
                MapTile tile = _tiles[y * Width + x];
                action(x, y, tile);
            }
        }
    }

    private MapTile[] _tiles;
}


// Generates a map from a heightmap.
public class MapGenerator
{
    public Map GenerateMapFromHeightMap(
        Color[] heightMapColors, uint heightMapWidth, uint heightMapHeight
    )
    {
        MapTile[] tiles = new MapTile[heightMapWidth * heightMapHeight];

        for(uint y = 0; y < heightMapHeight; ++y)
        {
            for(uint x = 0; x < heightMapWidth; ++x)
            {
                uint i = y * heightMapWidth + x;
                Color color = heightMapColors[i];
                tiles[i] = GetMapTile(color);
            }
        }

        Map map = new Map(
            tiles: tiles,
            width: heightMapWidth,
            height: heightMapHeight
        );
        return map;
    }

    // Get map tile for color.
    // Color is assumed to be grayscale with R=G=B.
    private static MapTile GetMapTile(Color color)
    {
        // take any color channel as gray value
        float gray = color.r;

        var tile = new MapTile(
            height: gray,
            type: GetTileType(gray)
        );
        return tile;
    }

    // Get tile type for gray value.
    private static MapTileType GetTileType(float gray)
    {
        MapTileType tileType;
        if (gray == 0.0f)
        {
            tileType = MapTileType.Water;
        }
        else if (gray <= 0.2f)
        {
            tileType = MapTileType.Sand;
        }
        else if (gray <= 0.4f)
        {
            tileType = MapTileType.Grass;
        }
        else if (gray <= 0.6f)
        {
            tileType = MapTileType.Forest;
        }
        else if (gray <= 0.8f)
        {
            tileType = MapTileType.Stone;
        }
        else if (gray <= 1.0f)
        {
            tileType = MapTileType.Mountain;
        }
        else {
            throw new ArgumentOutOfRangeException(
                "color", "gray value must be in range 0.0 to 1.0"
            );
        }
        return tileType;
    }
}
