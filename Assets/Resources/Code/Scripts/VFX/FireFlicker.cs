using UnityEngine;

public class FireFlicker : MonoBehaviour {
    public float minIntensity = 0.5f; // Minimum intensity of the flicker
    public float maxIntensity = 1.5f; // Maximum intensity of the flicker
    public float flickerSpeed = 2f; // Speed of the flicker

    private Light torchLight;
    private float baseIntensity;

    void Start() {
        torchLight = GetComponent<Light>();
        baseIntensity = torchLight.intensity;
    }

    void Update() {
        // Calculate flicker intensity based on Perlin noise
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0);
        float flickerIntensity = Mathf.Lerp(minIntensity, maxIntensity, noise);

        // Apply flicker intensity to the light
        torchLight.intensity = baseIntensity * flickerIntensity;
    }
}
