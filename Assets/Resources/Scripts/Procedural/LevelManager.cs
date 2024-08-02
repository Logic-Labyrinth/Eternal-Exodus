using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace TEE.Procedural {
    public class LevelManager : MonoBehaviour {
        System.Random random;
        bool          isSeedInitialised;

        [TabGroup("Level Settings"), DisableIf("useRandomSeed")]
        public string seedString;

        int seed;

        [TabGroup("Level Settings")] public bool useRandomSeed;

        [TabGroup("Level Settings"), PropertyTooltip("The number of tiles in the x direction."), Range(1, 50)]
        public int tileX = 10;

        [TabGroup("Level Settings"), PropertyTooltip("The number of tiles in the y direction."), Range(1, 50)]
        public int tileY = 10;

        [TabGroup("Level Settings"), PropertyTooltip(
             "The size of the small levelBlock.\nMedium and large tiles should have a scale of 2x1 and 2x2 respectively"
         )]
        public Vector2 baseTileSize;

        [TabGroup("Level Blocks"), LabelWidth(200)]
        public int smallBlockWeight = 3;

        [TabGroup("Level Blocks"), LabelWidth(200)]
        public int mediumBlockWeight = 2;

        [TabGroup("Level Blocks"), LabelWidth(200)]
        public int largeBlockWeight = 1;

        [TabGroup("Level Blocks"), PropertySpace, TableList(ShowPaging = true)]
        public List<LevelBlockScriptableObject> levelBlocks;

        void Start() {
            InitializeLevel();
        }

        public void InitializeLevel() {
            if (useRandomSeed) {
                seed       = Random.Range(0, 1000000);
                seedString = seed.ToString();
            } else {
                seed = seedString.GetHashCode();
            }

            random            = new System.Random(seed);
            isSeedInitialised = true;
        }

        /// <summary>
        /// Generates a new level based on the given settings and level blocks.
        /// </summary>
        [TabGroup("Level Settings")]
        [PropertySpace]
        [Button(ButtonSizes.Large, ButtonAlignment = 1f)]
        public void GenerateLevel() {
            // If the seed is not initialized, initialize the level
            if (!isSeedInitialised) InitializeLevel();

            // Initialize the level grid based on the tile size
            var    xSize     = tileX;
            var    ySize     = tileY;
            int[,] levelGrid = new int[xSize, ySize];

            // Get the base tile size
            // var baseTileX = baseTileSize.x;
            // var baseTileY = baseTileSize.y;

            // Create the parent for the level blocks
            var levelParent = CreateLevelParent();

            // First pass: Place the selected blocks on the level grid
            for (int x = 0; x < xSize; x++)
            for (int y = 0; y < ySize; y++) {
                // Skip if the position is already taken
                if (levelGrid[x, y] == 1) continue;

                // Select a random block and get its dimensions
                LevelBlockScriptableObject selectedBlock = SelectRandomBlock(
                    levelBlocks,
                    random
                );
                Vector2Int blockDimension = GetBlockDimension(selectedBlock.blockType);

                // Place the block if it can fit in the current position
                if (!CanPlaceBlock(x, y, blockDimension, levelGrid, xSize, ySize)) continue;
                PlaceBlock(x, y, selectedBlock, levelParent);
                // Mark the grid as occupied by the block
                MarkGrid(x, y, blockDimension, ref levelGrid);
            }

            // Second pass: Fill remaining gaps with small blocks
            for (int x = 0; x < xSize; x++)
            for (int y = 0; y < ySize; y++) {
                // Skip if the position is already taken
                if (levelGrid[x, y] == 1) continue;

                // Get a small block and place it in the current position
                LevelBlockScriptableObject smallBlock = GetSmallBlock(levelBlocks);
                if (smallBlock == null) continue;
                PlaceBlock(x, y, smallBlock, levelParent);
                // Mark the grid as occupied by the small block
                MarkGrid(x, y, new Vector2Int(1, 1), ref levelGrid);
            }

            levelParent.GetComponent<NavMeshSurface>().RemoveData();
            // levelParent.GetComponent<NavMeshSurface>().BuildNavMesh();
        }

        bool CanPlaceBlock(int x, int y, Vector2Int dimension, int[,] grid, int xSize, int ySize) {
            // Check if the block will fit within the grid
            if (x + dimension.x > xSize || y + dimension.y > ySize) return false;

            // Check each grid cell that the block would occupy
            for (int i = x; i < x + dimension.x; i++)
            for (int j = y; j < y + dimension.y; j++)
                // If any part of the grid is already occupied
                if (grid[i, j] == 1)
                    return false;

            return true; // The block can be placed
        }

        /// <summary>
        /// Places a block at the specified position in the level.
        /// </summary>
        /// <param name="x">The x-coordinate of the position.</param>
        /// <param name="y">The y-coordinate of the position.</param>
        /// <param name="block">The block to be placed.</param>
        /// <param name="levelParent">The parent GameObject of the level.</param>
        void PlaceBlock(
            int                        x,
            int                        y,
            LevelBlockScriptableObject block,
            GameObject                 levelParent
        ) {
            // Convert grid coordinates to world position
            Vector3 worldPosition = GridToWorldPosition(x, y);

            // Adjust the position based on the actual size of the block
            Vector2Int blockDimension   = GetBlockDimension(block.blockType);
            float      offsetX          = (blockDimension.x * baseTileSize.x) * 0.5f;
            float      offsetZ          = (blockDimension.y * baseTileSize.y) * 0.5f;
            Vector3    adjustedPosition = new(worldPosition.x + offsetX, 0, worldPosition.z + offsetZ);

            // Instantiate the block prefab at the adjusted position
            GameObject blockInstance = Instantiate(block.blockPrefab, adjustedPosition, Quaternion.identity);
            // Set the name and parent of the block instance
            blockInstance.name = block.blockName + $" ({x}, {y})";
            blockInstance.transform.SetParent(levelParent.transform, true);
        }

        // GetSmallBlock takes a list of LevelBlockScriptableObject and returns a small block from the list
        static LevelBlockScriptableObject GetSmallBlock(List<LevelBlockScriptableObject> blocks) {
            // Find and return a small block from the list
            return blocks.FirstOrDefault(block => block.blockType == BlockType.Small);
        }

        Vector3 GridToWorldPosition(int x, int y) {
            float worldX = x * baseTileSize.x + transform.position.x;
            float worldY = y * baseTileSize.y + transform.position.z;
            return new Vector3(worldX, 0, worldY);
        }

        static void MarkGrid(int x, int y, Vector2Int dimension, ref int[,] grid) {
            for (int i = x; i < x + dimension.x; i++)
            for (int j = y; j < y + dimension.y; j++)
                grid[i, j] = 1; // Mark the grid position as occupied
        }

        static Vector2Int GetBlockDimension(BlockType type) {
            return type switch {
                BlockType.Small  => new Vector2Int(1, 1),
                BlockType.Medium => new Vector2Int(1, 2),
                BlockType.Large  => new Vector2Int(2, 2),
                _                => Vector2Int.zero
            };
        }

        LevelBlockScriptableObject SelectRandomBlock(List<LevelBlockScriptableObject> blocks, System.Random rand) {
            if (blocks.Count == 0)
                return null;

            // Calculate total weight
            int totalWeight = blocks.Sum(GetWeightForBlock);

            // Random selection based on weight
            int randomNumber     = rand.Next(totalWeight);
            int currentWeightSum = 0;
            foreach (var block in blocks) {
                currentWeightSum += GetWeightForBlock(block);
                if (randomNumber < currentWeightSum) {
                    return block;
                }
            }

            return null;
        }

        int GetWeightForBlock(LevelBlockScriptableObject block) {
            // Define the weight based on the block size
            // For example, smaller blocks could have a higher weight
            return block.blockType switch {
                BlockType.Small  => smallBlockWeight, // Higher weight for small blocks
                BlockType.Medium => mediumBlockWeight,
                BlockType.Large  => largeBlockWeight,
                _                => 1,
            };
        }

        [TabGroup("Level Settings")]
        [PropertySpace]
        [Button(ButtonSizes.Large, ButtonAlignment = 1f)]
        public void ClearLevel() {
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

        GameObject CreateLevelParent() {
            // Instantiate an empty GameObject
            GameObject newLevel = new("Level");

            newLevel.transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
            newLevel.transform.SetParent(transform, true);
            var navmesh = newLevel.AddComponent<NavMeshSurface>();
            navmesh.useGeometry = NavMeshCollectGeometry.PhysicsColliders;

            return newLevel;
        }

        [TabGroup("Level Settings")]
        [PropertySpace]
        [Button(ButtonSizes.Large, ButtonAlignment = 1f)]
        void RegenerateLevel() {
            ClearLevel();
            ResetSeed();
            GenerateLevel();
        }
    }
}