using System.Collections.Generic;
using UnityEngine;

public class WorldObjectManager : MonoBehaviour
{
    // Public variables
    public float despawnDistance = 30f;
    
    // Private variables
    private Transform player;
    private List<GameObject> worldObjects = new();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // References
        player = GameObject.Find("Astronaut").transform;
    }

    public void AddWorldObject(GameObject obj)
    {
        if (!worldObjects.Contains(obj))
        {
            worldObjects.Add(obj);
        }
    }
    public void RemoveWorldObject(GameObject obj)
    {
        worldObjects.Remove(obj);
    }
    public void CheckDistantObjects()
    {
        for (int i = worldObjects.Count -1; i>=0; i--)
        {
            GameObject obj = worldObjects[i];

            if (obj == null)
            {
                worldObjects.RemoveAt(i);
                continue;
            }

            float distance = Vector3.Distance(player.position, obj.transform.position);

            if (distance > despawnDistance)
            {
                Destroy(obj);
                worldObjects.RemoveAt(i);
            }
        }
    }
}
