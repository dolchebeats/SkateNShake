using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TrickSplashUI : MonoBehaviour {
    public static TrickSplashUI Instance { get; private set; }
    private void Awake() {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    [Header("UI Prefabs")]
    [SerializeField] private GameObject splashPrefab; // has TMP_Text + Animator

    [Header("Settings")]
    [SerializeField] private int maxActiveSplashes = 3;
    [SerializeField] private float splashDuration = 1.5f;
    [SerializeField] private float verticalSpacing = 30f;

    // Use a list so we can insert at top and re-layout
    private readonly List<RectTransform> activeSplashes = new();
    private void OnDisable() {
        foreach (var rt in activeSplashes) {
            if (rt != null) Destroy(rt.gameObject);
        }
        activeSplashes.Clear();
    }
    public void ShowTrick(TrickData trick) {
        if (splashPrefab == null) return;
        if (!gameObject.activeInHierarchy) return;
        var go = Instantiate(splashPrefab, transform);
        var rt = go.GetComponent<RectTransform>();
        var text = go.GetComponentInChildren<TMP_Text>();
        if (text != null) text.text = $"{trick.name}  +{trick.score}";

        // Insert new splash at top visually and in our model
        activeSplashes.Insert(0, rt);
        rt.SetAsFirstSibling();

        ReLayout();

        // Overflow → remove the bottom-most
        if (activeSplashes.Count > maxActiveSplashes) {
            var toRemove = activeSplashes[activeSplashes.Count - 1];
            activeSplashes.RemoveAt(activeSplashes.Count - 1);
            if (toRemove != null) Destroy(toRemove.gameObject);
            ReLayout();
        }
        
        StartCoroutine(RemoveSplashAfterTime(go, splashDuration));
    }

    private void ReLayout() {
        // Top row is i=0 at y=0, then step downward
        for (int i = 0; i < activeSplashes.Count; i++) {
            var rt = activeSplashes[i];
            if (rt == null) continue;
            rt.anchoredPosition = new Vector2(0f, -i * verticalSpacing);
        }
    }

    private IEnumerator RemoveSplashAfterTime(GameObject splash, float lifetime) {
        if (splash == null) yield break;

        Animator animator = splash.GetComponent<Animator>();

        // Wait before fading out
        yield return new WaitForSeconds(lifetime);

        if (splash == null) yield break;

        if (animator != null) {
            animator.SetTrigger("FadeOut");
        }

        // Wait until fade animation finishes (assume ~1s, or match your clip length)
        float fadeDuration = 1f; // default fallback
        if (animator != null && animator.runtimeAnimatorController != null) {
            var clip = animator.runtimeAnimatorController.animationClips
                .FirstOrDefault(c => c.name == "FadeOut");
            if (clip != null) fadeDuration = clip.length;
        }
        yield return new WaitForSeconds(fadeDuration);

        if (splash != null) {
            activeSplashes.Remove(splash.GetComponent<RectTransform>());
            Destroy(splash);
            ReLayout();
        }
    }
}