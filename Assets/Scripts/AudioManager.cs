using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public Sound[] sounds;
    public float tapeSpeed = .25f;
    bool tapestop;
    void Start()
    {
        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            int i = Random.Range(0, s.clip.Length);
            s.source.clip = s.clip[i];
            s.source.loop = s.loop;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;


        }

        PlaySound("MainTheme");
        GetComponent<AudioSettingsUI>().Initialize();
    }

    public void MuteMusic(bool mute) {
        sounds[0].source.mute = !mute;
    }

    public void MuteSounds(bool mute) {
        for (int i = 1; i < sounds.Length; i++) {
            sounds[i].source.mute = !mute;
        }
    }

    private void Update() {
        if (sounds[0].source.pitch > 0 && tapestop) {
            sounds[0].source.pitch -= Time.deltaTime * 10;
            if (sounds[0].source.pitch < 0.1f)
                sounds[0].source.pitch = 0;
        }


    }

    public void PlayerDeath() {
        ChangePitch(1f);
        StopSound("Skate");
        TapeStop("MainTheme");
        PlaySound("GameOver");
    }


    public void PlaySound(string name)
    {
        foreach (Sound s in sounds) {
            if(s.name == name) {
                s.source.Play();
            }
        }
    }

    public void StopSound(string name) {
        foreach (Sound s in sounds) {
            if (s.name == name) {
                s.source.Stop();
            }
        }
    }
    public void TapeStop(string name) {
        tapestop=true;
    }

    public void ChangePitch(float speed) {
        foreach (Sound s in sounds) {
            s.source.pitch = speed;
        }
    }

}
