using UnityEngine;

public class BillboardRandomizer : MonoBehaviour {
    [Header("Billboard Setup")]
    public MeshRenderer backgroundPlane;  // Assign background plane
    public MeshRenderer logoPlane;        // Assign logo plane

    [Header("Logo Textures")]
    public Texture2D[] logoTextures;      // Assign logo textures in Inspector

    void Start() {
        ApplyRandomMaterials();
    }

    void ApplyRandomMaterials() {
        if (backgroundPlane != null) {
            // Create background material
            Material bgMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            SetupSpecularTransparent(bgMat, false); // no alpha clip needed here

            // Random background color
            bgMat.color = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.7f, 1f);
            backgroundPlane.material = bgMat;
        }

        if (logoPlane != null && logoTextures.Length > 0) {
            // Create logo material
            Material logoMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            SetupSpecularTransparent(logoMat, true); // enable alpha clipping for logos

            // Random texture for logo
            Texture2D chosenTex = logoTextures[Random.Range(0, logoTextures.Length)];
            logoMat.SetTexture("_BaseMap", chosenTex);
            logoMat.color = Color.white; // keep original texture colors
            logoPlane.material = logoMat;
        }
    }

    private void SetupSpecularTransparent(Material mat, bool useAlphaClip = false) {
        if (mat == null) return;

        mat.SetFloat("_WorkflowMode", 0); // 0=Specular

        if (useAlphaClip) {
            mat.SetFloat("_Surface", 0); // Opaque
            mat.SetFloat("_AlphaClip", 1);
            mat.EnableKeyword("_ALPHATEST_ON");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
        }
        else {
            mat.SetFloat("_Surface", 0); // Transparent
            mat.SetFloat("_AlphaClip", 1);
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.SetFloat("_ZWrite", 0);
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }
    }
}
