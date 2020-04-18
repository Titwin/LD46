using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;
public class Map : MonoBehaviour
{
    public Tilemap tilemap;
    public static Map main;
    static public Dictionary<Vector2Int, MapTile> map = new Dictionary<Vector2Int, MapTile>();

    public static Vector3Int[] Directions =
    {
        new Vector3Int(-1,-1,0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(1, -1, 0)
    };

    private void Awake()
    {
        main = this;
        Index();
    }

    void Index()
    {
        var bounds = tilemap.cellBounds;
        for (int x = bounds.min.x; x <= bounds.max.x; ++x)
        {
            for (int y = bounds.min.y; y <= bounds.max.y; ++y)
            {
                var cell = new Vector2Int(x, y);
                var tile = GetTile(cell);
                if (tile)
                {
                    map.Add(cell, tile);
                }
            }
        }
    }

    public static Vector2 GetWorldPosition(Vector2Int cell)
    {
        return main.tilemap.GetCellCenterWorld((Vector3Int)cell);
    }
    public static MapTile GetTile(Vector3Int cell)
    {
        return (MapTile)main.tilemap.GetTile((Vector3Int)cell);
    }
    static MapTile GetTile(Vector2Int cell)
    {
        return GetTile((Vector3Int)cell);
    }
    public static MapTile GetTile(Vector2 position)
    {
        return main.tilemap.GetTile<MapTile>(main.tilemap.layoutGrid.WorldToCell(position));
    }
    public static MapTile[] GetTileNeighbor(Vector2 position)
    {
        MapTile[] result = new MapTile[8];
        result[0] = main.tilemap.GetTile<MapTile>(main.tilemap.layoutGrid.WorldToCell(position) + Directions[0]);
        result[1] = main.tilemap.GetTile<MapTile>(main.tilemap.layoutGrid.WorldToCell(position) + Directions[1]);
        result[2] = main.tilemap.GetTile<MapTile>(main.tilemap.layoutGrid.WorldToCell(position) + Directions[2]);
        result[3] = main.tilemap.GetTile<MapTile>(main.tilemap.layoutGrid.WorldToCell(position) + Directions[3]);
        result[4] = main.tilemap.GetTile<MapTile>(main.tilemap.layoutGrid.WorldToCell(position) + Directions[4]);
        result[5] = main.tilemap.GetTile<MapTile>(main.tilemap.layoutGrid.WorldToCell(position) + Directions[5]);
        result[6] = main.tilemap.GetTile<MapTile>(main.tilemap.layoutGrid.WorldToCell(position) + Directions[6]);
        result[7] = main.tilemap.GetTile<MapTile>(main.tilemap.layoutGrid.WorldToCell(position) + Directions[7]);
        return result;
    }
    private void OnDrawGizmos()
    {
        if (!Player.main)
        {
            return;
        }
        BoundsInt bounds = new BoundsInt((int)(Player.main.position.x - 10), (int)(Player.main.position.y - 10), 0, 20, 20, 0);
        for (int x = bounds.min.x; x <= bounds.max.x; ++x)
        {
            for (int y = bounds.min.y; y <= bounds.max.y; ++y)
            {
                var cell = new Vector2Int(x, y);
                if (map.ContainsKey(cell))
                {
                    var tile = map[cell];
                    {
                        switch (tile.type)
                        {
                            case MapTile.Type.Walk:
                                Gizmos.color = Color.white;
                                break;
                            case MapTile.Type.Building:
                                Gizmos.color = Color.red;
                                break;
                            case MapTile.Type.Street:
                                Gizmos.color = Color.green;
                                break;
                        }
                        Gizmos.DrawWireCube(GetWorldPosition(cell), new Vector3(1, 1, 0));
                    }

                    for (int dx = -1; dx <= 1; ++dx)
                    {
                        for (int dy = -1; dy <= 1; ++dy)
                        {
                            if (dx == 0 && dy == 0)
                            {
                                continue;
                            }
                            var cell2 = new Vector2Int(cell.x + dx, cell.y + dy);
                            if (map.ContainsKey(cell2))
                            {
                                var tile2 = map[cell2];
                                {
                                    if (tile.type == tile2.type)
                                    {
                                        Gizmos.DrawLine(GetWorldPosition(cell), GetWorldPosition(cell2));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
