using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    // Public variables
    public bool onGround;

    // Private variables
    private Ray groundCheckRay;
    private PlayerController player;

    [SerializeField] private float checkDistance;
    
    void Start()
    {
        // References
        player = GameObject.Find("Astronaut").GetComponent<PlayerController>();
        onGround = true; // Defaults to being on ground
    }

    void Update()
    {
        // Creates a new Ray just above the base of the player and checks if grounded
        groundCheckRay = new(player.transform.position + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(groundCheckRay, checkDistance)) {onGround = true;} else {onGround = false;}
    }
}