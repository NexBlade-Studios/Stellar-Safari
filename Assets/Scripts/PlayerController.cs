using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public variables
    public Rigidbody rb;

    // Private variables

    private Ray moveCheckRay;
    private SwipeLogger swipeLogger;
    private GroundChecker groundChecker;

    private bool isMoving;
    [SerializeField] private bool isRotating;
    [SerializeField] private float moveForce;
    [SerializeField] private float jumpForce;
    [SerializeField] private float rotateDuration;

    void Start()
    {
        // References
        rb = gameObject.GetComponent<Rigidbody>();
        swipeLogger = GameObject.Find("SwipeLogger").GetComponent<SwipeLogger>();
        groundChecker = GameObject.Find("GroundChecker").GetComponent<GroundChecker>();
    }

    void Update()
    {
        // Checks if player is not moving or rotating and grounded
        if (!isMoving && !isRotating && groundChecker.onGround)
        {
            // Checks for player movement - mobile and PC
            Movement("Up", KeyCode.W, Vector3.forward, 0);
            Movement("Down", KeyCode.S, Vector3.back, 180);
            Movement("Left", KeyCode.A, Vector3.left, 270);
            Movement("Right", KeyCode.D, Vector3.right, 90);
            Movement("Tap", KeyCode.None, Vector3.forward, 0);

            // Rounds player position to ensure grid movement
            transform.localPosition = RoundTransform(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
            swipeLogger.swipeDir = null; // If player is not moving, swipe direction is reset
            isMoving = false;
        }
        if (!groundChecker.onGround)
        {
            swipeLogger.swipeDir = null; // If player is not grounded, swipe direction is reset
        }
    }

    private void Movement(string dir, KeyCode key, Vector3 moveCheckDir, int angle)
    {
        // Checks for input and player is grounded
        if ((swipeLogger.swipeDir == dir || Input.GetKeyDown(key)) && groundChecker.onGround)
        {
            swipeLogger.swipeDir = "Null"; // Resets swipe direction
            isMoving = true;
            if (!isRotating)
            {
                StartCoroutine(RotatePlayer(Quaternion.Euler(0, angle, 0), rotateDuration)); // Rotates player according to Quaternion
            }
            // Checks if player can move and if player has rotated
            if (CanMove(moveCheckDir) && transform.eulerAngles.y == angle)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Moves player
                rb.AddForce(moveCheckDir * moveForce, ForceMode.Impulse);
            }
        }
    }

    IEnumerator RotatePlayer(Quaternion target, float duration)
    {
        isRotating = true;
        // Rotates player to target rotation - independent of FPS!
        while (transform.localRotation != target)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, target, 10);
            yield return new WaitForSeconds(duration);
        }
        transform.localRotation = target; // Ensures rotation is exact
        isRotating = false;
    }
    private Vector3 RoundTransform(float x, float y, float z)
    {
        Vector3 newTransform = new(Mathf.Round(x * 2) / 2, y, Mathf.Round(z * 2) / 2); // Rounds x and z transform
        return newTransform;
    }
    // Raycast to check if there is an obstacle in front of the player
    private bool CanMove(Vector3 moveDir)
    {
        moveCheckRay = new(groundChecker.transform.position, moveDir);
        if (Physics.Raycast(moveCheckRay, 3f)) {return false;} else {return true;}
    }
}