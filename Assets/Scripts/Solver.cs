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
        List<Cell> cells = null;
        int minEnt = 1000;
        foreach (var cell in _cells)
        {
            int cellEnt = cell.GetCellEntropy();
            if (cellEnt < minEnt)
            {
                minEnt = cellEnt;
                cells.Clear();
                cells.Add(cell);
            }
            else if (cellEnt == minEnt)
            {
                cells.Add(cell);
            }
        }

        if (cells.Count > 1)
            return cells[Random.Range(0, cells.Count)].Coordinates;
        else
            return cells[0].Coordinates;
    }
}
