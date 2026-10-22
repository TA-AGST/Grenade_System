using UnityEngine;
using UnityEngine.UI;

// White full-screen image for the flash grenade. It stays bright for a moment, then fades out.
public class ScreenFlash : MonoBehaviour
{
    [SerializeField] private Image overlay;

    private float strength;
    private float holdTime;
    private float fadeSpeed;

    public static ScreenFlash Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        SetAlpha(0f);
    }

    public void Flash(float intensity, float duration)
    {
        strength = Mathf.Max(strength, intensity);
        holdTime = duration * 0.3f;
        fadeSpeed = strength / (duration * 0.7f);
    }

    private void Update()
    {
        if (strength <= 0f) return;

        if (holdTime > 0f) holdTime -= Time.deltaTime;
        else strength = Mathf.MoveTowards(strength, 0f, fadeSpeed * Time.deltaTime);

        SetAlpha(strength);
    }

    private void SetAlpha(float alpha)
    {
        Color c = overlay.color;
        c.a = alpha;
        overlay.color = c;
    }
}
