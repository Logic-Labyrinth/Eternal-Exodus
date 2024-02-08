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
        [PropertyTooltip("The size of the small levelBlock.\nMedium and large tiles should have a scale of 2x1 and 2x2 respectively")]
        public Vector2 baseTileSize;
        [TabGroup("Level Blocks")]
        [Searchable]
        [TableList(ShowIndexLabels = true, ShowPaging = true)]
        public List<LevelBlockScriptableObject> levelBlocks;

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

        [TabGroup("Level Settings")]
        [PropertySpace]
        [Button(ButtonSizes.Large, Stretch = false, ButtonAlignment = 1f)]
        void GenerateLevel() {
            var xSize = tileX;
            var ySize = tileY;
            int[,] levelGrid = new int[xSize, ySize];

            var baseTileX = baseTileSize.x;
            var basTileY = baseTileSize.y;

            int smallBlockCount = 0;
            int mediumBlockCount = 0;
            int largeBlockCount = 0;

            foreach (var block in levelBlocks) {
                switch (block.blockType) {
                    case BlockType.Small:
                        smallBlockCount++;
                        break;
                    case BlockType.Medium:
                        mediumBlockCount++;
                        break;
                    case BlockType.Large:
                        largeBlockCount++;
                        break;
                }
            }

            
        }
    }
}
