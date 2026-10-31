using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Variables
    [SerializeField] private Vector3 playerPos;
    [SerializeField] private Vector3 offset;
    private GameObject player;

    void Start()
    {
        // References
        player = GameObject.Find("Astronaut");
    }
    void Update()
    {
        playerPos = player.transform.position; // References the player's position
        // Follows player position directly with offset
        transform.position = new Vector3(player.transform.position.x + offset.x, transform.position.y, player.transform.position.z +offset.z);
    }
}