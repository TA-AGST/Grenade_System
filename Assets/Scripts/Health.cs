using System;
using UnityEngine;

// Simple health used by the player and by the target dummies.
[RequireComponent(typeof(Collider))]
public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    private Collider body;

    public float Max => maxHealth;
    public float Current { get; private set; }
    public bool IsDead => Current <= 0f;
    public Bounds Bounds => body.bounds;

    public event Action Changed;
    public event Action Died;

    private void Awake()
    {
        body = GetComponent<Collider>();
        Current = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f) return;

        Current = Mathf.Max(0f, Current - amount);
        Changed?.Invoke();

        if (IsDead) Died?.Invoke();
    }
}
