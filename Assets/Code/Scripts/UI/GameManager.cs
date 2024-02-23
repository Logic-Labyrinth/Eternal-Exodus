using System.Collections;
using System.Collections.Generic;
using Exodus.ProceduralTools;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    [SerializeField]
    LevelManager levelManager;

    [SerializeField]
    TMP_InputField seedInput;

    [SerializeField]
    GameObject player;

    [SerializeField]
    string gameSceneName;
    public bool useRandomSeed = true;
    public string seedText;

    // Start is called before the first frame update
    void Start() {
        DontDestroyOnLoad(this.gameObject);

        if (levelManager) {
            DontDestroyOnLoad(levelManager.gameObject);
        }
    }

    void Update() {
        seedInput.interactable = !useRandomSeed;
    }

    public void StartGame() {
        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel() {
        levelManager.useRandomSeed = useRandomSeed;
        levelManager.seedString = seedText;

        levelManager.InitializeLevel();

        var sceneLoad = SceneManager.LoadSceneAsync(gameSceneName, LoadSceneMode.Single);

        while (!sceneLoad.isDone) {
            yield return null;
        }

        levelManager.ClearLevel();
        levelManager.GenerateLevel();

        // Instantiate(player, levelManager.transform.GetChild(0).transform.position, Quaternion.identity);
    }

    private void LoadScene(Scene scene) { }

    public void SetUseRandomSeed(Toggle toggle) {
        useRandomSeed = !toggle.isOn;
    }

    public void SetSeed(TMP_InputField inputField) {
        seedText = inputField.text;
    }
}
