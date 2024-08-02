using System.Collections;
using System.Collections.Generic;
using LexUtils.Singleton;
using UnityEngine;

namespace TEE.Audio {
    public class SoundFXManager : Singleton<SoundFXManager> {
        [SerializeField] AudioSource    soundFXObject;

        readonly GameObject        mainCamera     = Camera.main.gameObject;
        readonly List<GameObject>  usedSources    = new();
        readonly Queue<GameObject> standbySources = new();

        // A method to play a sound at a given location, with optional parameters for the sound source location.
        public void Play(Sound sound, Transform location = null) {
            GameObject  source      = GetSource();
            AudioSource audioSource = source.GetComponent<AudioSource>();

            audioSource.clip         = sound.audioClip;
            audioSource.volume       = sound.volume;
            audioSource.spatialBlend = sound.spacialBlend;

            if (location) {
                source.transform.position = location.position;
            } else {
                source.transform.position = mainCamera.transform.position;
                if (sound.player) source.transform.SetParent(mainCamera.transform);
            }

            audioSource.Play();
            StartCoroutine(SetSourceToStandby(source, sound.audioClip.length));
        }

        /// <summary>
        /// Plays a random sound from the given array of sounds.
        /// </summary>
        /// <param name="sounds">An array of Sound objects to choose from.</param>
        /// <param name="location">An optional Transform to specify the location of the sound.</param>
        public void PlayRandom(Sound[] sounds, Transform location = null) {
            if (sounds.Length == 0) return;
            int randomIndex = Random.Range(0, sounds.Length);
            Play(sounds[randomIndex], location);
        }

        /// <summary>
        /// Retrieves a GameObject source from the standby queue and adds it to the used list.
        /// If the queue is empty, a new GameObject source is created and added to the used list.
        /// </summary>
        /// <returns>The retrieved GameObject source.</returns>
        GameObject GetSource() {
            GameObject source;
            if (standbySources.Count > 0) {
                source = standbySources.Dequeue();
                usedSources.Add(source);
                return source;
            }

            source = Instantiate(soundFXObject.gameObject);
            usedSources.Add(source);
            return source;
        }

        /// <summary>
        /// Sets the specified GameObject's AudioSource to standby after a specified delay.
        /// </summary>
        /// <param name="source">The GameObject with the AudioSource to set to standby.</param>
        /// <param name="delay">The delay, in seconds, before setting the AudioSource to standby. Default is 0.</param>
        /// <returns>An IEnumerator that waits for the specified delay before setting the AudioSource to standby.</returns>
        IEnumerator SetSourceToStandby(GameObject source, float delay = 0) {
            yield return new WaitForSeconds(delay);
            usedSources.Remove(source);
            standbySources.Enqueue(source);
            source.transform.SetParent(transform);
            AudioSource audioSource = source.GetComponent<AudioSource>();
            audioSource.Stop();
            audioSource.clip = null;
        }

        // Called when the sfx manager becomes disabled or inactive.
        private void OnDisable() {
            foreach (var source in usedSources)
                StartCoroutine(SetSourceToStandby(source));
        }

        // Function to handle the cleanup of used and standby game object sources.
        void OnDestroy() {
            foreach (var source in usedSources) Destroy(source);
            foreach (var source in standbySources) Destroy(source);
        }
    }
}