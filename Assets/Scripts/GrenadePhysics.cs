using UnityEngine;

// Tiny physics for grenades (gravity + bounce).
// The real grenade and the red preview line both call Step(), so the line always matches the real path.
public static class GrenadePhysics
{
    public const float TimeStep = 0.02f;   // same as Unity's default fixed time step
    public const float Radius = 0.12f;
    public const float Gravity = -14f;
    public const float Bounciness = 0.5f;  // how much speed is kept when bouncing off a surface
    public const float Friction = 0.8f;    // how much speed is kept while sliding along a surface
    public const float RestSpeed = 1.2f;   // slower than this on the ground = stop moving

    private static readonly RaycastHit[] hits = new RaycastHit[8];

    // Moves the grenade by one step. Returns true if it hit something.
    public static bool Step(ref Vector3 position, ref Vector3 velocity, ref bool atRest, LayerMask mask)
    {
        if (atRest) return false;

        velocity.y += Gravity * TimeStep;
        Vector3 move = velocity * TimeStep;
        float distance = move.magnitude;
        if (distance < 0.0001f) return false;

        Vector3 direction = move / distance;
        if (!FindHit(position, direction, distance, mask, out RaycastHit hit))
        {
            position += move;
            return false;
        }

        // go up to the surface, then bounce
        position += direction * Mathf.Max(0f, hit.distance - 0.001f);

        Vector3 normal = hit.normal;
        float into = Vector3.Dot(velocity, normal);
        if (into < 0f)
        {
            Vector3 intoPart = into * normal;
            Vector3 slidePart = velocity - intoPart;
            velocity = slidePart * Friction - intoPart * Bounciness;
        }

        if (normal.y > 0.7f && velocity.magnitude < RestSpeed)
        {
            velocity = Vector3.zero;
            atRest = true;
        }
        return true;
    }

    // Nearest hit that is not a living thing (players and dummies do not stop grenades).
    private static bool FindHit(Vector3 origin, Vector3 direction, float distance, LayerMask mask, out RaycastHit nearest)
    {
        int count = Physics.SphereCastNonAlloc(origin, Radius, direction, hits, distance, mask, QueryTriggerInteraction.Ignore);

        nearest = default;
        float best = float.MaxValue;
        bool found = false;

        for (int i = 0; i < count; i++)
        {
            if (hits[i].collider.GetComponentInParent<Health>() != null) continue;
            if (hits[i].distance < best)
            {
                best = hits[i].distance;
                nearest = hits[i];
                found = true;
            }
        }
        return found;
    }
}
