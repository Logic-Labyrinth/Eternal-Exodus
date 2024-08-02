using System;
using System.Collections;
using System.Linq;
using LexUtils.Events;
using LexUtils.Singleton;
using TEE.Enemies;
using TEE.Health;
using TEE.Player;
using TEE.Player.Camera;
using TEE.Player.Movement;
using TEE.Player.Weapons;
using TEE.UI.Controllers;
using TEE.VFX;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace TEE {
    public enum EnemyType {
        Pawn,
        Bishop,
        Rook
    }

    public class GameManager : PersistentSingleton<GameManager> {
        [SerializeField] LoadingScreenController loadingScreenController;
        [SerializeField] VolumeProfile           volumeProfile;
        [SerializeField] float                   slowdownTime = 3f;

        public int KillCountPawn   { get; private set; }
        public int KillCountRook   { get; private set; }
        public int KillCountBishop { get; private set; }

        ColorAdjustments colorAdjustments;

        protected override void Awake() {
            base.Awake();
            volumeProfile.TryGet(out colorAdjustments);
            colorAdjustments.saturation.value = 0;
        }

        IEnumerator LoadLevel(string sceneName) {
            loadingScreenController.gameObject.SetActive(true);

            var sceneLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            while (sceneLoad is { isDone: false }) yield return null;

            loadingScreenController.gameObject.SetActive(false);
        }

        public void LoadScene(string sceneName) {
            StartCoroutine(LoadLevel(sceneName));
        }

        public void EndLevel() {
            GameObject explosionSource = GameObject.Find("Explosion Source");
            explosionSource.GetComponent<ExplosionVFX>().Play();

            SpawnManager.Instance.SetSpawnerActive(false);
            FindObjectsByType<HealthSystem>(FindObjectsSortMode.None).ToList().ForEach(x => { x.gameObject.SetActive(false); });
        }

        public void AddKillCount(EnemyType type) {
            switch (type) {
                case EnemyType.Pawn:
                    KillCountPawn++;
                    UITimer.Instance.AddPawnTime();
                    break;
                case EnemyType.Rook:
                    KillCountRook++;
                    UITimer.Instance.AddRookTime();
                    break;
                case EnemyType.Bishop:
                    KillCountBishop++;
                    UITimer.Instance.AddBishopTime();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static void DisablePlayerInput() {
            FindAnyObjectByType<PlayerMovement>().DisableMovementInput();
            FindAnyObjectByType<PlayerCamera>().DisableCameraInput();
            FindAnyObjectByType<WeaponsController>().DisableWeaponsInput();
        }

        public static void EnablePlayerInput() {
            FindAnyObjectByType<PlayerMovement>().EnableMovementInput();
            FindAnyObjectByType<PlayerCamera>().EnableCameraInput();
            FindAnyObjectByType<WeaponsController>().EnableWeaponsInput();
        }

        public void Quit() {
            Application.Quit();
        }

        void ResetCounter() {
            KillCountPawn   = 0;
            KillCountRook   = 0;
            KillCountBishop = 0;
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

            DisablePlayerInput();
            FindAnyObjectByType<EndScreenController>(FindObjectsInactive.Include).gameObject.SetActive(true);
        }

        void OnDestroy() {
            colorAdjustments.saturation.value = 1;
            EventForge.UnregisterAllEvents();
        }
    }
}