using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour {
    public static MusicManager Instance { get; private set; }
    [SerializeField] AudioSource musicObject;
    [SerializeField] Music startMusic;
    [SerializeField] Music loopMusic;

    GameObject musicSource;

    void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        DontDestroyOnLoad(this);

        musicSource = Instantiate(musicObject).gameObject;
        musicSource.transform.SetParent(gameObject.transform);
        musicSource.transform.localPosition = Vector3.zero;
    }

    private void Start() {
        Play(startMusic);
        Play(loopMusic, startMusic.audioClip.length);
    }

    public void Play(Music music, float delay = 0) {
        StartCoroutine(PlayDelayed(music, delay));
    }

    IEnumerator PlayDelayed(Music music, float delay) {
        yield return new WaitForSeconds(delay);
        AudioSource audioSource = musicSource.GetComponent<AudioSource>();
        audioSource.clip = music.audioClip;
        audioSource.volume = music.volume;
        audioSource.loop = music.loop;
        audioSource.Play();
    }

    void OnDestroy() {
        Destroy(musicSource);
    }
}
