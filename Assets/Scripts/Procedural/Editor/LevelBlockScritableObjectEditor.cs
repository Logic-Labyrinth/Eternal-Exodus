using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelBlockScriptableObject))]
public class LevelBlockScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // DrawDefaultInspector();

        LevelBlockScriptableObject scriptableObj = (LevelBlockScriptableObject)target;

        scriptableObj.blockType = (BlockType)
            EditorGUILayout.EnumPopup("Block Type", scriptableObj.blockType);

        // Create a 2x2 grid representation
        GUILayout.BeginVertical("box");
        for (int i = 0; i < 2; i++)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < 2; j++)
            {
                // Determine if the current grid square should be highlighted
                bool highlight = false;
                switch (scriptableObj.blockType)
                {
                    case BlockType.Small:
                        highlight = (i == 0 && j == 0);
                        break;
                    case BlockType.Medium:
                        highlight = (i == 0);
                        break;
                    case BlockType.Large:
                        highlight = true;
                        break;
                }

                // Draw the grid square
                GUIStyle style = new GUIStyle(GUI.skin.box);
                style.normal.background = MakeTex(2, 2, highlight ? Color.white : Color.gray);
                GUILayout.Box("", style, GUILayout.Width(50), GUILayout.Height(50));
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();


        scriptableObj.blockPrefab = (GameObject)
            EditorGUILayout.ObjectField(
                "Block Prefab",
                scriptableObj.blockPrefab,
                typeof(GameObject),
                false
            );

        // Save changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

    // Utility function to create a texture
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
