using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Exodus.ProceduralTools
{
    public class LevelManager : MonoBehaviour
    {
        [TabGroup("Level Settings")]
        [DisableIf("useRandomSeed")]
        public string seedString;
        private int seed;
        [TabGroup("Level Settings")]
        public bool useRandomSeed;
        [TabGroup("Level Settings")]
        [PropertyTooltip("The number of tiles in the x direction.")]
        [Range(1, 20)]
        public int tileX = 10;
        [TabGroup("Level Settings")]
        [PropertyTooltip("The number of tiles in the y direction.")]
        [Range(1, 20)]
        public int tileY = 10;
        [TabGroup("Level Settings")]
        public Vector2 baseTileSize;
        [TabGroup("Block Prefabs")]
        public List<LevelBlockScriptableObject> blockPrefabs;

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
        void Update() { }

        void InitializeLevel()
        {
            if (useRandomSeed)
            {
                seed = Random.Range(0, 1000000);
                seedString = seed.ToString();
            }
            else
            {
                seed = seedString.GetHashCode();
            }
            Random.InitState(seed);

            Debug.Log("Seed: " + seedString);
        }

        int TestSeed()
        {
            return Random.Range(0, 1000000);
        }
    }
}
