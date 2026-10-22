using UnityEngine;

// Decides how much of a target the explosion can "see".
//
// One ray to one point is not enough: a thin pole would block it, but in real life the blast
// still reaches a player hiding behind a pole. So:
//  1. We check several points on the body (head, chest, hips, feet, left and right of the chest).
//  2. A thin obstacle (pole, tree trunk) only weakens the blast, a thick one (wall) blocks it fully.
// Exposure is the average over all points: 0 = fully covered, 1 = fully open.
public static class ExplosionVisibility
{
    public static bool ShowDebug;                  // F1 toggles it, draws the rays in the game

    private const float ThinLimit = 0.4f;          // thinner than this (in metres) = "thin"
    private const float ThinPass = 0.5f;           // how much of the blast gets through a thin obstacle

    private static readonly RaycastHit[] hits = new RaycastHit[16];
    private static Material debugMaterial;

    // 0..1 for a whole body (several points).
    public static float Exposure(Vector3 source, Health target)
    {
        Bounds b = target.Bounds;
        float h = b.extents.y;

        Vector3 flat = b.center - source;
        flat.y = 0f;
        Vector3 side = flat.sqrMagnitude > 0.0001f ? Vector3.Cross(Vector3.up, flat).normalized : Vector3.right;
        side *= b.extents.x * 0.8f;

        Vector3 chest = b.center + Vector3.up * (h * 0.35f);
        Vector3[] points =
        {
            b.center + Vector3.up * (h * 0.9f),    // head
            chest,                                  // chest
            b.center,                               // hips
            b.center - Vector3.up * (h * 0.8f),     // feet
            chest + side,                           // chest, one side
            chest - side                            // chest, other side
        };

        float total = 0f;
        foreach (Vector3 p in points) total += PointFactor(source, p);
        return total / points.Length;
    }

    // 0..1 for a single point: 1 = clear line, 0 = blocked by something thick.
    public static float PointFactor(Vector3 source, Vector3 point)
    {
        Vector3 toPoint = point - source;
        float distance = toPoint.magnitude;
        if (distance < 0.01f) return 1f;

        Vector3 direction = toPoint / distance;
        int count = Physics.RaycastNonAlloc(source, direction, hits, distance, ~0, QueryTriggerInteraction.Ignore);

        float factor = 1f;
        for (int i = 0; i < count; i++)
        {
            Collider c = hits[i].collider;
            if (c.GetComponentInParent<Health>() != null) continue;   // bodies do not shield

            float thickness = Thickness(c, point, direction, distance, hits[i].distance);
            factor *= thickness < ThinLimit ? ThinPass : 0f;
        }

        if (ShowDebug) DrawRay(source, point, factor);
        return factor;
    }

    // How thick the obstacle is along the ray: shoot a ray back from the target and see where it leaves the collider.
    private static float Thickness(Collider c, Vector3 point, Vector3 direction, float totalDistance, float entryDistance)
    {
        Ray back = new Ray(point, -direction);
        if (c.Raycast(back, out RaycastHit exit, totalDistance))
        {
            float exitDistance = totalDistance - exit.distance;
            return Mathf.Max(0f, exitDistance - entryDistance);
        }
        return float.MaxValue;   // could not measure it, treat it as solid
    }

    // Green = reached, yellow = passed a thin obstacle, red = blocked.
    private static void DrawRay(Vector3 from, Vector3 to, float factor)
    {
        if (debugMaterial == null) debugMaterial = new Material(Shader.Find("Sprites/Default"));

        var go = new GameObject("LOS ray", typeof(LineRenderer));
        var line = go.GetComponent<LineRenderer>();
        line.sharedMaterial = debugMaterial;
        line.positionCount = 2;
        line.SetPosition(0, from);
        line.SetPosition(1, to);
        line.widthMultiplier = 0.03f;

        Color color = factor >= 0.99f ? Color.green : (factor > 0f ? Color.yellow : Color.red);
        line.startColor = color;
        line.endColor = color;

        Object.Destroy(go, 3f);
    }
}
