using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Public variables
    
    // Private variables
    private Ray cameraObstructRay;
    private GameObject obstructChecker;
    private MeshRenderer[] obstruction;
    [SerializeField] private bool obstructed = false;
    [SerializeField] private float maxRayDistance;
    
    void Start()
    {
        // References
        obstructChecker = GameObject.Find("ObstructChecker");
    }

    void Update()
    {
        CheckIfObstructed();
        Debug.DrawRay(transform.position, obstructChecker.transform.forward * 12, Color.white);
    }

    private void CheckIfObstructed()
    {
        RaycastHit hit;
        cameraObstructRay = new(transform.position, obstructChecker.transform.forward);
        if (Physics.Raycast(cameraObstructRay, out hit, maxRayDistance))
        {
            obstructed = true;
            MeshRenderer[] renderer;
            renderer = hit.collider.gameObject.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < renderer.Length; i++)
            {
                renderer[i].enabled = false;
            }
            obstruction = renderer;
        }
        else { obstructed = false; }

        if (!obstructed && obstruction != null)
        {
            for (int i = 0; i < obstruction.Length; i++)
            {
                obstruction[i].enabled = true;
            }
        }
    }
}