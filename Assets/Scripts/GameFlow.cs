using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Death screen for the player and R to restart the scene.
public class GameFlow : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private PlayerMover mover;
    [SerializeField] private GrenadeThrower thrower;
    [SerializeField] private GameObject deathPanel;

    private void OnEnable()
    {
        playerHealth.Died += OnPlayerDied;
    }

    private void OnDisable()
    {
        playerHealth.Died -= OnPlayerDied;
    }

    private void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb != null && kb.rKey.wasPressedThisFrame)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnPlayerDied()
    {
        deathPanel.SetActive(true);
        mover.CanMove = false;
        thrower.enabled = false;
    }
}
