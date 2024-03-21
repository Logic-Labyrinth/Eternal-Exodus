using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour {
    [SerializeField] AudioMixer audioMixer;

    // Setting slider going from 0.0001 to 1

    public void SetMasterVolume(float volume) {
        audioMixer.SetFloat("masterVolume", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float volume) {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSoundVolume(float volume) {
        audioMixer.SetFloat("soundVolume", Mathf.Log10(volume) * 20);
    }
}
