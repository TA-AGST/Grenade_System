using UnityEngine;

// Starts a fire. The fire prefab has a FireZone that burns things standing in it.
public class FireGrenade : Grenade
{
    [SerializeField] private GameObject effectPrefab;

    protected override void OnExplode(Vector3 position)
    {
        if (effectPrefab != null) Instantiate(effectPrefab, position, Quaternion.identity);
    }
}
