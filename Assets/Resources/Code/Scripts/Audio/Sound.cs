using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class Sound {
    [LabelText("Sound Clip")] public AudioClip audioClip;
    [ProgressBar(0f, 1f)] public float volume = 1;
    [ProgressBar(0f, 1f)] public float spacialBlend = 1;
    public bool loop = false;
    [LabelText("Attach to Player")] public bool player = false;
}
