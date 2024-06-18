using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public enum EnemyType {
    Pawn,
    Bishop,
    Rook
}

public class GameManager : MonoBehaviour {
    [SerializeField] LoadingScreenController loadingScreenController;
    [SerializeField] VolumeProfile volumeProfile;
    [SerializeField] float slowdownTime = 3f;

    public int KillCountPawn { get; private set; } = 0;
    public int KillCountRook { get; private set; } = 0;
    public int KillCountBishop { get; private set; } = 0;

    public static GameManager Instance { get; private set; }

    ColorAdjustments colorAdjustments;

    void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
        DontDestroyOnLoad(gameObject);
        volumeProfile.TryGet(out colorAdjustments);
        colorAdjustments.saturation.value = 0;
    }

    IEnumerator LoadLevel(string sceneName) {
        loadingScreenController.gameObject.SetActive(true);

        var sceneLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!sceneLoad.isDone) yield return null;

        loadingScreenController.gameObject.SetActive(false);
    }

    public void LoadScene(string sceneName) {
        StartCoroutine(LoadLevel(sceneName));
    }

    public void EndLevel() {
        GameObject explosionSource = GameObject.Find("Explosion Source");
        explosionSource.GetComponent<ExplosionVFX>().Play();

        SpawnManager.Instance.SetSpawnerActive(false);
        FindObjectsByType<HealthSystem>(FindObjectsSortMode.None).ToList().ForEach(x => {
            x.gameObject.SetActive(false);
        });
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
        }
    }

    public void DisablePlayerInput() {
        FindAnyObjectByType<PlayerMovement>().DisableMovementInput();
        FindAnyObjectByType<PlayerCamera>().DisableCameraInput();
        FindAnyObjectByType<WeaponsController>().DisableWeaponsInput();
    }

    public void EnablePlayerInput() {
        FindAnyObjectByType<PlayerMovement>().EnableMovementInput();
        FindAnyObjectByType<PlayerCamera>().EnableCameraInput();
        FindAnyObjectByType<WeaponsController>().EnableWeaponsInput();
    }

    public void Quit() {
        Application.Quit();
    }

    void ResetCounter() {
        KillCountPawn = 0;
        KillCountRook = 0;
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
            Time.timeScale = 1 - scale;
            colorAdjustments.saturation.value = scale * -100;

            yield return null;
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        DisablePlayerInput();
        FindAnyObjectByType<EndScreenController>(FindObjectsInactive.Include).gameObject.SetActive(true);
        yield break;
    }
}
