using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public Transform player;
    public float fadeDistance;

    public List<GameObject> tiles;
    private Dictionary<int, GameObject> tilesInScene;

    void Start()
    {
        player = GameObject.Find("Astronaut").transform;
        StartingTileGen();
    }
    void Update()
    {

    }

    private void StartingTileGen()
    {
        Instantiate(tiles[0], new Vector3(0, 0, player.position.z - 9), Quaternion.identity);
        Instantiate(tiles[1], Vector3.zero, Quaternion.identity);
        int rnd = 0;
        int rotation = 0;
        for (int i = 0; i < tiles.Count; i++)
        {
            rnd = UnityEngine.Random.Range(1, 6);
            rotation = UnityEngine.Random.Range(0, 3);
            switch (rotation)
            {
                case 1:
                    rotation = 90;
                    break;
                case 2:
                    rotation = -90;
                    break;
                case 3:
                    rotation = 180;
                    break;
                default:
                    rotation = 0;
                    break;
            }
            if (i == 0)
            {
                Instantiate(tiles[rnd], new Vector3(0, 0, 9), new Quaternion(0, rotation, 0, 0));
            }
            if (i == 1)
            {
                Instantiate(tiles[rnd], new Vector3(9, 0, 9), new Quaternion(0, rotation, 0, 0));
            }
            if (i == 2)
            {
                Instantiate(tiles[rnd], new Vector3(9, 0, 0), new Quaternion(0, rotation, 0, 0));
            }
            if (i == 3)
            {
                Instantiate(tiles[rnd], new Vector3(9, 0, -9), new Quaternion(0, rotation, 0, 0));
            }
            if (i == 4)
            {
                Instantiate(tiles[rnd], new Vector3(-9, 0, -9), new Quaternion(0, rotation, 0, 0));
            }
            if (i == 5)
            {
                Instantiate(tiles[rnd], new Vector3(-9, 0, 0), new Quaternion(0, rotation, 0, 0));
            }
            if (i == 6)
            {
                Instantiate(tiles[rnd], new Vector3(-9, 0, 9), new Quaternion(0, rotation, 0, 0));
            }
        }
    }
}
