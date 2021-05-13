﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private Vector2Int _coordinates = new Vector2Int();
    [SerializeField]
    private List<BasicTile> _possibleTiles = new List<BasicTile>();

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

        GetComponent<SpriteRenderer>().sprite = _possibleTiles[0].Sprite;
        Debug.Log("Collapsed to " + _possibleTiles[0].name);
    }
}
