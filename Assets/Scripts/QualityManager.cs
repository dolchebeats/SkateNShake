using UnityEngine;
using UnityEngine.UI; // Or TMP_Dropdown if using TextMeshPro
using TMPro;

public class QualityManager : MonoBehaviour {
    public TMP_Dropdown dropdown; // Or TMP_Dropdown if preferred

    void Start() {
        int saved = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        QualitySettings.SetQualityLevel(saved, true);
        // Clear existing options just in case
        dropdown.ClearOptions();

        // Add only the enabled quality levels (Low + High in your case)
        dropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));

        // Set dropdown to currently active quality
        dropdown.value = QualitySettings.GetQualityLevel();
        dropdown.RefreshShownValue();

        // Add listener for when player changes value
        dropdown.onValueChanged.AddListener(SetQuality);
    }

    void SetQuality(int index) {
        QualitySettings.SetQualityLevel(index, true);
        PlayerPrefs.SetInt("QualityLevel", index); // Save preference
    }


}
