using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicTile", menuName = "", order = 1)]
public class BasicTile : ScriptableObject
{
    public Sprite Sprite = null;
    public int Id = -1;

    public List<int> NorthNeighbors = new List<int>();
    public List<int> SouthNeighbors = new List<int>();
    public List<int> WestNeighbors = new List<int>();
    public List<int> EastNeighbors = new List<int>();
}
