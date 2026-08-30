using System;
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
        public GameObject prefab;
        public bool permanent;
    }

    // Public variables
    public Dictionary<Vector2Int, GridTile> mapGrid = new Dictionary<Vector2Int, GridTile>();
    public GameObject[] tiles;
    public GameObject startTile;
    public GameObject[] ores;
    public int generationRadius = 4; // Maximum radius from player

    // Private variables
    private Transform player;
    private WorldObjectManager worldObjectManager;
    private readonly int gridScale = 9;
    private Vector2Int tileOrigin;
    private Vector2Int lastDirection = Vector2Int.up;
    private bool oreSpawning = false;
    private Dictionary<GameObject, Queue<GameObject>> tilePools = new Dictionary<GameObject, Queue<GameObject>>();

    void Start()
    {
        // References
        player = GameObject.Find("Astronaut").transform;
        worldObjectManager = GameObject.Find("WorldObjectManager").GetComponent<WorldObjectManager>();

        tileOrigin = new Vector2Int(
            Mathf.RoundToInt(player.position.x / gridScale), 
            Mathf.RoundToInt(player.position.z / gridScale)
        ); // Ensures the starting world position matches the grid
        StartingTileGen();

        TileGen(tileOrigin, lastDirection); // Tile generation happens before the scene is loaded
    }
    void Update()
    {
        Vector2Int currentPos = new(
            Mathf.RoundToInt(player.position.x / gridScale), 
            Mathf.RoundToInt(player.position.z / gridScale)
        ); // Gets the player's world position and converts it to a grid position
        // Checks if the player has entered a new tile, stops constant calling of tileGen is not moving
        if (tileOrigin != currentPos)
        {
            Vector2Int movementDirection = currentPos - tileOrigin; // Calculates movement direction

            if (movementDirection != Vector2Int.zero)
            {
                lastDirection = movementDirection;
            }

            tileOrigin = currentPos;

            TileGen(tileOrigin, lastDirection);

            DespawnTiles();
            worldObjectManager.CheckDistantObjects();
        }
    }

    private void StartingTileGen()
    {
        // Rocket Tile
        GameObject instance = Instantiate(startTile, new Vector3(0, 0, player.position.z - 9), Quaternion.identity);
        GridTile tile = new() {
            rotation = new Quaternion(0, 0, 0, 0),
            instance = instance,
            permanent = true
        };
        mapGrid.Add(new Vector2Int(0, -1), tile);
        // Base Tile
        GameObject prefab = tiles[5];
        instance = Instantiate(prefab, new Vector3(0, 0, player.position.z), Quaternion.identity);
        tile = new() {
            rotation = new Quaternion(0, 0, 0, 0),
            instance = instance,
            prefab = prefab,
            permanent = true
        };
        mapGrid.Add(new Vector2Int(0, 0), tile);
    }

    // Determines where tiles should spawn
    private void TileGen(Vector2Int centre, Vector2Int direction)
    {
        for (int x = -generationRadius;  x <= generationRadius; x++)
        {
            for (int y = -generationRadius; y <= generationRadius; y++)
            {
                Vector2Int offset = new(x, y); // Represents position relative to the player

                if (offset.sqrMagnitude > (generationRadius * generationRadius)) // Gives the square magnitude instead of running a slow sqrt
                {
                    continue; // Removes the corners to have the semicircle shape
                }

                // Checks whether the tile is in front, behind or to the side of the player
                // If +ve then in front, -ve then behind and 0 then to the side
                int dotProduct = offset.x * direction.x + offset.y * direction.y;

                // Removes the tiles behind the player
                if (dotProduct < 0)
                {
                    continue;
                }

                Vector2Int checkPos = centre + offset;

                SpawnTileAt(checkPos);
            }
        }
    }

    // Attempts to spawn tile at calculated position
    private void SpawnTileAt(Vector2Int checkPos)
    {
        // If not empty return
        if (!CheckIfEmpty(checkPos)) { return; }

        // Random tile and rotation
        int rnd = UnityEngine.Random.Range(0, tiles.Length);
        int rndRot = UnityEngine.Random.Range(0, 4);
        Quaternion rotation = Quaternion.Euler(0, rndRot * 90, 0);

        // Spawn position from grid position
        Vector3 spawnPos = new(checkPos.x * gridScale, 0f, checkPos.y * gridScale);
        // Gets current prefab
        GameObject prefab = tiles[rnd];
        // Checks if tile is in pool
        GameObject instance = GetTileFromPool(prefab, spawnPos, rotation);

        // Tries to spawn ore if the random tile is blank (the last tile)
        if (rnd == (tiles.Length - 1) && !oreSpawning)
        {
            SpawnOre(spawnPos, rotation);
        }

        // New GridTile object with information about the tile
        GridTile tile = new()
        {
            rotation = rotation,
            instance = instance,
            prefab = prefab
        };
        mapGrid.Add(checkPos, tile);
    }

    private GameObject GetTileFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        // Checks if the tile that is to be instantiated is already in the pool
        if (tilePools.TryGetValue(prefab, out Queue<GameObject> pool))
        {
            // If it is, it is dequeued
            if (pool.Count > 0)
            {
                GameObject instance = pool.Dequeue();

                instance.transform.SetPositionAndRotation(position, rotation);

                instance.SetActive(true);

                return instance;
            }
        }
        // If it isn't then the tile is instantiated
        return Instantiate(prefab, position, rotation);
    }

    private void ReturnTileToPool(GridTile tile)
    {
        // When out of range, the tile is returned to the pool
        tile.instance.SetActive(false);

        // If the queue of that tile doesn't exist, the queue is created
        if (!tilePools.ContainsKey(tile.prefab))
        {
            tilePools[tile.prefab] = new Queue<GameObject>();
        }

        // Tile is queued
        tilePools[tile.prefab].Enqueue(tile.instance);
    }

    private void DespawnTiles()
    {
        List<Vector2Int> tilesToRemove = new();

        foreach (var pair in mapGrid)
        {
            if (pair.Value.permanent) { continue; }
            
            Vector2Int tilePos = pair.Key;

            int distanceX = tilePos.x - tileOrigin.x;
            int distanceY = tilePos.y - tileOrigin.y;
            int sqrDistance = distanceX * distanceX + distanceY * distanceY;
            int despawnRadius = generationRadius + 2;

            if (sqrDistance > despawnRadius * despawnRadius)
            {
                tilesToRemove.Add(tilePos);
            }
        }

        foreach (Vector2Int tilePos in tilesToRemove)
        {
            GridTile tile = mapGrid[tilePos];

            ReturnTileToPool(tile);

            mapGrid.Remove(tilePos);
        }
    }
    private void SpawnOre(Vector3 pos, Quaternion rot)
    {
        int spawnChance = 1;
        int rndOre = UnityEngine.Random.Range(0, ores.Length);
        int multiplier = 1;
        Vector3 tempPos = pos;

        oreSpawning = true;

        if (OreMultiplier("Copper", rndOre))
        {
            multiplier = 1;
        }
        else if (OreMultiplier("Iron", rndOre))
        {
            multiplier = 2;
        }

        if ((rndOre % 2) == 0)
        {
            spawnChance = 4;
        }
        else
        {
            spawnChance = 2;
        }
        spawnChance *= multiplier;
        int spawnRate = UnityEngine.Random.Range(0, spawnChance);
        if (spawnRate == 0)
        {
            Vector3 orePos = new(tempPos.x, 0f, tempPos.z);
            GameObject ore = Instantiate(ores[rndOre], (ores[rndOre].transform.position + orePos), rot);

            worldObjectManager.AddWorldObject(ore);
        }
        oreSpawning = false;
    }

    private bool OreMultiplier(string target, int index)
    {
         return ores[index].name.ToString().Contains(target);
    }
    // Checks if the grid position already has a tile
    private bool CheckIfEmpty(Vector2Int pos)
    {
        if (mapGrid.ContainsKey(pos)) { return false; }
        return true;
    }
}
