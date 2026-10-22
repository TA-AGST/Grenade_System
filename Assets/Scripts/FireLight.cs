using UnityEngine;

// Makes a light flicker like fire, and fades it out when the fire ends.
[RequireComponent(typeof(Light))]
public class FireLight : MonoBehaviour
{
    [SerializeField] private float baseIntensity = 3f;
    [SerializeField] private float flicker = 1.2f;
    [SerializeField] private float flickerSpeed = 8f;
    [SerializeField] private float lifetime = 8f;

    private Light fireLight;
    private float age;
    private float seed;

    private void Awake()
    {
        fireLight = GetComponent<Light>();
        seed = Random.value * 100f;
    }

    private void Update()
    {
        age += Time.deltaTime;

        float noise = Mathf.PerlinNoise(seed, Time.time * flickerSpeed);
        float fadeOut = Mathf.Clamp01((lifetime - age) / 1.5f);
        fireLight.intensity = (baseIntensity + (noise - 0.5f) * flicker * 2f) * fadeOut;
    }
}
