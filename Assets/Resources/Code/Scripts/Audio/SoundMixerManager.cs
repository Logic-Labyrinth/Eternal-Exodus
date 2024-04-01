using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour {
    [SerializeField] AudioMixer audioMixer;

    // Setting slider going from 0.0001 to 1

    public void SetMasterVolume(float volume) {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat("masterVolume", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float volume) {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSoundVolume(float volume) {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat("soundVolume", Mathf.Log10(volume) * 20);
    }
}
