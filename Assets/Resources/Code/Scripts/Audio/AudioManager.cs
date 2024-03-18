using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    [SerializeField] Sound[] sounds;

    public static AudioManager instance;

    void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start() {
        Sound s = Play("AmbienceWindStart");
        Play("AmbienceWindLoop", s.clip.length - 5);
    }

    //FindObjectOfType<AudioManager>().Play("name");
    public Sound Play(string name, float delaySeconds = 0) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return null;
        }
        if (delaySeconds == 0) {
            s.source.Play();
            AudioLog("Playing sound: " + name);
        } else {
            s.source.PlayDelayed(delaySeconds);
            AudioLog("Playing sound: " + name, delaySeconds);
        }
        return s;
    }

    public void AudioLog(string message, float delay = 0) {
        StartCoroutine(DelayedLog(message, delay));
    }

    IEnumerator DelayedLog(string message, float delay) {
        yield return new WaitForSeconds(delay);
        Debug.Log(message);
    }
}
