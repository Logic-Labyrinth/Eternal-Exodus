using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Exodus.ProceduralTools;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] LevelManager levelManager;
    [SerializeField] TMP_InputField seedInput;
    [SerializeField] GameObject player;
    [SerializeField] string gameSceneName;
    public bool useRandomSeed = true;
    public string seedText;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        if (levelManager) {
            DontDestroyOnLoad(levelManager.gameObject);
        }
    }

    void Update() {
        seedInput.interactable = !useRandomSeed;
    }

    public void StartGame() {
            levelManager.useRandomSeed = useRandomSeed;
            levelManager.seedString = seedText;

            levelManager.InitializeLevel();

            SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);

            levelManager.ClearLevel();
            levelManager.GenerateLevel();

            Instantiate(player, levelManager.transform.GetChild(0).transform.position, Quaternion.identity);
    }

    private void LoadScene(Scene scene) {

    }

    public void SetUseRandomSeed(Toggle toggle) {
        useRandomSeed = !toggle.isOn;
    }

    public void SetSeed(TMP_InputField inputField) {
        seedText = inputField.text;
    }
}
