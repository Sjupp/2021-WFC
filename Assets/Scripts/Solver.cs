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

    private List<Cell> _cells = new List<Cell>();

    private void Start()
    {
        Initialize(_size, _tiles);
        var coord = GetMinimumEntropy();
        CollapseCell(coord);
    }

    private void Initialize(Vector2Int fieldSize, List<BasicTile> tiles)
    {
        for (int y = 0; y < fieldSize.y; y++)
        {
            for (int x = 0; x < fieldSize.x; x++)
            {
                var obj = Instantiate(_cellPrefab, new Vector3(x, 0, y), Quaternion.Euler(90, 0, 0));
                _cells.Add(obj);
                obj.Init(new Vector2Int(x, y), _tiles);
            }
        }
    }

    private Vector2Int GetMinimumEntropy()
    {
        List<Cell> cells = new List<Cell>();
        int minEnt = 1000;
        foreach (var cell in _cells)
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
            Debug.Log($"Randomly selected Cell at coordinates: {randomCellCoordinates}");
            return randomCellCoordinates;
        }
        else
        {
            Debug.Log($"Selected Cell at coordinates: {cells[0].Coordinates}");
            return cells[0].Coordinates;
        }
    }

    private void CollapseCell(Vector2Int coord)
    {

    }
}
