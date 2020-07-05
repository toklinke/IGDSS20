using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NavigationManager
{

    private static uint _traversalWeightWater = 30;
    private static uint _traversalWeightSand = 2;
    private static uint _traversalWeightGrass = 1;
    private static uint _traversalWeighForest = 2;
    private static uint _traversalWeightStone = 1;
    private static uint _traversalWeightMountain = 3;

    /**
     * Method that creates a TravelWeight Hashmap and a predecessor Hashmap
     */
    public Tuple<Dictionary<Tile, Tile>, Dictionary<Tile, uint>> generateTravelMapForBuilding(Tile buildingTile)
    {
        // https://docs.microsoft.com/de-de/dotnet/api/system.collections.generic.queue-1?view=netcore-3.1

        // Current Tile + Future Tile
        Queue<Tuple<Tile, Tile>> toBeExploredTiles = new Queue<Tuple<Tile, Tile>>();
        // Use enqueue and dequeue
        Dictionary<Tile, Tile> predecessorHashmap = new Dictionary<Tile, Tile>();
        Dictionary<Tile, uint> weightHashmap = new Dictionary<Tile, uint>();


        List<Tile> initialNeighborList = buildingTile._neighborTiles;
        Tile currentTile = buildingTile;

        predecessorHashmap[buildingTile] = buildingTile;
        weightHashmap[buildingTile] = 0;

        foreach (Tile t in initialNeighborList)
        {
            toBeExploredTiles.Enqueue(new Tuple<Tile, Tile>(currentTile, t));
        }

        while (toBeExploredTiles.Count > 0)
        {
            Tuple<Tile, Tile> currentAndFutureTuple = toBeExploredTiles.Dequeue();

            exploreNeighbors(weightHashmap, predecessorHashmap, currentAndFutureTuple.Item2, currentAndFutureTuple.Item1, toBeExploredTiles);
        }

        Debug.Log(weightHashmap.Count);
        Debug.Log(predecessorHashmap.Count);
        // Check if explicit copy is needed here
        return new Tuple<Dictionary<Tile, Tile>, Dictionary<Tile, uint>>(predecessorHashmap, weightHashmap);
    }

    private static void exploreNeighbors(Dictionary<Tile, uint> travelweightTable, Dictionary<Tile, Tile> predecessorTable, Tile currentTile, Tile previousTile, Queue<Tuple<Tile, Tile>> foundTiles)
    {
        uint currentTileWeight = 0;
        travelweightTable.TryGetValue(currentTile, out currentTileWeight);
        //Debug.Log(currentTileWeight);


        // Tile has not been explored
        if (currentTileWeight == 0)
        {
            // Calculate Weight of current tile
            currentTileWeight = calcTraversalDifficulty(currentTile._type);

            uint totalWeight = currentTileWeight + travelweightTable[previousTile];
            travelweightTable[currentTile] = totalWeight;
            predecessorTable[currentTile] = previousTile;

            // Save tile to dictionary
            // Add neighbor tiles to queue

            foreach (Tile t in currentTile._neighborTiles)
            {

                if (t != previousTile)
                {
                    Tuple<Tile, Tile> currentAndFutureTuple = new Tuple<Tile, Tile>(currentTile, t);
                    foundTiles.Enqueue(currentAndFutureTuple);
                }


            }

        }
        else // Tile has been explored
        {
            // If new weight is lower then previously known, replace entry otherwise do nothing
            uint newTilePathWeight = travelweightTable[previousTile] + calcTraversalDifficulty(currentTile._type);

            if (travelweightTable[currentTile] > newTilePathWeight)
            {
                travelweightTable[currentTile] = newTilePathWeight;
                predecessorTable[currentTile] = previousTile;

            }
            else
            {

            }
        }


    }



    private static uint calcTraversalDifficulty(Tile.TileTypes tileType)
    {
        switch ((tileType)
)
        {
            case Tile.TileTypes.Water:
                {
                    return _traversalWeightWater;

                }
            case Tile.TileTypes.Forest:
                {
                    return _traversalWeighForest;

                }

            case Tile.TileTypes.Grass:
                {
                    return _traversalWeightGrass;

                }
            case Tile.TileTypes.Mountain:
                {
                    return _traversalWeightMountain;

                }
            case Tile.TileTypes.Sand:
                {
                    return _traversalWeightSand;

                }
            case Tile.TileTypes.Stone:
                {
                    return _traversalWeightStone;

                }
            default:
                return 0;
        }


    }

    private static List<Tile> getShortestPath(Dictionary<Tile, Tile> predecessorHashmap, Tile startingPoint, Tile goal)
    {
        List<Tile> easiestPathList = new List<Tile>();

        while (!startingPoint.Equals(goal))
        {
            startingPoint = predecessorHashmap[startingPoint];
        }

        return easiestPathList;
    }

}
