using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour {
    [SerializeField] Image image;
    [SerializeField] List<Sprite> images;

    void Awake() {
        int rand = Random.Range(0, images.Count);
        image.sprite = images[rand];
    }
}
