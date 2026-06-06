using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Variables
    [SerializeField] private Vector3 playerPos;
    [SerializeField] private Vector3 offset;
    private GameObject player;

    [SerializeField] private bool lerp;
    void Start()
    {
        // References
        player = GameObject.Find("Astronaut");
    }
    void Update()
    {
        playerPos = player.transform.position; // References the player's position
        // Either lerps to the player position or just follows it directly
        if (lerp)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(playerPos.x, 0, playerPos.z) + offset, 0.01f);
        }
        else
        {
            transform.position = new Vector3(player.transform.position.x + offset.x, transform.position.y, player.transform.position.z +offset.z);}
    }
}