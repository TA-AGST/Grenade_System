using UnityEngine;

// Burns everything standing near the fire. Walls still protect: it uses the same exposure check as the frag.
public class FireZone : MonoBehaviour
{
    [SerializeField] private float radius = 3f;
    [SerializeField] private float damagePerSecond = 12f;
    [SerializeField] private float duration = 8f;
    [SerializeField] private float tickTime = 0.5f;

    private float age;
    private float nextTick;

    private void Update()
    {
        age += Time.deltaTime;
        if (age >= duration)
        {
            enabled = false;
            return;
        }
        if (age < nextTick) return;
        nextTick = age + tickTime;

        Vector3 source = transform.position + Vector3.up * 0.3f;
        foreach (Health h in Explosion.FindLiving(source, radius))
        {
            float exposure = ExplosionVisibility.Exposure(source, h);
            h.TakeDamage(damagePerSecond * tickTime * exposure);
        }
    }
}
