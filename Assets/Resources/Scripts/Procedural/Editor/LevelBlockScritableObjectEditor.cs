using UnityEditor;
using UnityEngine;

namespace TEE.Procedural {
    [CustomEditor(typeof(LevelBlockScriptableObject))]
    public class LevelBlockScriptableObjectEditor : Editor {
        public override void OnInspectorGUI() {
            LevelBlockScriptableObject scriptableObj = (LevelBlockScriptableObject)target;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Block Name");
            scriptableObj.blockName = GUILayout.TextField(scriptableObj.blockName);
            GUILayout.EndHorizontal();
            scriptableObj.blockType = (BlockType)EditorGUILayout.EnumPopup("Block Type", scriptableObj.blockType);

            // Create a 2x2 grid representation
            GUILayout.BeginVertical("box");
            for (int i = 0; i < 2; i++) {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < 2; j++) {
                    // Determine if the current grid square should be highlighted
                    bool highlight = scriptableObj.blockType switch {
                        BlockType.Small  => (i == 0 && j == 0),
                        BlockType.Medium => (i == 0),
                        BlockType.Large  => true,
                        _                => false
                    };

                    // Draw the grid square
                    GUIStyle style = new(GUI.skin.box) {
                        normal = {
                            background = MakeTex(2, 2, highlight ? Color.white : Color.gray)
                        }
                    };
                    GUILayout.Box("", style, GUILayout.Width(50), GUILayout.Height(50));
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();


            scriptableObj.blockPrefab = (GameObject)EditorGUILayout.ObjectField(
                "Block Prefab",
                scriptableObj.blockPrefab,
                typeof(GameObject),
                false
            );

            // Save changes
            if (GUI.changed) EditorUtility.SetDirty(target);
        }

        // Utility function to create a texture
        static Texture2D MakeTex(int width, int height, Color col) {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}