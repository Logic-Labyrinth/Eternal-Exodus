using UnityEngine;

namespace TEE.Procedural {
    [CreateAssetMenu(fileName = "New Level Block", menuName = "ExodusTools/Level Block")]
    public class LevelBlockScriptableObject : ScriptableObject {
        public string     blockName;
        public BlockType  blockType;
        public GameObject blockPrefab;
    }

    public enum BlockType {
        Small,
        Medium,
        Large
    }
}