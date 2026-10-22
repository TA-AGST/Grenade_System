using UnityEngine;

// Base class for all grenades. It flies with GrenadePhysics, counts down the fuse and calls OnExplode().
public abstract class Grenade : MonoBehaviour
{
    [SerializeField] private LayerMask collisionMask = ~0;
    [SerializeField] private float spinSpeed = 540f;

    private Vector3 velocity;
    private bool atRest;
    private float fuseLeft;
    private bool exploded;

    public void Launch(Vector3 startVelocity, float fuse)
    {
        velocity = startVelocity;
        fuseLeft = fuse;
    }

    private void FixedUpdate()
    {
        if (exploded) return;

        Vector3 position = transform.position;
        GrenadePhysics.Step(ref position, ref velocity, ref atRest, collisionMask);
        transform.position = position;

        if (!atRest) transform.Rotate(Vector3.right, spinSpeed * GrenadePhysics.TimeStep);

        fuseLeft -= GrenadePhysics.TimeStep;
        if (fuseLeft <= 0f) Explode();
    }

    public void Explode()
    {
        if (exploded) return;
        exploded = true;

        OnExplode(transform.position);
        Destroy(gameObject);
    }

    protected abstract void OnExplode(Vector3 position);
}
