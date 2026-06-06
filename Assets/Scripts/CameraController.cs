using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Private variables

    private Ray cameraObstructRay;
    private GameObject obstructChecker;
    private GameObject obstruction;
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
            hit.collider.gameObject.SetActive(false);
            obstructed = true;
            obstruction = hit.collider.gameObject;
        }
        else { obstructed = false; }

        if (!obstructed && obstruction != null)
        {
            obstruction.SetActive(true);
            obstruction = null;
        }
    }
}