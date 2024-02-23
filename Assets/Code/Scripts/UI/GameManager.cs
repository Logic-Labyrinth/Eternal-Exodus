using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Exodus.ProceduralTools;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] LevelManager levelManager;
    [SerializeField] TMP_InputField seedInput;
    [SerializeField] GameObject player;
    public bool useRandomSeed;
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
    }

    public void SetUseRandomSeed(Toggle toggle) {
        useRandomSeed = toggle.isOn;
    }

    public void SetSeed() {
        seedText = seedInput.text;
    }
}
