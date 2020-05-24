using System;

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
