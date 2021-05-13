using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solver : MonoBehaviour
{
    [SerializeField]
    private List<BasicTile> _tiles = new List<BasicTile>();
    [SerializeField]
    private Vector2Int _size = new Vector2Int();

    [SerializeField]
    private Cell _cellPrefab = null;

    private Dictionary<Vector2Int, Cell> _cells = new Dictionary<Vector2Int, Cell>();

    private void Start()
    {
        Initialize(_size, _tiles);
        var coord = GetMinimumEntropy();
        
        foreach (var item in _cells.Values)
        {
            Debug.Log(item.name + " has entropy " + item.GetCellEntropy());
        }

        CollapseCell(coord);

        foreach (var item in _cells.Values)
        {
            Debug.Log(item.name + " has entropy " + item.GetCellEntropy());
        }
    }

    private void Initialize(Vector2Int fieldSize, List<BasicTile> tiles)
    {
        for (int y = 0; y < fieldSize.y; y++)
        {
            for (int x = 0; x < fieldSize.x; x++)
            {
                var obj = Instantiate(_cellPrefab, new Vector3(x, 0, y), Quaternion.Euler(90, 0, 0));
                Vector2Int coord = new Vector2Int(x, y);
                _cells.Add(coord, obj);
                obj.Init(coord, _tiles);
                obj.gameObject.name = "Cell " + x + ", " + y;
            }
        }
    }

    private Vector2Int GetMinimumEntropy()
    {
        List<Cell> cells = new List<Cell>();
        int minEnt = 1000;
        foreach (Cell cell in _cells.Values)
        {
            int cellEnt = cell.GetCellEntropy();
            if (cellEnt < minEnt)
            {
                minEnt = cellEnt;
                if (cells.Count > 0)
                    cells.Clear();
                cells.Add(cell);
            }
            else if (cellEnt == minEnt)
            {
                cells.Add(cell);
            }
        }

        Debug.Log($"Minimum entropy found: {minEnt}");

        if (cells.Count > 1)
        {
            Debug.Log($"{cells.Count} Cells share the lowest entropy value");
            var randomCellCoordinates = cells[UnityEngine.Random.Range(0, cells.Count)].Coordinates;
            return randomCellCoordinates;
        }
        else
        {
            return cells[0].Coordinates;
        }
    }

    private void CollapseCell(Vector2Int coord)
    {
        Debug.Log($"Collapsing at coordinates: {coord}");
        _cells.TryGetValue(coord, out Cell cell);
        cell.Collapse();
    }
}
