using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Block", menuName = "ExodusTools/Level Block")]
public class LevelBlockScriptableObject : ScriptableObject
{
    public BlockType blockType;
    public GameObject blockPrefab;
}

public enum BlockType
{
    Small,
    Medium,
    Large
}
