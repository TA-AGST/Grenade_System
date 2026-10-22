using UnityEngine;

// Leaves a cloud of smoke for a few seconds.
public class SmokeGrenade : Grenade
{
    [SerializeField] private GameObject effectPrefab;

    protected override void OnExplode(Vector3 position)
    {
        if (effectPrefab != null) Instantiate(effectPrefab, position + Vector3.up * 0.3f, Quaternion.identity);
    }
}
