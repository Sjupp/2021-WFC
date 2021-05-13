using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Directions
{
    North,
    South,
    West,
    East
}

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
    
        // One iteration
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
            var currentCoords = stack.Pop();

            var validDirections = ValidDirections(currentCoords);
            foreach (Directions direction in validDirections)
            {
                var othercoords = GetCoordinateInDirection(currentCoords, direction);

                var otherPossibleTiles = new List<BasicTile>(_cells[othercoords].PossibleTiles);
                
                var possibleNeighborTiles = new List<BasicTile>(ValidNeighborTiles(currentCoords, othercoords));

                foreach (BasicTile tile in otherPossibleTiles)
                {
                    if (!otherPossibleTiles.Any(x => x == tile))
                    {
                        Constrain(othercoords, tile);
                        if (!stack.Contains(othercoords))
                            stack.Push(othercoords);
                    }
                }
            }
        }
    }

    private void Constrain(Vector2Int coordinates, BasicTile tile)
    {
        Debug.Log("Constraining, nr before: " + _cells[coordinates].PossibleTiles.Count);
        _cells[coordinates].PossibleTiles.Remove(tile);
        Debug.Log("Constraining, nr after: " + _cells[coordinates].PossibleTiles.Count);
    }

    private List<Directions> ValidDirections(Vector2Int coordinates)
    {
        List<Directions> list = new List<Directions>();

        for (int i = 0; i < 4; i++)
        {
            Vector2Int otherCoords = GetCoordinateInDirection(coordinates, (Directions)i);

            if (otherCoords.x < 0 || otherCoords.x > _size.x || otherCoords.y < 0 || otherCoords.y > _size.y)
                continue;
            
            list.Add((Directions)i);
        }

        return list;
    }

    private Vector2Int GetCoordinateInDirection(Vector2Int fromCoordinate, Directions direction)
    {
        Vector2Int otherCoords = new Vector2Int();

        switch (direction)
        {
            case Directions.North:
                otherCoords = new Vector2Int(fromCoordinate.x, fromCoordinate.y + 1);
                break;
            case Directions.South:
                otherCoords = new Vector2Int(fromCoordinate.x, fromCoordinate.y - 1);
                break;
            case Directions.West:
                otherCoords = new Vector2Int(fromCoordinate.x - 1, fromCoordinate.y);
                break;
            case Directions.East:
                otherCoords = new Vector2Int(fromCoordinate.x + 1, fromCoordinate.y);
                break;
        }

        return otherCoords;
    }

    private List<BasicTile> ValidNeighborTiles(Vector2Int currentCoordinates, Vector2Int otherCoordinates)
    {
        List<BasicTile> tiles = new List<BasicTile>();



        return tiles;
    }
}
