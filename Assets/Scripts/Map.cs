using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public class Tile
    {
        Vector2Int position;
        
    }
    Dictionary<Vector2Int, Tile> map;
}
