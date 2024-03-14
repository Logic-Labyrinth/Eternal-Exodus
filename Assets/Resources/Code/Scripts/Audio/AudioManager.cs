using UnityEngine;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour {
    public static AudioManager instance;
    public AudioSource audioSource;
    public AudioClip[] audioClips;

    private void Awake() {
        if (instance == null) instance = this;
    }

    public void PlayAudio(int index) {
        audioSource.clip = audioClips[index];
        audioSource.Play();
    }
}
