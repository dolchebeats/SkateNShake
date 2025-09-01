using UnityEngine;

public class DebugOverlay : MonoBehaviour {
    [Header("Overlay Settings")]
    public int fontSize = 14;
    public Color textColor = Color.white;
    public Vector2 offset = new Vector2(10, 10);

    private float deltaTime = 0.0f;
    private Camera mainCam;

    void Start() {
        mainCam = Camera.main;
    }

    void Update() {
        // Exponential moving average for smoother FPS readout
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI() {
        GUIStyle style = new GUIStyle();
        Rect rect = new Rect(offset.x, offset.y, Screen.width, Screen.height);

        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = fontSize;
        style.normal.textColor = textColor;

        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        float frameTime = Time.deltaTime * 1000.0f;

        // Runtime-safe triangle/vertex calculation
        long triCount = 0;
        long vertCount = 0;
        int rendererCount = 0;

        if (mainCam != null) {
            var renderers = FindObjectsOfType<Renderer>();
            foreach (var r in renderers) {
                if (!r.isVisible) continue; // Only count visible objects
                rendererCount++;

                Mesh mesh = null;

                if (r is MeshRenderer && r.TryGetComponent(out MeshFilter mf)) {
                    mesh = mf.sharedMesh;
                }
                else if (r is SkinnedMeshRenderer smr) {
                    mesh = smr.sharedMesh;
                }

                if (mesh != null) {
                    vertCount += mesh.vertexCount; // safe, vertexCount always available

                    // SAFE triangle count without needing Read/Write
                    for (int i = 0; i < mesh.subMeshCount; i++) {
                        triCount += mesh.GetIndexCount(i) / 3;
                    }
                }
            }
        }

        string text = string.Format(
            "FPS: {0:0.} ({1:0.0} ms)\nFrame Time: {2:0.0} ms\nVSync: {3}\nResolution: {4}x{5}\nApprox Batches: {6}\nTriangles: {7}\nVertices: {8}",
            fps, msec, frameTime,
            QualitySettings.vSyncCount > 0 ? "On" : "Off",
            Screen.width, Screen.height,
            rendererCount, triCount, vertCount
        );

        GUI.Label(rect, text, style);
    }
}
