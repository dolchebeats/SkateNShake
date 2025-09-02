using UnityEngine;

public class SunRotation : MonoBehaviour {
    public float rotationPeriod = 240f; // 4 minutes
    private float currentRotationX;

    private void Start() {
        // Pick a random starting X rotation
        currentRotationX = 90;

        // Ensure Y matches this X right from the start
        UpdateRotation();
    }

    private void Update() {
        // Degrees per second
        float degreesPerSecond = 360f / rotationPeriod;

        // Advance X rotation
        currentRotationX += degreesPerSecond * Time.deltaTime;
        if (currentRotationX >= 360f) currentRotationX -= 360f;

        UpdateRotation();
    }

    private void UpdateRotation() {
        float x = currentRotationX;

        // Y oscillates between 45 and 135 depending on X
        float y = 90f + 45f * Mathf.Sin(x * Mathf.Deg2Rad * 2f) *-1f;

        // Apply rotation
        transform.rotation = Quaternion.Euler(x, y, 0f);
    }
}