using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public class GridTile
    {
        public Quaternion rotation;
        public GameObject instance;
    }

    // Public variables
    public Transform player;
    public float fadeDistance;
    public Dictionary<Vector2Int, GridTile> mapGrid = new Dictionary<Vector2Int, GridTile>();
    public GameObject[] tiles;
    public GameObject startTile;
    
    // Private variables
    private int gridScale = 9;
    private Vector2Int tileOrigin;
    private readonly Vector2Int[] directions =
        { Vector2Int.up, Vector2Int.up + Vector2Int.left, Vector2Int.up + Vector2Int.right, 
        Vector2Int.down, Vector2Int.down + Vector2Int.left, Vector2Int.down + Vector2Int.right,
        Vector2Int.left, Vector2Int.right };

    void Start()
    {
        // References
        player = GameObject.Find("Astronaut").transform;
        tileOrigin = new Vector2Int(Mathf.RoundToInt(player.position.x), Mathf.RoundToInt(player.position.z));
        StartingTileGen();
    }
    void Update()
    {
        TileGen(tileOrigin);
        Vector2Int currentPos = new(Mathf.RoundToInt(player.position.x / gridScale), Mathf.RoundToInt(player.position.z / gridScale));
        if (tileOrigin != currentPos)
        {
            tileOrigin = currentPos;
        }
    }

    private void StartingTileGen()
    {
        // Rocket Tile
        GameObject instance = Instantiate(startTile, new Vector3(0, 0, player.position.z - 9), Quaternion.identity);
        GridTile tile = new() { rotation = new Quaternion(0, 0, 0, 0), instance = instance };
        mapGrid.Add(new Vector2Int(0, -1), tile);
        // Base Tile
        instance = Instantiate(tiles[5], new Vector3(0, 0, player.position.z), Quaternion.identity);
        tile = new() { rotation = new Quaternion(0, 0, 0, 0), instance = instance };
        mapGrid.Add(new Vector2Int(0, 0), tile);
    }

    private void TileGen(Vector2Int centre)
    {
        foreach (Vector2Int dir in directions)
        {
            int rnd = Random.Range(0, tiles.Length);
            int rndRot = Random.Range(0, 4);
            Vector2Int checkPos = centre + dir;
            if (CheckIfEmpty(checkPos))
            {
                Quaternion rotation = Quaternion.Euler(0, rndRot * 90, 0);
                GameObject instance = Instantiate(tiles[rnd], new Vector3(checkPos.x * gridScale, 0f, checkPos.y * gridScale), rotation);
                GridTile tile = new() { rotation = rotation, instance = instance };
                mapGrid.Add(checkPos, tile);
            }
        }
        
    }
    private bool CheckIfEmpty(Vector2Int pos)
    {
        if (mapGrid.ContainsKey(pos)) { return false; }
        return true;

        // if (mapGrid.TryGetValue(pos, out GridTile tile)) { tile.instance = Instantiate}
    }
}
