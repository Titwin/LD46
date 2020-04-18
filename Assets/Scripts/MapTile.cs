using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MapTile Tile", menuName = "Tiles/Map Tile")]
public class MapTile : RuleTile
{
    public enum Type
    {
        Street, Walk, Building
    }
    public Type type;
}
