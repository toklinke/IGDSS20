using System;
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

    // the tile has been clicked on.
    public event EventHandler<EventArgs> TileClicked;


    void OnMouseDown()
    {
        OnTileClicked();
    }

    private void OnTileClicked()
    {
        EventHandler<EventArgs> handler = TileClicked;
        if (handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }
}
