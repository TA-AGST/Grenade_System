using UnityEngine;

// A light that starts bright and fades out quickly (explosion flash).
[RequireComponent(typeof(Light))]
public class LightFade : MonoBehaviour
{
    [SerializeField] private float fadeTime = 0.4f;

    private Light flashLight;
    private float startIntensity;
    private float age;

    private void Awake()
    {
        flashLight = GetComponent<Light>();
        startIntensity = flashLight.intensity;
    }

    private void Update()
    {
        age += Time.deltaTime;
        flashLight.intensity = Mathf.Lerp(startIntensity, 0f, age / fadeTime);
        if (age >= fadeTime) enabled = false;
    }
}
