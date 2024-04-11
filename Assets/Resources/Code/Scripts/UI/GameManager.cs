using System.Collections;
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
        LevelSelectMenuController levelSelectMenu = LevelSelectMenuController.Instance;
        levelSelectMenu.gameObject.SetActive(false);
        loadingScreenController.gameObject.SetActive(true);

        var sceneLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!sceneLoad.isDone) yield return null;
    }

    public void LoadScene(string sceneName){
        StartCoroutine(LoadLevel(sceneName));
    }
}
