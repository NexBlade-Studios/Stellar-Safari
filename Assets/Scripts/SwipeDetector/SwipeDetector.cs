using System;
using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;

    [SerializeField]
    private float minDistanceForSwipe = 10f;

    public static event Action<SwipeData> OnSwipe = delegate { };
    public static event Action<Vector2> OnTap = delegate { };

    private void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUpPosition = touch.position;
                fingerDownPosition = touch.position;
            }

            else if (touch.phase == TouchPhase.Ended)
            {
                fingerDownPosition = touch.position;
                DetectSwipeOrTap();
            }
        }
    }

    private void DetectSwipeOrTap()
    {
        if (SwipeDistanceCheckMet())
        {
            if (IsHorizontalSwipe())
            {
                var direction = fingerDownPosition.x - fingerUpPosition.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
                SendSwipe(direction);
            }
            
            else if (IsVerticalSwipe())
            {
                var direction = fingerDownPosition.y - fingerUpPosition.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
                SendSwipe(direction);
            }
            fingerUpPosition = fingerDownPosition;
        }
        else
        {
            SendTap(fingerDownPosition);
        }
    }

    private bool IsVerticalSwipe()
    {
        return VerticalMovementDistance() * 1.5f > HorizontalMovementDistance();
    }

    private bool IsHorizontalSwipe()
    {
        return HorizontalMovementDistance() > VerticalMovementDistance() * 1.5f;
    }

    private bool SwipeDistanceCheckMet()
    {
        return HorizontalMovementDistance() > minDistanceForSwipe || VerticalMovementDistance() > minDistanceForSwipe;
    }

    private float VerticalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.y - fingerUpPosition.y);
    }

    private float HorizontalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.x - fingerUpPosition.x);
    }

    private void SendSwipe(SwipeDirection direction)
    {
        SwipeData swipeData = new SwipeData()
        {
            Direction = direction,
            StartPosition = fingerDownPosition,
            EndPosition = fingerUpPosition
        };
        OnSwipe(swipeData);
    }

    private void SendTap(Vector2 tapPosition)
    {
        OnTap(tapPosition);
    }
}

public struct SwipeData
{
    public Vector2 StartPosition;
    public Vector2 EndPosition;
    public SwipeDirection Direction;
}

public enum SwipeDirection
{
    Up,
    Down,
    Left,
    Right
}