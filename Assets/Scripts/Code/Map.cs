using System;
using System.Collections.Generic;
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
                _neighborList.Add(returnListOfNeighbors(x, y, Width));
                action(x, y, tile);
            }
        }
    }

    public List<uint> getNeighboursOfTile(uint x, uint y)
    {

        int arrayIndex = (int)CalcArrayIndex(x, y, Height);
        return _neighborList[arrayIndex];

    }


    private uint CalcArrayIndex(uint xPos, uint yPos, uint xDimension)
    {
        return (yPos * xDimension) + xPos;
    }
    private List<uint> returnListOfNeighbors(uint xPos, uint yPos, uint xDimension)
    {
        uint calculatedIndex = CalcArrayIndex(xPos, yPos, xDimension);

        List<uint> neighborList = new List<uint>();

        bool LowestRow = false;
        bool HighestRow = false;
        bool leftmostColum = false;
        bool rightMostColumn = false;

        if (yPos == 0)
        {
            LowestRow = true;
        }
        else if (yPos == (xDimension - 1))
        {
            HighestRow = true;
        }

        if (xPos == 0)
        {
            leftmostColum = true;
        }
        if (xPos == (xDimension - 1))
        {
            rightMostColumn = true;
        }

        //UnityEngine.Debug.Log($"LowestRow Index: {LowestRow} HighestRow {HighestRow} leftmostColum {leftmostColum} rightMostColumn {rightMostColumn} ");


        if ((yPos + 1) % 2 == 0) // Number is even +1 due to Indices starting at 0
        {
            UnityEngine.Debug.Log("Even Row");
            if (!leftmostColum)
            {
                neighborList.Add(CalcArrayIndex(xPos - 1, yPos, xDimension));
            }
            if (!rightMostColumn)
            {
                neighborList.Add(CalcArrayIndex(xPos + 1, yPos, xDimension));
            }


            if (!LowestRow)
            {
                neighborList.Add(CalcArrayIndex(xPos, yPos - 1, xDimension));
            }

            if (!rightMostColumn && !LowestRow)
            {
                neighborList.Add(CalcArrayIndex(xPos + 1, yPos - 1, xDimension));
            }

            if (!rightMostColumn && !HighestRow)
            {
                neighborList.Add(CalcArrayIndex(xPos, yPos + 1, xDimension));
            }

            if (!HighestRow)
            {
                neighborList.Add(CalcArrayIndex(xPos + 1, yPos + 1, xDimension));
            }


        }
        else // Number is odd
        {
            UnityEngine.Debug.Log("Uneven Row");

            if (!leftmostColum)
            {
                neighborList.Add(CalcArrayIndex(xPos - 1, yPos, xDimension));
            }
            if (!rightMostColumn)
            {
                neighborList.Add(CalcArrayIndex(xPos + 1, yPos, xDimension));
            }

            if (!LowestRow && !leftmostColum)
            {
                neighborList.Add(CalcArrayIndex(xPos - 1, yPos - 1, xDimension));
            }

            if (!LowestRow)
            {
                neighborList.Add(CalcArrayIndex(xPos, yPos - 1, xDimension));
            }

            if (!HighestRow && !leftmostColum)
            {
                neighborList.Add(CalcArrayIndex(xPos, yPos + 1, xDimension));
            }

            if (!HighestRow)
            {
                neighborList.Add(CalcArrayIndex(xPos + 1, yPos + 1, xDimension));
            }

        }
        UnityEngine.Debug.Log($"Calculated Number of neighbors: {neighborList.Count}");

        return neighborList;

    }



















}
