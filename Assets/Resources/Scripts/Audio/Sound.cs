using Sirenix.OdinInspector;
using UnityEngine;

namespace TEE.Audio {
    [System.Serializable]
    public class Sound {
        [LabelText("Sound Clip")] public       AudioClip audioClip;
        [ProgressBar(0f, 1f)]     public       float     volume       = 1;
        [ProgressBar(0f, 1f)]     public       float     spacialBlend = 1;
        public                                 bool      loop;
        [LabelText("Attach to Player")] public bool      player;
    }
}