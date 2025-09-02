using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrickManager : MonoBehaviour {
    public static TrickManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        foreach (var trick in trickDatabase) {
            trickLookup[trick.name] = trick;
        }
    }

    public TrickData[] trickDatabase; // assign tricks in Inspector
    private Dictionary<string, TrickData> trickLookup = new Dictionary<string, TrickData>();
    [SerializeField] private List<ComboEntry> comboDatabase = new();
    public string CurrentTrick { get; private set; } = "Jump";
    public string CurrentGrind { get; private set; } = "50-50";
    public string CurrentSpin { get; private set; }

    public int CurrentTrickScore { get; private set; }

    private TrickData lastSwipeTrick;
    private bool lastSwipeTrickWasFakie;
    private float lastSwipeTime;
    private Coroutine swipeFallbackRoutine;

    public bool isFakie = false;

    private readonly Dictionary<Swipe, (string trick, int requiredLevel)> trickTable =
        new()
        {
            { Swipe.Up, ("Ollie", 0) },
            { Swipe.Left, ("Pop Shuv", 3) },
            { Swipe.UpLeft, ("Kickflip", 4) },
            { Swipe.Right, ("Front Shuv", 3) },
            { Swipe.UpRight, ("Heelflip", 5) },
            { Swipe.Down, ("Nollie", 1) },
            { Swipe.DownLeft, ("Nollie Flip", 8) },
            { Swipe.DownRight, ("Nollie Heel", 8) }
        };

    // Grind table now includes weights
    // Format: (trickName, requiredLevel, weight)
    private readonly List<(string trick, int requiredLevel, int weight)> grindTable =
        new()
        {
            ("50-50", 0, 10),   // Default grind, common + fallback
            ("5-0", 2, 5),
            ("Nosegrind", 3, 5),
            ("Crooked", 4, 4),
            ("Salad", 5, 3),
            ("Suski", 5, 3),
            ("Feeble", 6, 2),
            ("Smith", 6, 2),
            ("Willy", 7, 1),
            ("OverWilly", 7, 1)
        };

    private void OnEnable() {
        SwipeManager.OnSwipeDetected += HandleSwipe;
    }

    private void OnDisable() {
        SwipeManager.OnSwipeDetected -= HandleSwipe;
    }

    // Called by SwipeManager
    private void HandleSwipe(Swipe swipe, Vector2 velocity) {
        if (!GameManager.Instance.isGameStarted||!GameManager.Instance.isAlive) return;
        if (PlayerMovement.Instance.isGrounded() || PlayerMovement.Instance.isGrinding || PlayerMovement.Instance.isKicking) {
            if (trickTable.TryGetValue(swipe, out var trickData)) {
                CurrentTrick = SaveManager.saveData.level >= trickData.requiredLevel
                    ? trickData.trick
                    : "Ollie";
            }
            else {
                CurrentTrick = "Ollie";
            }
            PlayerMovement.Instance.Jump();
            AnimationManager.Instance.ChooseJumpTrick(trickLookup[CurrentTrick].animatorTrigger);
            RegisterSwipeTrick(trickLookup[CurrentTrick], isFakie);
            AudioManager.Instance.PlaySound("Jump");
            if (PowerupManager.Instance.hasJumpBoost)
                AudioManager.Instance.PlaySound("Boing");

        }



    }

    public void TrySpin(float hInput) {
        if (SaveManager.saveData.level < 2) return;

        if (hInput > 0) {
            CurrentSpin = "Backside 180";
            AnimationManager.Instance.PlaySpin("back180");
        }
        else {
            CurrentSpin = "Frontside 180";
            AnimationManager.Instance.PlaySpin("front180");
        }
        RegisterSpinTrick(trickLookup[CurrentSpin]);


        isFakie = !isFakie;
    }

    public void ChooseGrind() {
        int playerLevel = SaveManager.saveData.level;
        // Filter only grinds the player can do
        var availableGrinds = grindTable
            .Where(g => playerLevel >= g.requiredLevel)
            .ToList();

        if (availableGrinds.Count == 0) {
            CurrentGrind = "50-50"; // fallback safety
        }
        else {
            // Weighted random selection
            int totalWeight = availableGrinds.Sum(g => g.weight);
            int roll = Random.Range(0, totalWeight);
            int cumulative = 0;

            foreach (var grind in availableGrinds) {
                cumulative += grind.weight;
                if (roll < cumulative) {
                    CurrentGrind = grind.trick;
                    break;
                }
            }
        }
        PerformTrick(trickLookup[CurrentGrind]);
        AnimationManager.Instance.PlayGrind(trickLookup[CurrentGrind].animatorTrigger);

    }

    

    public void PerformTrick(TrickData trick) {
        if (!GameManager.Instance.isAlive) return;
        GameManager.Instance.AddScore(trick.score);
        TrickSplashUI.Instance.ShowTrick(trick);
        SaveManager.saveData.tricksDone++;
    }

    public void RegisterSwipeTrick(TrickData swipe, bool wasFakie) {
        lastSwipeTrick = swipe;
        lastSwipeTime = Time.time;
        lastSwipeTrickWasFakie = wasFakie;
        // Cancel any old fallback
        if (swipeFallbackRoutine != null) {
            StopCoroutine(swipeFallbackRoutine);
        }

        // Start a new fallback
        swipeFallbackRoutine = StartCoroutine(SwipeFallbackRoutine(swipe,lastSwipeTime));
    }

    private IEnumerator SwipeFallbackRoutine(TrickData swipe, float swipeTime) {
        yield return new WaitForSeconds(0.4f);

        // If still waiting on a spin, perform just the swipe
        if (lastSwipeTrick == swipe && lastSwipeTime == swipeTime) {
            var combo = ResolveCombo(swipe, new TrickData { name = "N/A", score = 0 },lastSwipeTrickWasFakie);
            PerformTrick(combo);
            lastSwipeTrick = null;
        }
        swipeFallbackRoutine = null;
    }

    public void RegisterSpinTrick(TrickData spin) {
        bool comboResolved = false;

        // Cancel swipe fallback if running
        if (swipeFallbackRoutine != null) {
            StopCoroutine(swipeFallbackRoutine);
            swipeFallbackRoutine = null;
        }

        if (lastSwipeTrick != null && Time.time - lastSwipeTime <= 0.4f) {
            // Make a combo
            TrickData combo = ResolveCombo(lastSwipeTrick, spin, lastSwipeTrickWasFakie);
            PerformTrick(combo);
            comboResolved = true;
        }

        if (!comboResolved) {
            // Just the spin alone
            PerformTrick(spin);
        }

        // Clear old swipe once spin is resolved
        lastSwipeTrick = null;
    }


    private TrickData ResolveCombo(TrickData swipe, TrickData spin, bool wasFakie) {
        // 1. Database check
        foreach (var combo in comboDatabase) {
            if (combo.swipeTrick == swipe.name &&
                combo.spinTrick == spin.name &&
                combo.requiresFakie == wasFakie) {
                return new TrickData
                {
                    name = combo.resultTrick,
                    score = swipe.score + spin.score, // or custom combo multiplier
                };
            }
        }

        string fallbackName = "";
        if (wasFakie) fallbackName = "Fakie ";
        fallbackName += swipe.name;
        if (spin.name!="N/A") fallbackName += " " + spin.name;
        return new TrickData
        {
            name = fallbackName,
            score = swipe.score + spin.score
        };
    }
}
