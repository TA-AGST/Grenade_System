using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Handles the grenade input: pick a type (1-4), hold the left mouse button to pull the pin (the fuse starts)
// and aim, release to throw. Q switches between a short and a long throw.
// If you hold too long the grenade explodes in your hand.
public class GrenadeThrower : MonoBehaviour
{
    [Serializable]
    public class ThrowMode
    {
        public string label = "Long";
        public float speed = 18f;
        public float upAngle = 22f;     // degrees above where the camera looks
    }

    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Transform throwOrigin;
    [SerializeField] private TrajectoryLine trajectory;
    [SerializeField] private GrenadeData[] grenades;
    [SerializeField] private ThrowMode[] modes =
    {
        new ThrowMode { label = "Short", speed = 9f, upAngle = 8f },
        new ThrowMode { label = "Long", speed = 18f, upAngle = 22f }
    };

    private int[] ammo;
    private int selected;
    private int modeIndex = 1;
    private bool cooking;
    private float cookTime;

    public GrenadeData[] Grenades => grenades;
    public int Selected => selected;
    public string ModeLabel => modes[modeIndex].label;
    public bool IsCooking => cooking;
    public float FuseLeft01 => cooking ? Mathf.Clamp01(1f - cookTime / grenades[selected].fuseTime) : 0f;

    public int AmmoOf(int index) => ammo[index];

    private void Awake()
    {
        ammo = new int[grenades.Length];
        for (int i = 0; i < grenades.Length; i++) ammo[i] = grenades[i].startAmmo;
    }

    private void OnDisable()
    {
        if (trajectory != null) trajectory.Hide();
    }

    private void Update()
    {
        Keyboard kb = Keyboard.current;
        Mouse mouse = Mouse.current;
        if (kb == null || mouse == null) return;

        if (kb.f1Key.wasPressedThisFrame) ExplosionVisibility.ShowDebug = !ExplosionVisibility.ShowDebug;
        if (kb.qKey.wasPressedThisFrame) modeIndex = (modeIndex + 1) % modes.Length;

        if (!cooking) SelectWithNumberKeys(kb);

        bool cursorLocked = Cursor.lockState == CursorLockMode.Locked;

        if (!cooking && cursorLocked && mouse.leftButton.wasPressedThisFrame && ammo[selected] > 0)
        {
            cooking = true;
            cookTime = 0f;
        }

        if (cooking) UpdateCooking(mouse, cursorLocked);
    }

    private void SelectWithNumberKeys(Keyboard kb)
    {
        for (int i = 0; i < grenades.Length; i++)
        {
            Key key = (Key)((int)Key.Digit1 + i);
            if (kb[key].wasPressedThisFrame) selected = i;
        }
    }

    private void UpdateCooking(Mouse mouse, bool cursorLocked)
    {
        cookTime += Time.deltaTime;
        float fuseLeft = grenades[selected].fuseTime - cookTime;

        if (fuseLeft <= 0f)
        {
            ExplodeInHand();
            return;
        }

        trajectory.Show(throwOrigin.position, LaunchVelocity(), fuseLeft);

        if (!mouse.leftButton.isPressed || !cursorLocked) Throw(fuseLeft);
    }

    private Vector3 LaunchVelocity()
    {
        ThrowMode mode = modes[modeIndex];
        Vector3 direction = Quaternion.AngleAxis(-mode.upAngle, cameraPivot.right) * cameraPivot.forward;
        return direction * mode.speed;
    }

    private void Throw(float fuseLeft)
    {
        Grenade grenade = Instantiate(grenades[selected].prefab, throwOrigin.position, Quaternion.identity);
        grenade.Launch(LaunchVelocity(), fuseLeft);
        FinishCooking();
    }

    private void ExplodeInHand()
    {
        Grenade grenade = Instantiate(grenades[selected].prefab, throwOrigin.position, Quaternion.identity);
        grenade.Launch(Vector3.zero, 0f);
        grenade.Explode();
        FinishCooking();
    }

    private void FinishCooking()
    {
        ammo[selected]--;
        cooking = false;
        trajectory.Hide();
    }
}
