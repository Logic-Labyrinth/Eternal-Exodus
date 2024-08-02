using Sirenix.OdinInspector;
using UnityEngine;

namespace TEE.Audio {
    [System.Serializable]
    public class Music {
        [LabelText("Music Clip")] public AudioClip audioClip;
        [ProgressBar(0f, 1f)]     public float     volume = 1;
        public                           bool      loop;
    }
}