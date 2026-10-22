using UnityEngine;

// Explodes and hurts everything in range that the blast can reach.
public class FragGrenade : Grenade
{
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private float radius = 8f;
    [SerializeField] private float maxDamage = 100f;
    [SerializeField] private float pushForce = 14f;

    protected override void OnExplode(Vector3 position)
    {
        if (effectPrefab != null) Instantiate(effectPrefab, position, Quaternion.identity);
        Explosion.Blast(position, radius, maxDamage, pushForce);
    }
}
