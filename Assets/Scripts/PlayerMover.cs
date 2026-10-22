using UnityEngine;
using UnityEngine.InputSystem;

// WASD to walk, mouse to look, Space to jump, Esc to free the cursor.
// Explosions can push the player through AddImpulse().
[RequireComponent(typeof(CharacterController))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float jumpHeight = 1.1f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float lookSensitivity = 0.12f;
    [SerializeField] private float minPitch = -60f;
    [SerializeField] private float maxPitch = 70f;
    [SerializeField] private float pushDamping = 6f;

    private CharacterController controller;
    private float yaw;
    private float pitch;
    private float verticalVelocity;
    private Vector3 push;

    public bool CanMove { get; set; } = true;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        yaw = transform.eulerAngles.y;
        SetCursorLocked(true);
    }

    public void AddImpulse(Vector3 impulse)
    {
        verticalVelocity += impulse.y;
        impulse.y = 0f;
        push += impulse;
    }

    private void Update()
    {
        Keyboard kb = Keyboard.current;
        Mouse mouse = Mouse.current;
        if (kb == null || mouse == null) return;

        if (kb.escapeKey.wasPressedThisFrame) SetCursorLocked(false);
        else if (mouse.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked) SetCursorLocked(true);

        bool locked = Cursor.lockState == CursorLockMode.Locked;
        if (CanMove && locked) Look(mouse);

        Move(kb, CanMove && locked);
    }

    private void Look(Mouse mouse)
    {
        Vector2 delta = mouse.delta.ReadValue() * lookSensitivity;
        yaw += delta.x;
        pitch = Mathf.Clamp(pitch - delta.y, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void Move(Keyboard kb, bool active)
    {
        Vector2 input = Vector2.zero;
        if (active)
        {
            if (kb.wKey.isPressed) input.y += 1f;
            if (kb.sKey.isPressed) input.y -= 1f;
            if (kb.dKey.isPressed) input.x += 1f;
            if (kb.aKey.isPressed) input.x -= 1f;
            input = Vector2.ClampMagnitude(input, 1f);
        }

        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
        if (active && controller.isGrounded && kb.spaceKey.wasPressedThisFrame)
            verticalVelocity = Mathf.Sqrt(2f * -gravity * jumpHeight);
        verticalVelocity += gravity * Time.deltaTime;

        push = Vector3.Lerp(push, Vector3.zero, pushDamping * Time.deltaTime);

        Vector3 walk = (transform.right * input.x + transform.forward * input.y) * walkSpeed;
        controller.Move((walk + push + Vector3.up * verticalVelocity) * Time.deltaTime);
    }

    private static void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
