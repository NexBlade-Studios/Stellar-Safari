using Unity.VisualScripting;
using UnityEngine;

public class TileFader : MonoBehaviour
{
    public int fadeDistance;
    public Vector3 lastPos;
    private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Astronaut");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
