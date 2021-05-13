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

        CollapseCell(coord);
        Propagate(coord);
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

    private void Propagate(Vector2Int coord)
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(coord);

        while (stack.Count > 0)
        {
            var current = stack.Pop();

            var neighbors = ValidNeighbors(current);

            foreach (Vector2Int coordinate in neighbors)
            {

            }


            _cells.TryGetValue(otherCoords, out Cell otherCell);
            var hej = otherCell.PossibleTiles;

        }
    }

    private List<Vector2Int> ValidNeighbors(Vector2Int coordinates)
    {
        List<Vector2Int> list = new List<Vector2Int>();

        for (int i = 0; i < 4; i++)
        {
            Vector2Int otherCoords = new Vector2Int();

            switch (i)
            {
                case 0:
                    // north
                    otherCoords = new Vector2Int(coordinates.x, coordinates.y + 1);
                    break;
                case 1:
                    // south
                    otherCoords = new Vector2Int(coordinates.x, coordinates.y - 1);
                    break;
                case 2:
                    // east
                    otherCoords = new Vector2Int(coordinates.x + 1, coordinates.y);
                    break;
                case 3:
                    // west
                    otherCoords = new Vector2Int(coordinates.x - 1, coordinates.y);
                    break;
                default:
                    break;
            }

            if (otherCoords.x < 0 || otherCoords.x > _size.x || otherCoords.y < 0 || otherCoords.y > _size.y)
            {
                Debug.Log(otherCoords + " is outside the grid");
                continue;
            }

            Debug.Log("Adding " + otherCoords);
            list.Add(otherCoords);
        }

        return list;
    }
}
