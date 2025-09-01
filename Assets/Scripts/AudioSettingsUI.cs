using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour {
    [Header("Assign your Toggles in Inspector")]
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle soundToggle;

    private const string MusicPrefKey = "MusicEnabled";
    private const string SoundPrefKey = "SoundEnabled";

    public void Initialize() {
        // Load saved values (default = ON)
        bool musicEnabled = PlayerPrefs.GetInt(MusicPrefKey, 1) == 1;
        bool soundEnabled = PlayerPrefs.GetInt(SoundPrefKey, 1) == 1;

        // Apply to toggles
        musicToggle.isOn = musicEnabled;
        soundToggle.isOn = soundEnabled;

        // Hook up listeners
        musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
        soundToggle.onValueChanged.AddListener(OnSoundToggleChanged);

        // Apply to your AudioManager immediately
        AudioManager.Instance.MuteMusic(musicEnabled);
        AudioManager.Instance.MuteSounds(soundEnabled);
    }

    private void OnDestroy() {
        // Cleanup listeners
        musicToggle.onValueChanged.RemoveListener(OnMusicToggleChanged);
        soundToggle.onValueChanged.RemoveListener(OnSoundToggleChanged);
    }

    private void OnMusicToggleChanged(bool value) {
        PlayerPrefs.SetInt(MusicPrefKey, value ? 1 : 0);
        PlayerPrefs.Save();

        AudioManager.Instance.MuteMusic(value);
    }

    private void OnSoundToggleChanged(bool value) {
        PlayerPrefs.SetInt(SoundPrefKey, value ? 1 : 0);
        PlayerPrefs.Save();

        AudioManager.Instance.MuteMusic(value);
    }
}