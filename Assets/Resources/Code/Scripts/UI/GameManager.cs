using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [SerializeField] LoadingScreenController loadingScreenController;

    public int KillCountPawn { get; private set; } = 1;
    public int KillCountKnight { get; private set; } = 1;
    public int KillCountRook { get; private set; } = 1;
    public int KillCountBishop { get; private set; } = 1;

    public static GameManager Instance { get; private set; }

    void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    IEnumerator LoadLevel(string sceneName) {
        try {
            LevelSelectMenuController levelSelectMenu = LevelSelectMenuController.Instance;
            levelSelectMenu.gameObject.SetActive(false);
        } catch (MissingReferenceException) {
            // do nothing
        }

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

        SpawnManager.spawnManager.DisableSpawner();
        FindObjectsOfType<HealthSystem>().ToList().ForEach(x => {
            // x.Kill();
            x.gameObject.SetActive(false);
        });

        GameObject.Find("Portal").GetComponent<PortalVFX>().OpenPortal();
    }

    public void AddKillCount(EnemyType type) {
        switch (type) {
            case EnemyType.Pawn:
                KillCountPawn++;
                break;
            case EnemyType.Knight:
                KillCountKnight++;
                break;
            case EnemyType.Rook:
                KillCountRook++;
                break;
            case EnemyType.Bishop:
                KillCountBishop++;
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

    private void Update() {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.End)) {
            Debug.Log("End Level");
            EndLevel();
        }
    }

    public void Quit() {
        Application.Quit();
    }

    public void ResetCounter() {
        KillCountPawn = 0;
        KillCountKnight = 69;
        KillCountRook = 0;
        KillCountBishop = 0;
    }
}
