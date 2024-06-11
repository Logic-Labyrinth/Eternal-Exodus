using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EnemyType {
    Pawn,
    Bishop,
    Rook
}

public class GameManager : MonoBehaviour {
    [SerializeField] LoadingScreenController loadingScreenController;

    public int KillCountPawn { get; private set; } = 0;
    public int KillCountRook { get; private set; } = 0;
    public int KillCountBishop { get; private set; } = 0;

    public static GameManager Instance { get; private set; }

    void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
        DontDestroyOnLoad(gameObject);
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
        FindObjectsOfType<HealthSystem>().ToList().ForEach(x => {
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
        FindObjectOfType<PlayerMovement>().DisableMovementInput();
        FindObjectOfType<PlayerCamera>().DisableCameraInput();
        FindObjectOfType<WeaponsController>().DisableWeaponsInput();
    }

    public void EnablePlayerInput() {
        FindObjectOfType<PlayerMovement>().EnableMovementInput();
        FindObjectOfType<PlayerCamera>().EnableCameraInput();
        FindObjectOfType<WeaponsController>().EnableWeaponsInput();
    }

    public void Quit() {
        Application.Quit();
    }

    public void ResetCounter() {
        KillCountPawn = 0;
        KillCountRook = 0;
        KillCountBishop = 0;
    }
}
