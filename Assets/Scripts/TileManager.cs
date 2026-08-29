using NUnit.Framework;
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
    }

    // Public variables
    public Transform player;
    public float fadeDistance;
    public Dictionary<Vector2Int, GridTile> mapGrid = new Dictionary<Vector2Int, GridTile>();
    public GameObject[] tiles;
    public GameObject startTile;
    public GameObject[] ores;
    public int generationRadius = 5; // Maximum radius from player
    
    // Private variables
    private readonly int gridScale = 9;
    private Vector2Int tileOrigin;
    private Vector2Int lastDirection = Vector2Int.up;
    private bool oreSpawning = false;

    void Start()
    {
        // References
        player = GameObject.Find("Astronaut").transform;
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
        // Stores current instance
        GameObject instance = Instantiate(tiles[rnd], spawnPos, rotation);

        // Tries to spawn ore if the random tile is blank (the last tile)
        if (rnd == (tiles.Length - 1) && !oreSpawning)
        {
            SpawnOre(spawnPos, rotation);
        }

        // New GridTile object with information about the tile
        GridTile tile = new()
        {
            rotation = rotation,
            instance = instance
        };
        mapGrid.Add(checkPos, tile);
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
            Instantiate(ores[rndOre], (ores[rndOre].transform.position + orePos), rot);
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
