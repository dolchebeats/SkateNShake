using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public enum Swipe {
    None,
    Up,
    Down,
    Left,
    Right,
    UpLeft,
    UpRight,
    DownLeft,
    DownRight
}

public static class CardinalDirection {
    public static readonly Vector2 Up = Vector2.up;
    public static readonly Vector2 Down = Vector2.down;
    public static readonly Vector2 Right = Vector2.right;
    public static readonly Vector2 Left = Vector2.left;
    public static readonly Vector2 UpRight = new Vector2(1, 1);
    public static readonly Vector2 UpLeft = new Vector2(-1, 1);
    public static readonly Vector2 DownRight = new Vector2(1, -1);
    public static readonly Vector2 DownLeft = new Vector2(-1, -1);
}

public class SwipeManager : MonoBehaviour {
    #region Inspector

    [Header("Swipe Settings")]
    [Tooltip("Min swipe distance (inches) to register as swipe")]
    [SerializeField] private float minSwipeLength = 0.5f;

    [Tooltip("Trigger swipe instantly when threshold is reached")]
    [SerializeField] private bool triggerSwipeAtMinLength = false;

    [Tooltip("Use eight or four cardinal directions")]
    [SerializeField] private bool useEightDirections = false;

    #endregion

    #region Static State

    public static Vector2 SwipeVelocity { get; private set; }
    public static Swipe CurrentSwipe => swipeDirection;
    public static bool IsTapping => isTap;
    public static bool IsSwiping => swipeDirection != Swipe.None;

    public static event Action<Swipe, Vector2> OnSwipeDetected;

    private static float dpcm;
    private static float swipeStartTime;
    private static float swipeEndTime;
    private static bool swipeEnded;
    private static bool isTap;
    private static Swipe swipeDirection;

    private static Vector2 firstPressPos;
    private static Vector2 secondPressPos;
    private static SwipeManager instance;

    #endregion

    #region Constants

    private const float EightDirAngle = 0.906f;
    private const float FourDirAngle = 0.5f;
    private const float DefaultDPI = 72f;
    private const float DPCMFactor = 2.54f;

    private static readonly Dictionary<Swipe, Vector2> CardinalDirections = new()
    {
        { Swipe.Up, CardinalDirection.Up },
        { Swipe.Down, CardinalDirection.Down },
        { Swipe.Right, CardinalDirection.Right },
        { Swipe.Left, CardinalDirection.Left },
        { Swipe.UpRight, CardinalDirection.UpRight },
        { Swipe.UpLeft, CardinalDirection.UpLeft },
        { Swipe.DownRight, CardinalDirection.DownRight },
        { Swipe.DownLeft, CardinalDirection.DownLeft }
    };

    #endregion

    private void Awake() {
        instance = this;

        float dpi = Screen.dpi == 0 ? DefaultDPI : Screen.dpi;
        dpcm = dpi / DPCMFactor;
    }

    private void Update() {
        if (OnSwipeDetected != null) {
            
        }
        DetectSwipe();
    }

    #region Swipe Detection

    private static void DetectSwipe() {
        if (HandleTouchInput() || HandleMouseInput()) {
            if (swipeEnded) return;

            Vector2 swipeVector = secondPressPos - firstPressPos;
            float swipeLength = swipeVector.magnitude / dpcm;

            if (swipeLength < instance.minSwipeLength) {
                if (!instance.triggerSwipeAtMinLength) {
                    swipeDirection = Swipe.None;
                }
                return;
            }

            swipeEndTime = Time.time;
            SwipeVelocity = swipeVector * (swipeEndTime - swipeStartTime);
            swipeDirection = GetSwipeDirection(swipeVector.normalized);
            swipeEnded = true;

            OnSwipeDetected?.Invoke(swipeDirection, SwipeVelocity);
        }
        else {
            swipeDirection = Swipe.None;
            isTap = false;
        }
    }

    private static bool HandleTouchInput() {
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase) {
                case TouchPhase.Began:
                    firstPressPos = touch.position;
                    swipeStartTime = Time.time;
                    swipeEnded = false;
                    isTap = true;
                    break;

                case TouchPhase.Ended:
                    secondPressPos = touch.position;
                    return true;

                default:
                    return instance.triggerSwipeAtMinLength;
            }
        }
        else {
            isTap = false;
        }

        return false;
    }

    private static bool HandleMouseInput() {
        if (Input.GetMouseButtonDown(0)) {
            firstPressPos = Input.mousePosition;
            swipeStartTime = Time.time;
            swipeEnded = false;
            isTap = true;
        }
        else if (Input.GetMouseButtonUp(0)) {
            secondPressPos = Input.mousePosition;
            return true;
        }
        else {
            return instance.triggerSwipeAtMinLength;
        }

        return false;
    }

    private static Swipe GetSwipeDirection(Vector2 direction) {
        float threshold = instance.useEightDirections ? EightDirAngle : FourDirAngle;
        foreach (var pair in CardinalDirections) {
            if (Vector2.Dot(direction, pair.Value.normalized) > threshold)
                return pair.Key;
        }

        return Swipe.None;
    }

    #endregion

    /// <summary>
    /// Returns -1 if player is holding on left side of screen,
    /// +1 if on right side,
    /// and 0 if no valid hold (or swiping is active).
    /// </summary>
    public static float SteeringInput {
        get {
            if (IsSwiping) return 0f; // Don't steer while swiping

            foreach (Touch touch in Input.touches) {
                if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) {
                    Vector2 delta = touch.position - firstPressPos;

                    // Must be a minimal movement to count as a hold (not a swipe)
                    if (delta.magnitude < dpcm * 0.2f) {
                        if (touch.position.x < Screen.width / 2f)
                            return -1f; // Left
                        else
                            return +1f; // Right
                    }
                }
            }

            return 0f;
        }
    }

    #region Public Swipe Checks

    public static bool IsSwipingDirection(Swipe target) => CurrentSwipe == target;

    public static bool IsSwipingRight() => IsSwipingDirection(Swipe.Right);
    public static bool IsSwipingLeft() => IsSwipingDirection(Swipe.Left);
    public static bool IsSwipingUp() => IsSwipingDirection(Swipe.Up);
    public static bool IsSwipingDown() => IsSwipingDirection(Swipe.Down);
    public static bool IsSwipingUpLeft() => IsSwipingDirection(Swipe.UpLeft);
    public static bool IsSwipingUpRight() => IsSwipingDirection(Swipe.UpRight);
    public static bool IsSwipingDownLeft() => IsSwipingDirection(Swipe.DownLeft);
    public static bool IsSwipingDownRight() => IsSwipingDirection(Swipe.DownRight);

    #endregion
}