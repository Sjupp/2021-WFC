using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private Vector2Int _coordinates = new Vector2Int();
    private List<BasicTile> _tiles = new List<BasicTile>();

    public Vector2Int Coordinates { get => _coordinates; }
    public List<BasicTile> Tiles { get => _tiles; }

    public void Init(Vector2Int coords, List<BasicTile> tiles)
    {
        _coordinates = coords;
        _tiles = tiles;
    }
    
    public int GetCellEntropy()
    {
        return _tiles.Count;
    }
}
