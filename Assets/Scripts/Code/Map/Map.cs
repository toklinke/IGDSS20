using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;

// A game map consisting of a rectangular grid of hexagonal tiles.
// Each tile is assigned an (x, y) position in the grid.
// Position (0, 0) is at the top left of the map when viewed from above.
public class Map
{
    private List<List<uint>> _neighborList = new List<List<uint>>();

    private MapTile[] _tiles;


    public Map(MapTile[] tiles, uint width, uint height)
    {
        _tiles = tiles;
        Width = width;
        Height = height;

        for (uint y = 0; y < Height; ++y)
        {
            for (uint x = 0; x < Width; ++x)
            {
                //UnityEngine.Debug.Log(returnListOfNeighbors(x, y, Width).Count);
                _neighborList.Add(returnListOfNeighbors(x, y, Width));
            }
        }
    }
    public uint Width { get; }
    public uint Height { get; }

    // Iterate over all tiles with their map positions (x, y).
    // action: Called for each tile with map position and tile.
    public void ForEachTile(Action<uint, uint, MapTile> action)
    {
        for (uint y = 0; y < Height; ++y)
        {
            for (uint x = 0; x < Width; ++x)
            {
                MapTile tile = _tiles[y * Width + x];
                action(x, y, tile);
            }
        }
    }

    public MapTile GetTile(uint x, uint y)
    {
        uint index = CalcArrayIndex(x, y, Width);
        var tile = _tiles[index];
        return tile;
    }

    public List<MapTile> getNeighboursOfTile(uint x, uint y)
    {

        int arrayIndex = (int)CalcArrayIndex(x, y, Width);

        List<uint> tileNumberList = _neighborList[arrayIndex];
        List<MapTile> tileObjectList = new List<MapTile>();

        foreach (uint tileNumber in tileNumberList)
        {
            tileObjectList.Add(_tiles[(int)tileNumber]);
        }

        return tileObjectList;
    }


    private uint CalcArrayIndex(uint xPos, uint yPos, uint width)
    {
        return (yPos * width) + xPos;
    }
    private List<uint> returnListOfNeighbors(uint xPos, uint yPos, uint width)
    {
        uint calculatedIndex = CalcArrayIndex(xPos, yPos, width);

        List<uint> neighborList = new List<uint>();

        bool LowestRow = false;
        bool HighestRow = false;
        bool leftmostColum = false;
        bool rightMostColumn = false;

        if (yPos == 0)
        {
            LowestRow = true;
        }
        if (yPos == (Height - 1))
        {
            HighestRow = true;
        }

        if (xPos == 0)
        {
            leftmostColum = true;
        }
        if (xPos == (width - 1))
        {
            rightMostColumn = true;
        }

        //UnityEngine.Debug.Log($"LowestRow Index: {LowestRow} HighestRow {HighestRow} leftmostColum {leftmostColum} rightMostColumn {rightMostColumn} ");


        if ((yPos + 1) % 2 == 0) // Number is even +1 due to Indices starting at 0
        {
            //UnityEngine.Debug.Log("Even Row");
            if (!leftmostColum)
            {
                neighborList.Add(CalcArrayIndex(xPos - 1, yPos, width));
            }
            if (!rightMostColumn)
            {
                neighborList.Add(CalcArrayIndex(xPos + 1, yPos, width));
            }


            if (!LowestRow)
            {
                neighborList.Add(CalcArrayIndex(xPos, yPos - 1, width));
            }

            if (!rightMostColumn && !LowestRow)
            {
                neighborList.Add(CalcArrayIndex(xPos + 1, yPos - 1, width));
            }

            if (!rightMostColumn && !HighestRow)
            {
                neighborList.Add(CalcArrayIndex(xPos, yPos + 1, width));
            }

            if (!HighestRow)
            {
                neighborList.Add(CalcArrayIndex(xPos + 1, yPos + 1, width));
            }


        }
        else // Number is odd
        {
            //UnityEngine.Debug.Log("Uneven Row");

            if (!leftmostColum)
            {
                neighborList.Add(CalcArrayIndex(xPos - 1, yPos, width));
            }
            if (!rightMostColumn)
            {
                neighborList.Add(CalcArrayIndex(xPos + 1, yPos, width));
            }

            if (!LowestRow && !leftmostColum)
            {
                neighborList.Add(CalcArrayIndex(xPos - 1, yPos - 1, width));
            }

            if (!LowestRow)
            {
                neighborList.Add(CalcArrayIndex(xPos, yPos - 1, width));
            }

            if (!HighestRow && !leftmostColum)
            {
                neighborList.Add(CalcArrayIndex(xPos, yPos + 1, width));
            }

            if (!HighestRow)
            {
                neighborList.Add(CalcArrayIndex(xPos + 1, yPos + 1, width));
            }

        }
        //UnityEngine.Debug.Log($"Calculated Number of neighbors: {neighborList.Count}");

        return neighborList;

    }

}
