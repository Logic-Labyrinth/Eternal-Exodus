using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Exodus.ProceduralTools
{
    public class LevelManager : MonoBehaviour
    {
        private System.Random random;
        private bool isSeedInitialised = false;

        [TabGroup("Level Settings")]
        [DisableIf("useRandomSeed")]
        public string seedString;
        private int seed;

        [TabGroup("Level Settings")]
        public bool useRandomSeed;

        [TabGroup("Level Settings")]
        [PropertyTooltip("The number of tiles in the x direction.")]
        [Range(1, 50)]
        public int tileX = 10;

        [TabGroup("Level Settings")]
        [PropertyTooltip("The number of tiles in the y direction.")]
        [Range(1, 50)]
        public int tileY = 10;

        [TabGroup("Level Settings")]
        [PropertyTooltip(
            "The size of the small levelBlock.\nMedium and large tiles should have a scale of 2x1 and 2x2 respectively"
        )]
        public Vector2 baseTileSize;

        [TabGroup("Level Blocks")]
        [LabelWidth(200)]
        public int smallBlockWeight = 3;

        [TabGroup("Level Blocks")]
        [LabelWidth(200)]
        public int mediumBlockWeight = 2;

        [TabGroup("Level Blocks")]
        [LabelWidth(200)]
        public int largeBlockWeight = 1;

        [TabGroup("Level Blocks")]
        [PropertySpace]
        [TableList(ShowPaging = true)]
        public List<LevelBlockScriptableObject> levelBlocks;

        // Start is called before the first frame update
        void Start()
        {
            InitializeLevel();
            // for (int i = 0; i < 10; i++)
            // {
            //     Debug.Log("Test Seed: " + TestSeed());
            // }
        }

        // Update is called once per frame
        void Update() { }

        public void InitializeLevel()
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
            random = new System.Random(seed);
            isSeedInitialised = true;
        }

        // Previously used for testing the consistency of seeds, currently no longer required
        // int TestSeed()
        // {
        //     return Random.Range(0, 1000000);
        // }

        /// <summary>
        /// Generates a new level based on the given settings and level blocks.
        /// </summary>
        [TabGroup("Level Settings")]
        [PropertySpace]
        [Button(ButtonSizes.Large, ButtonAlignment = 1f)]
        public void GenerateLevel()
        {
            // If the seed is not initialized, initialize the level
            if (!isSeedInitialised)
                InitializeLevel();

            // Initialize the level grid based on the tile size
            var xSize = tileX;
            var ySize = tileY;
            int[,] levelGrid = new int[xSize, ySize];

            // Get the base tile size
            var baseTileX = baseTileSize.x;
            var baseTileY = baseTileSize.y;

            // Create the parent for the level blocks
            var levelParent = CreateLevelParent();

            // First pass: Place the selected blocks on the level grid
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    // Skip if the position is already taken
                    if (levelGrid[x, y] == 1)
                        continue;

                    // Select a random block and get its dimensions
                    LevelBlockScriptableObject selectedBlock = SelectRandomBlock(
                        levelBlocks,
                        random
                    );
                    Vector2Int blockDimension = GetBlockDimension(selectedBlock.blockType);
                    // Place the block if it can fit in the current position
                    if (CanPlaceBlock(x, y, blockDimension, levelGrid, xSize, ySize))
                    {
                        PlaceBlock(x, y, selectedBlock, random, levelParent);
                        // Mark the grid as occupied by the block
                        MarkGrid(x, y, blockDimension, ref levelGrid);
                    }
                }
            }

            // Second pass: Fill remaining gaps with small blocks
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    // Skip if the position is already taken
                    if (levelGrid[x, y] == 1)
                        continue;

                    // Get a small block and place it in the current position
                    LevelBlockScriptableObject smallBlock = GetSmallBlock(levelBlocks);
                    if (smallBlock != null)
                    {
                        PlaceBlock(x, y, smallBlock, random, levelParent);
                        // Mark the grid as occupied by the small block
                        MarkGrid(x, y, new Vector2Int(1, 1), ref levelGrid);
                    }
                }
            }
        }

        bool CanPlaceBlock(int x, int y, Vector2Int dimension, int[,] grid, int xSize, int ySize)
        {
            // Check if the block will fit within the grid
            if (x + dimension.x > xSize || y + dimension.y > ySize)
            {
                return false;
            }

            // Check each grid cell that the block would occupy
            for (int i = x; i < x + dimension.x; i++)
            {
                for (int j = y; j < y + dimension.y; j++)
                {
                    if (grid[i, j] == 1)
                    { // If any part of the grid is already occupied
                        return false;
                    }
                }
            }

            return true; // The block can be placed
        }

        /// <summary>
        /// Places a block at the specified position in the level.
        /// </summary>
        /// <param name="x">The x-coordinate of the position.</param>
        /// <param name="y">The y-coordinate of the position.</param>
        /// <param name="block">The block to be placed.</param>
        /// <param name="random">The random number generator.</param>
        /// <param name="levelParent">The parent GameObject of the level.</param>
        void PlaceBlock(
            int x,
            int y,
            LevelBlockScriptableObject block,
            System.Random random,
            GameObject levelParent
        )
        {
            // Convert grid coordinates to world position
            Vector3 worldPosition = GridToWorldPosition(x, y);

            // Adjust the position based on the actual size of the block
            Vector2Int blockDimension = GetBlockDimension(block.blockType);
            float offsetX = (blockDimension.x * baseTileSize.x) * 0.5f;
            float offsetZ = (blockDimension.y * baseTileSize.y) * 0.5f;
            Vector3 adjustedPosition = new Vector3(
                worldPosition.x + offsetX,
                0,
                worldPosition.z + offsetZ
            );

            // Instantiate the block prefab at the adjusted position
            GameObject blockInstance = Instantiate(
                block.blockPrefab,
                adjustedPosition,
                Quaternion.identity
            );
            // Set the name and parent of the block instance
            blockInstance.name = block.blockName + $" ({x}, {y})";
            blockInstance.transform.SetParent(levelParent.transform, true);
        }

        // GetSmallBlock takes a list of LevelBlockScriptableObject and returns a small block from the list
        LevelBlockScriptableObject GetSmallBlock(List<LevelBlockScriptableObject> blocks)
        {
            // Find and return a small block from the list
            return blocks.FirstOrDefault(block => block.blockType == BlockType.Small);
        }

        Vector3 GridToWorldPosition(int x, int y)
        {
            float worldX = x * baseTileSize.x + this.transform.position.x;
            float worldY = y * baseTileSize.y + this.transform.position.z;
            return new Vector3(worldX, 0, worldY);
        }

        void MarkGrid(int x, int y, Vector2Int dimension, ref int[,] grid)
        {
            for (int i = x; i < x + dimension.x; i++)
            {
                for (int j = y; j < y + dimension.y; j++)
                {
                    grid[i, j] = 1; // Mark the grid position as occupied
                }
            }
        }

        Vector2Int GetBlockDimension(BlockType type)
        {
            switch (type)
            {
                case BlockType.Small:
                    return new Vector2Int(1, 1);
                case BlockType.Medium:
                    return new Vector2Int(1, 2);
                case BlockType.Large:
                    return new Vector2Int(2, 2);
                default:
                    return Vector2Int.zero;
            }
        }

        LevelBlockScriptableObject SelectRandomBlock(
            List<LevelBlockScriptableObject> blocks,
            System.Random random
        )
        {
            if (blocks.Count == 0)
                return null;

            // Calculate total weight
            int totalWeight = 0;
            foreach (var block in blocks)
            {
                int weight = GetWeightForBlock(block);
                totalWeight += weight;
            }

            // Random selection based on weight
            int randomNumber = random.Next(totalWeight);
            int currentWeightSum = 0;
            foreach (var block in blocks)
            {
                currentWeightSum += GetWeightForBlock(block);
                if (randomNumber < currentWeightSum)
                {
                    return block;
                }
            }

            return null;
        }

        int GetWeightForBlock(LevelBlockScriptableObject block)
        {
            // Define the weight based on the block size
            // For example, smaller blocks could have a higher weight
            switch (block.blockType)
            {
                case BlockType.Small:
                    return smallBlockWeight; // Higher weight for small blocks
                case BlockType.Medium:
                    return mediumBlockWeight;
                case BlockType.Large:
                    return largeBlockWeight;
                default:
                    return 1;
            }
        }

        [TabGroup("Level Settings")]
        [PropertySpace]
        [Button(ButtonSizes.Large, ButtonAlignment = 1f)]
        public void ClearLevel()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
#if UNITY_EDITOR
                DestroyImmediate(transform.GetChild(i).gameObject);
#else
                Destroy(transform.GetChild(i).gameObject);
#endif
            }
        }

        [TabGroup("Level Settings")]
        [PropertySpace]
        [Button(ButtonSizes.Large, ButtonAlignment = 1f)]
        void ResetSeed()
        {
            isSeedInitialised = false;
        }

        GameObject CreateLevelParent()
        {
            // Instantiate an empty GameObject
            GameObject newLevel = new GameObject();

            newLevel.name = "Level";

            newLevel.transform.position = new Vector3(0, 0, 0);
            newLevel.transform.rotation = Quaternion.identity;
            newLevel.transform.SetParent(this.transform, true);

            return newLevel;
        }
    }
}
