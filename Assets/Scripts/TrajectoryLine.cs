using System.Collections.Generic;
using UnityEngine;

// The red line that shows where the grenade will go, bounce and finally explode.
// It plays the grenade physics forward for the time left on the fuse.
[RequireComponent(typeof(LineRenderer))]
public class TrajectoryLine : MonoBehaviour
{
    [SerializeField] private Transform endMarker;
    [SerializeField] private LayerMask collisionMask = ~0;

    private LineRenderer line;
    private readonly List<Vector3> points = new List<Vector3>();

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        Hide();
    }

    public void Show(Vector3 start, Vector3 velocity, float fuseLeft)
    {
        points.Clear();
        points.Add(start);

        Vector3 position = start;
        bool atRest = false;
        int steps = Mathf.CeilToInt(fuseLeft / GrenadePhysics.TimeStep);

        for (int i = 0; i < steps; i++)
        {
            GrenadePhysics.Step(ref position, ref velocity, ref atRest, collisionMask);
            points.Add(position);
            if (atRest) break;
        }

        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());
        line.enabled = true;

        if (endMarker != null)
        {
            endMarker.position = position;
            endMarker.gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        if (line != null) line.enabled = false;
        if (endMarker != null) endMarker.gameObject.SetActive(false);
    }
}
