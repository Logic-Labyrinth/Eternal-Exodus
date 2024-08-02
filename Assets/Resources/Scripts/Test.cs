using UnityEngine;

public class Test : MonoBehaviour {
    void Start() {
        // set the desired aspect ratio (the values in this example are
        // hard-coded for 16:9, but you could make them into public
        // variables instead so you can set them at design time)
        const float targetAspect = 16.0f / 9.0f;

        // determine the game window's current aspect ratio
        float windowAspect = Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleHeight = windowAspect / targetAspect;

        // obtain camera component so we can modify its viewport
        Camera cam  = GetComponent<Camera>();
        Rect   rect = cam.rect;

        // if scaled height is less than current height, add letterbox
        if (scaleHeight < 1.0f) {
            rect.width  = 1.0f;
            rect.height = scaleHeight;
            rect.x      = 0;
            rect.y      = (1.0f - scaleHeight) / 2.0f;
        } else {
            // add pillarbox
            float scaleWidth = 1.0f / scaleHeight;

            rect.width  = scaleWidth;
            rect.height = 1.0f;
            rect.x      = (1.0f - scaleWidth) / 2.0f;
            rect.y      = 0;
        }

        cam.rect = rect;
    }
}