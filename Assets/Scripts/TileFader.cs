using Unity.VisualScripting;
using UnityEngine;

public class TileFader : MonoBehaviour
{
    // Variables
    public int fadeDistance;
    public Vector3 lastPos;
    private GameObject player;
    void Start()
    {
        // References
        player = GameObject.Find("Astronaut");
    }

    void Update()
    {

    }
}
