using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Exodus.ProceduralTools
{
    public class LevelManager : MonoBehaviour {
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
        [PropertyTooltip("The size of the small levelBlock.\nMedium and large tiles should have a scale of 2x1 and 2x2 respectively")]
        public Vector2 baseTileSize;
        [TabGroup("Level Blocks")]
        [TableList(ShowPaging = true)]
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
            random = new System.Random(seed);
            isSeedInitialised = true;
        }

        int TestSeed()
        {
            return Random.Range(0, 1000000);
        }

        [TabGroup("Level Settings")]
        [PropertySpace]
        [Button(ButtonSizes.Large, ButtonAlignment = 1f)]
        void GenerateLevel() {
            if (!isSeedInitialised) InitializeLevel();

            var xSize = tileX;
            var ySize = tileY;
            int[,] levelGrid = new int[xSize, ySize];

            var baseTileX = baseTileSize.x;
            var basTileY = baseTileSize.y;

            // First pass
            for (int x = 0; x < xSize; x++) {
                for (int y = 0; y < ySize; y++) {
                    if (levelGrid[x, y] == 1) continue; // Skip if the position is already taken

                    LevelBlockScriptableObject selectedBlock = SelectRandomBlock(levelBlocks, random);
                    Vector2Int blockDimension = GetBlockDimension(selectedBlock.blockType);
                    if (CanPlaceBlock(x, y, blockDimension, levelGrid, xSize, ySize)) {
                        PlaceBlock(x, y, selectedBlock, random);
                        MarkGrid(x, y, blockDimension, ref levelGrid); // Mark the grid as occupied
                    }
                }
            }

            // Second pass: Fill remaining gaps with small blocks
            for (int x = 0; x < xSize; x++) {
                for (int y = 0; y < ySize; y++) {
                    if (levelGrid[x, y] == 1) continue; // Skip if the position is already taken

                    LevelBlockScriptableObject smallBlock = GetSmallBlock(levelBlocks);
                    if (smallBlock != null) {
                        PlaceBlock(x, y, smallBlock, random);
                        MarkGrid(x, y, new Vector2Int(1, 1), ref levelGrid);
                    }
                }
            }
        }

        bool CanPlaceBlock(int x, int y, Vector2Int dimension, int[,] grid, int xSize, int ySize) {
            // Check if the block will fit within the grid
            if (x + dimension.x > xSize || y + dimension.y > ySize) {
                return false;
            }

            // Check each grid cell that the block would occupy
            for (int i = x; i < x + dimension.x; i++) {
                for (int j = y; j < y + dimension.y; j++) {
                    if (grid[i, j] == 1) { // If any part of the grid is already occupied
                        return false;
                    }
                }
            }

            return true; // The block can be placed
        }

        void PlaceBlock(int x, int y, LevelBlockScriptableObject block, System.Random random) {
            Vector3 worldPosition = GridToWorldPosition(x, y);

            // Adjust the position based on the actual size of the block
            Vector2Int blockDimension = GetBlockDimension(block.blockType);
            float offsetX = (blockDimension.x * baseTileSize.x) * 0.5f;
            float offsetZ = (blockDimension.y * baseTileSize.y) * 0.5f;
            Vector3 adjustedPosition = new Vector3(worldPosition.x + offsetX, 0, worldPosition.z + offsetZ);

            GameObject blockInstance = Instantiate(block.blockPrefab, adjustedPosition, Quaternion.identity);
            blockInstance.name = block.blockName + $" ({x}, {y})";
            blockInstance.transform.SetParent(this.transform, true);
        }

        LevelBlockScriptableObject GetSmallBlock(List<LevelBlockScriptableObject> blocks) {
            // Find and return a small block from the list
            return blocks.FirstOrDefault(block => block.blockType == BlockType.Small);
        }

        Vector3 GridToWorldPosition(int x, int y) {
            // Convert grid coordinates to world space coordinates
            // This depends on how your grid and world space are set up.
            // For example:
            float worldX = x * baseTileSize.x;
            float worldY = y * baseTileSize.y;
            return new Vector3(worldX, 0, worldY);
        }

        void MarkGrid(int x, int y, Vector2Int dimension, ref int[,] grid) {
            for (int i = x; i < x + dimension.x; i++) {
                for (int j = y; j < y + dimension.y; j++) {
                    grid[i, j] = 1; // Mark the grid position as occupied
                }
            }
        }

        Vector2Int GetBlockDimension(BlockType type) {
            switch(type) {
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

        LevelBlockScriptableObject SelectRandomBlock(List<LevelBlockScriptableObject> blocks, System.Random random) {
            // Randomly select and return a block from the list
            if(blocks.Count == 0) return null;
            return blocks[random.Next(blocks.Count)];
        }

        [TabGroup("Level Settings")]
        [PropertySpace]
        [Button(ButtonSizes.Large, ButtonAlignment = 1f)]
        void ClearLevel() {
            for (int i = transform.childCount - 1; i >= 0; i--) {
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
        void ResetSeed() {
            isSeedInitialised = false;
        }
    }
}
