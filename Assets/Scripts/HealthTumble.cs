using System.Collections;
using UnityEngine;

// When the dummy dies it falls over.
[RequireComponent(typeof(Health))]
public class HealthTumble : MonoBehaviour
{
    [SerializeField] private float fallTime = 0.4f;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.Died += OnDied;
    }

    private void OnDisable()
    {
        health.Died -= OnDied;
    }

    private void OnDied()
    {
        StartCoroutine(FallOver());
    }

    private IEnumerator FallOver()
    {
        Quaternion start = transform.rotation;
        Quaternion end = start * Quaternion.Euler(90f, 0f, 0f);
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + Vector3.down * 0.5f;

        for (float t = 0f; t < 1f; t += Time.deltaTime / fallTime)
        {
            transform.rotation = Quaternion.Slerp(start, end, t);
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }
        transform.rotation = end;
        transform.position = endPosition;
    }
}
