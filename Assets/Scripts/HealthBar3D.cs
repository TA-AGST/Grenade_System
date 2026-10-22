using UnityEngine;

// Small bar above a dummy. It always faces the camera and shrinks as the health goes down.
public class HealthBar3D : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Transform fill;

    private void LateUpdate()
    {
        if (health.IsDead)
        {
            gameObject.SetActive(false);
            return;
        }

        float ratio = health.Current / health.Max;
        fill.localScale = new Vector3(ratio, 1f, 1f);
        fill.localPosition = new Vector3(-(1f - ratio) * 0.5f, 0f, -0.01f);

        Camera cam = Camera.main;
        if (cam != null) transform.forward = transform.position - cam.transform.position;
    }
}
