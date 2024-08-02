using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TEE.UI.Controllers {
    public class LoadingScreenController : MonoBehaviour {
        [SerializeField] Image        image;
        [SerializeField] List<Sprite> images;

        void Awake() {
            DontDestroyOnLoad(gameObject);
            ShowLoadingScreen();
        }

        void OnEnable() {
            ShowLoadingScreen();
        }

        void ShowLoadingScreen() {
            int rand = Random.Range(0, images.Count);
            image.sprite = images[rand];
        }
    }
}