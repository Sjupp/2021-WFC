using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private Vector2Int _coordinates = new Vector2Int();
    [SerializeField]
    private List<BasicTile> _possibleTiles = new List<BasicTile>();

    [SerializeField]
    private SpriteRenderer _renderer = null;

    private List<SpriteRenderer> _renderers = new List<SpriteRenderer>(); 
    private bool _collapsed = false;

    public bool Collapsed { get => _collapsed; }
    public Vector2Int Coordinates { get => _coordinates; }
    public List<BasicTile> PossibleTiles { get => _possibleTiles; }

    public void Init(Vector2Int coords, List<BasicTile> tiles)
    {
        _coordinates = coords;
        _possibleTiles = new List<BasicTile>(tiles);

        CreateRenderers();
    }
    
    public int GetCellEntropy()
    {
        return _possibleTiles.Count;
    }

    public void Collapse()
    {
        int indexOfSavedTile = Random.Range(0, _possibleTiles.Count);
        _possibleTiles.RemoveRange(indexOfSavedTile + 1, _possibleTiles.Count - (indexOfSavedTile + 1));
        _possibleTiles.RemoveRange(0, _possibleTiles.Count - 1);
        SetCollapsed();
    }

    public void SetCollapsed()
    {
        _collapsed = true;
        Render();
    }

    private void CreateRenderers()
    {
        int y = 0;
        for (int i = 0; i < _possibleTiles.Count; i++)
        {
            int gridSize = CalculateDisplayGrid();

            float offset = -(1f / 2f);

            float segmentSize = (1f / ((float)gridSize + 1f));

            float moduloAmountX = 1 + ((i % gridSize));

            float moduloAmountY = 1f + (y);

            Vector3 pos = transform.position + new Vector3(
                offset + segmentSize * moduloAmountX,
                0,
                offset + segmentSize * moduloAmountY);

            var obj = Instantiate(_renderer, pos, gameObject.transform.rotation, gameObject.transform);
            _renderers.Add(obj);
            Render();

            if (i % gridSize == gridSize - 1)
                y++;
        }
    }

    private int CalculateDisplayGrid()
    {
        int gridSize = 0;

        var count = _possibleTiles.Count;

        if (count == 1)
        {
            gridSize = 1;
        }
        else if (count >= 2 && count <= 4)
        {
            gridSize = 2;
        }
        else if (count >= 5 && count <= 9)
        {
            gridSize = 3;
        }
        else if (count >= 10 && count <= 16)
        {
            gridSize = 4;
        }
        else if (count >= 17 && count <= 25)
        {
            gridSize = 5;
        }

        return gridSize;
    }

    public void Render()
    {
        if (Collapsed)
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = _possibleTiles[0].Sprite;

            foreach (var item in _renderers)
            {
                item.sprite = null;
            }
            return;
        }
        else
        {
            for (int i = 0; i < _renderers.Count; i++)
            {
                if (i < _possibleTiles.Count)
                    _renderers[i].sprite = _possibleTiles[i].Sprite;
                else
                    _renderers[i].sprite = null;
            }
        }
    }
}
