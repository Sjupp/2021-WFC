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
    private float _tileSpacing = 1.0f;
    [SerializeField]
    private float _intervalTime = 0.1f;
    [SerializeField]
    private Cell _cellPrefab = null;

    private Dictionary<Vector2Int, Cell> _cells = new Dictionary<Vector2Int, Cell>();

    private void Start()
    {
        //StartCoroutine(WFC());

        Initialize(_size, _tiles);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(WFC());
        }
    }

    private IEnumerator WFC()
    {
        ClearPrevious();

        Initialize(_size, _tiles);
        
        yield return new WaitForSeconds(_intervalTime);

        while (!IsCollapsed())
        {
            Iterate();
            yield return new WaitForSeconds(_intervalTime);
        }

        Debug.Log("Done");
    }

    private void ClearPrevious()
    {
        _cells.Clear();

        if (gameObject.transform.childCount > 0)
            foreach (Transform child in gameObject.transform)
                Destroy(child.gameObject);
    }

    private void Initialize(Vector2Int fieldSize, List<BasicTile> tiles)
    {
        for (int y = 0; y < fieldSize.y; y++)
        {
            for (int x = 0; x < fieldSize.x; x++)
            {
                var obj = Instantiate(_cellPrefab, new Vector3(x * _tileSpacing, 0, y * _tileSpacing), Quaternion.Euler(90, 0, 0), gameObject.transform);
                Vector2Int coord = new Vector2Int(x, y);
                _cells.Add(coord, obj);
                obj.Init(coord, _tiles);
                obj.gameObject.name = "Cell " + x + ", " + y;
            }
        }
    }

    private void Iterate()
    {
        var coord = GetMinimumEntropy();
        CollapseCell(coord);
        Propagate(coord);
        
        Render();
        
        if (IsCollapsed())
            Debug.LogWarning("We're done!");
    }

    private void Render()
    {
        foreach (Cell cell in _cells.Values)
        {
            cell.Render();
        }
    }

    private Vector2Int GetMinimumEntropy()
    {
        List<Cell> cells = new List<Cell>();
        int minEnt = 1000;
        foreach (Cell cell in _cells.Values.Where(x => x.Collapsed == false))
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
            Debug.Log($"{cells.Count} Cells share the lowest entropy value, collapsing random");
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
                
                var possibleNeighborTiles = new List<BasicTile>(ValidNeighborTiles(currentCoords, direction));

                foreach (BasicTile tile in otherPossibleTiles)
                {
                    //Debug.Log("Checking if " + tile.name + " in " + othercoords + " is a possible neighbor for " + currentCoords);

                    if (!possibleNeighborTiles.Contains(tile))
                        {
                        //Debug.Log("It was not, removing " + tile.name + " from " + othercoords);

                        Constrain(othercoords, tile);

                        if (!stack.Contains(othercoords))
                        {
                            //Debug.Log(othercoords + " has been modified, adding it to the stack");
                            stack.Push(othercoords);
                        }
                    }
                    else
                    {
                        //Debug.Log("It was, keeping " + tile.name + " in " + othercoords);
                    }
                }
            }
        }
    }

    private void Constrain(Vector2Int coordinates, BasicTile tile)
    {
        var cell = _cells[coordinates];
        cell.PossibleTiles.Remove(tile);

        if (cell.PossibleTiles.Count == 1)
        {
            cell.SetCollapsed();
        }
    }

    private List<Directions> ValidDirections(Vector2Int coordinates)
    {
        List<Directions> list = new List<Directions>();

        for (int i = 0; i < 4; i++)
        {
            Vector2Int otherCoords = GetCoordinateInDirection(coordinates, (Directions)i);

            if (otherCoords.x < 0 || otherCoords.x >= _size.x || otherCoords.y < 0 || otherCoords.y >= _size.y)
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

    private List<BasicTile> ValidNeighborTiles(Vector2Int fromCoordinate, Directions direction)
    {
        List<BasicTile> tiles = new List<BasicTile>();

        var cell = _cells[fromCoordinate];

        switch (direction)
        {
            case Directions.North:
                foreach (BasicTile possibleTile in cell.PossibleTiles)
                {
                    foreach (int viableNeighborTileId in possibleTile.NorthNeighbors)
                    {
                        var tile = _tiles[viableNeighborTileId];
                        if (!tiles.Contains(tile))
                            tiles.Add(tile);
                    }
                }
                break;
            case Directions.South:
                foreach (BasicTile possibleTile in cell.PossibleTiles)
                {
                    foreach (int viableNeighborTileId in possibleTile.SouthNeighbors)
                    {
                        var tile = _tiles[viableNeighborTileId];
                        if (!tiles.Contains(tile))
                            tiles.Add(tile);
                    }
                }
                break;
            case Directions.West:
                foreach (BasicTile possibleTile in cell.PossibleTiles)
                {
                    foreach (int viableNeighborTileId in possibleTile.WestNeighbors)
                    {
                        var tile = _tiles[viableNeighborTileId];
                        if (!tiles.Contains(tile))
                            tiles.Add(tile);
                    }
                }
                break;
            case Directions.East:
                foreach (BasicTile possibleTile in cell.PossibleTiles)
                {
                    foreach (int viableNeighborTileId in possibleTile.EastNeighbors)
                    {
                        var tile = _tiles[viableNeighborTileId];
                        if (!tiles.Contains(tile))
                            tiles.Add(tile);
                    }
                }
                break;
        }

        return tiles;
    }

    private bool IsCollapsed()
    {
        foreach (Cell cell in _cells.Values)
        {
            if (!cell.Collapsed)
                return false;
        }

        return true;
    }

}
