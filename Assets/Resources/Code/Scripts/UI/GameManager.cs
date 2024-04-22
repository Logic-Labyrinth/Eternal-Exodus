using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [SerializeField] LoadingScreenController loadingScreenController;

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

        FindObjectsOfType<HealthSystem>().ToList().ForEach(x => x.Kill());

        GameObject.Find("Portal").GetComponent<PortalVFX>().OpenPortal();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.G)) {
            Debug.Log("End Level");
            EndLevel();

        }
    }
}
