using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private Vector2Int _coordinates = new Vector2Int();
    [SerializeField]
    private List<BasicTile> _possibleTiles = new List<BasicTile>();
    [SerializeField]
    private List<SpriteRenderer> _renderers = null;

    private bool _collapsed = false;

    public bool Collapsed { get => _collapsed; }
    public Vector2Int Coordinates { get => _coordinates; }
    public List<BasicTile> PossibleTiles { get => _possibleTiles; }

    public void Init(Vector2Int coords, List<BasicTile> tiles)
    {
        _coordinates = coords;
        _possibleTiles = new List<BasicTile>(tiles);
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

    public void Render()
    {
        if (Collapsed)
        {
            GetComponent<SpriteRenderer>().sprite = _possibleTiles[0].Sprite;

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
