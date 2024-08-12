using System.Collections;
using LexUtils.Singleton;
using UnityEngine;

namespace TEE.Audio {
    public class MusicManager : PersistentSingleton<MusicManager> {
        [SerializeField] AudioSource musicObject;
        [SerializeField] Music       startMusic;
        [SerializeField] Music       loopMusic;

        AudioSource musicSource;

        protected override void Awake() {
            base.Awake();
            musicSource                         = Instantiate(musicObject, gameObject.transform);
            musicSource.transform.localPosition = Vector3.zero;
        }

        void Start() {
            Play(startMusic);
            Play(loopMusic, startMusic.audioClip.length);
        }

        void Play(Music music, float delay = 0) {
            StartCoroutine(PlayDelayed(music, delay));
        }

        IEnumerator PlayDelayed(Music music, float delay) {
            yield return new WaitForSeconds(delay);
            musicSource.clip   = music.audioClip;
            musicSource.volume = music.volume;
            musicSource.loop   = music.loop;
            musicSource.Play();
        }

        void OnDestroy() {
            Destroy(musicSource.gameObject);
        }
    }
}