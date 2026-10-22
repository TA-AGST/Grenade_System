using UnityEngine;

// One asset per grenade type (Frag, Flash, Smoke, Fire) so the numbers live in data, not in code.
[CreateAssetMenu(menuName = "Grenade/Grenade Data")]
public class GrenadeData : ScriptableObject
{
    public string displayName = "Frag";
    public Color color = Color.white;
    public float fuseTime = 4f;
    public int startAmmo = 5;
    public Grenade prefab;
}
