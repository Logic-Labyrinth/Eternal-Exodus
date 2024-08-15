using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LexUtils.Events;
using LexUtils.Singleton;
using TEE.Enemy;
using TEE.Health;
using TEE.Input;
using TEE.Player;
using TEE.UI.Controllers;
using TEE.VFX;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace TEE {
    public class GameManager : PersistentSingleton<GameManager> {
        [SerializeField] LoadingScreenController loadingScreenController;
        [SerializeField] VolumeProfile           volumeProfile;
        [SerializeField] float                   slowdownTime         = 3f;
        [SerializeField] float                   countdownTimeSeconds = 120f;

        const float SecondsPerPawn   = 0.5f;
        const float SecondsPerRook   = 3f;
        const float SecondsPerBishop = 2f;

        public static readonly Dictionary<EnemyType, int> KillCounts = new() {
            { EnemyType.Pawn, 0 },
            { EnemyType.Rook, 0 },
            { EnemyType.Bishop, 0 }
        };

        ColorAdjustments colorAdjustments;
        float            countdownTime = -1;

        protected override void Awake() {
            base.Awake();
            volumeProfile.TryGet(out colorAdjustments);
            colorAdjustments.saturation.value = 1;
        }

        void FixedUpdate() {
            HandleCountdown();
        }

        void HandleCountdown() {
            if (countdownTime == -1) return;
            countdownTime -= Time.fixedDeltaTime;

            UITimer.UpdateTornado(countdownTime, 1f - countdownTime / countdownTimeSeconds);
        }

        IEnumerator LoadLevel(string sceneName) {
            loadingScreenController.gameObject.SetActive(true);

            var sceneLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            while (sceneLoad?.isDone == false) yield return null;

            loadingScreenController.gameObject.SetActive(false);
            countdownTime = countdownTimeSeconds;
        }

        public void LoadScene(string sceneName) {
            StartCoroutine(LoadLevel(sceneName));
        }

        public void EndLevel() {
            GameObject explosionSource = GameObject.Find("Explosion Source");
            if (explosionSource) explosionSource.GetComponent<ExplosionVFX>().Play();

            SpawnManager.Instance.SetSpawnerActive(false);
            FindObjectsByType<HealthSystem>(FindObjectsSortMode.None).ToList().ForEach(x => { x.gameObject.SetActive(false); });
        }

        public void AddKillCount(EnemyType type) {
            KillCounts[type]++;
            float timeToAdd = type switch {
                EnemyType.Pawn   => SecondsPerPawn,
                EnemyType.Rook   => SecondsPerRook,
                EnemyType.Bishop => SecondsPerBishop,
                _                => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            countdownTime = Mathf.Min(countdownTime + timeToAdd, countdownTimeSeconds);
        }

        public static void Quit() {
            Application.Quit();
        }

        static void ResetCounter() {
            foreach (var killCount in KillCounts)
                KillCounts[killCount.Key] = 0;
        }

        public void Reset() {
            ResetCounter();
            colorAdjustments.saturation.value = 0;
        }

        public void Kill() {
            StartCoroutine(SlowDownGame());
        }

        IEnumerator SlowDownGame() {
            float timer = 0;

            while (timer < slowdownTime) {
                timer += Time.unscaledDeltaTime;
                float scale = Mathf.Clamp01(timer / slowdownTime);
                Time.timeScale                    = 1 - scale;
                colorAdjustments.saturation.value = scale * -100;

                yield return null;
            }

            Time.timeScale   = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;

            InputManager.Disable();
            FindAnyObjectByType<EndScreenController>(FindObjectsInactive.Include).gameObject.SetActive(true);
        }

        void OnDestroy() {
            colorAdjustments.saturation.value = 1;
            EventForge.UnregisterAllEvents();
        }
    }
}