using UnityEngine;

[RequireComponent(typeof(Light))]
public class FireFlicker : MonoBehaviour {
    [SerializeField] float minIntensity = 0.5f;
    [SerializeField] float maxIntensity = 1.5f;
    [SerializeField] float flickerSpeed = 2f;

    Light torchLight;
    float baseIntensity;

    void Awake() {
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
