using System.Collections.Generic;
using UnityEngine;

// Damage and push for an explosion. Used by the frag grenade (and the fire zone for its damage).
public static class Explosion
{
    // All living things (player, dummies) inside the radius.
    public static List<Health> FindLiving(Vector3 source, float radius)
    {
        var result = new List<Health>();
        Collider[] near = Physics.OverlapSphere(source, radius, ~0, QueryTriggerInteraction.Ignore);

        foreach (Collider c in near)
        {
            Health h = c.GetComponentInParent<Health>();
            if (h != null && !result.Contains(h)) result.Add(h);
        }
        return result;
    }

    public static void Blast(Vector3 origin, float radius, float maxDamage, float pushForce)
    {
        // a little above the ground so the rays do not start inside the floor
        Vector3 source = origin + Vector3.up * 0.2f;

        // 1) living things: damage and knockback depend on distance and on how exposed they are
        foreach (Health h in FindLiving(source, radius))
        {
            float exposure = ExplosionVisibility.Exposure(source, h);
            if (exposure <= 0f) continue;   // fully covered

            float distance = Vector3.Distance(source, h.Bounds.ClosestPoint(source));
            float falloff = 1f - Mathf.Clamp01(distance / radius);
            float strength = falloff * exposure;

            h.TakeDamage(maxDamage * strength);

            PlayerMover mover = h.GetComponent<PlayerMover>();
            if (mover != null)
            {
                Vector3 away = (h.Bounds.center - source).normalized + Vector3.up * 0.4f;
                mover.AddImpulse(away.normalized * pushForce * strength);
            }
        }

        // 2) loose objects (crates and barrels) fly away
        Collider[] near = Physics.OverlapSphere(source, radius, ~0, QueryTriggerInteraction.Ignore);
        var pushed = new List<Rigidbody>();

        foreach (Collider c in near)
        {
            Rigidbody rb = c.attachedRigidbody;
            if (rb == null || rb.isKinematic || pushed.Contains(rb)) continue;
            if (rb.GetComponentInParent<Health>() != null) continue;
            pushed.Add(rb);

            float visible = ExplosionVisibility.PointFactor(source, rb.worldCenterOfMass);
            if (visible <= 0f) continue;

            rb.AddExplosionForce(pushForce * visible, source, radius, 0.5f, ForceMode.VelocityChange);
        }
    }
}
