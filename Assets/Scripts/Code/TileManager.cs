using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    // The associated map tile.
    public MapTile Tile { get; set; }

    // The map position of the tile
    public uint MapX { get; set; }
    public uint MapY { get; set; }

    void OnMouseDown()
    {
        Debug.Log($"clicked on tile: {Tile.Type} at map pos {MapX}, {MapY}");
    }



}
