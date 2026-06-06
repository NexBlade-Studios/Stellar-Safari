using UnityEngine;

public class SwipeLogger : MonoBehaviour
{
    public string swipeDir = null;
    private GroundChecker groundChecker;

    private void Start()
    {
        groundChecker = GameObject.Find("GroundChecker").GetComponent<GroundChecker>();
    }

    private void Awake()
    {
        SwipeDetector.OnSwipe += SwipeDetector_OnSwipe;
        SwipeDetector.OnTap += SwipeDetector_OnTap;
    }

    private void SwipeDetector_OnSwipe(SwipeData data)
    {
        Debug.Log("Swipe in Direction: " + data.Direction);
        swipeDir = data.Direction.ToString();
        Debug.Log("Swipe dir is" +  swipeDir);
    }

    private void SwipeDetector_OnTap(Vector2 tapPos)
    {
        Debug.Log("Tap detected at position: " + tapPos);
        if (tapPos.x > 0)
        {
            swipeDir = "Tap";
        }
    }
}