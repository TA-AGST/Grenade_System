using UnityEngine;

// Blinds the player: stronger when close, when looking at the flash and when nothing is in between.
public class FlashGrenade : Grenade
{
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private float radius = 25f;
    [SerializeField] private float maxDuration = 4f;

    protected override void OnExplode(Vector3 position)
    {
        if (effectPrefab != null) Instantiate(effectPrefab, position, Quaternion.identity);

        Camera cam = Camera.main;
        if (cam == null || ScreenFlash.Instance == null) return;

        Vector3 source = position + Vector3.up * 0.2f;
        Vector3 toFlash = source - cam.transform.position;
        float distance = toFlash.magnitude;
        if (distance > radius) return;

        float facing = Vector3.Dot(cam.transform.forward, toFlash / distance);        // -1 (behind) .. 1 (looking at it)
        float lookFactor = Mathf.Lerp(0.2f, 1f, Mathf.InverseLerp(-0.2f, 0.8f, facing));
        float visible = ExplosionVisibility.PointFactor(source, cam.transform.position);
        float strength = (1f - distance / radius) * lookFactor * visible;

        if (strength > 0.02f) ScreenFlash.Instance.Flash(Mathf.Clamp01(strength * 1.5f), Mathf.Lerp(1f, maxDuration, strength));
    }
}
