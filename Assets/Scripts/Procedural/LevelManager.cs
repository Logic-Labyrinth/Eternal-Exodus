using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public string seedString;
    public int seed;
    public bool useRandomSeed;
    public int levelWidth;
    public int levelHeight;

    // Start is called before the first frame update
    void Start()
    {
        InitializeLevel();
        for (int i = 0; i < 10; i++)
        {
            Debug.Log("Test Seed: " + TestSeed());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitializeLevel()
    {
        if (useRandomSeed)
        {
            seed = Random.Range(0, 1000000);
            seedString = seed.ToString();
        } else
        {
            seed = seedString.GetHashCode();
        }
        Random.InitState(seed);

        Debug.Log("Seed: " + seedString);
    }

    int TestSeed() {
        return Random.Range(0, 1000000);
    }
}
