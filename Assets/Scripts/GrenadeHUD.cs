using UnityEngine;
using UnityEngine.UI;

// Shows the player's health, the 4 grenade slots with ammo, the throw mode and the fuse while cooking.
public class GrenadeHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GrenadeThrower thrower;
    [SerializeField] private Health playerHealth;

    [Header("Health")]
    [SerializeField] private RectTransform healthFill;
    [SerializeField] private Text healthText;

    [Header("Grenade slots (one per grenade type)")]
    [SerializeField] private Image[] slotImages;
    [SerializeField] private Text[] slotLabels;

    [Header("Fuse and throw mode")]
    [SerializeField] private GameObject fuseRoot;
    [SerializeField] private RectTransform fuseFill;
    [SerializeField] private Text modeText;

    private void Update()
    {
        UpdateHealth();
        UpdateSlots();
        UpdateFuse();
    }

    private void UpdateHealth()
    {
        float ratio = playerHealth.Current / playerHealth.Max;
        healthFill.anchorMax = new Vector2(ratio, 1f);
        healthText.text = Mathf.CeilToInt(playerHealth.Current) + " / " + Mathf.CeilToInt(playerHealth.Max);
    }

    private void UpdateSlots()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            GrenadeData data = thrower.Grenades[i];
            bool isSelected = i == thrower.Selected;

            slotLabels[i].text = (i + 1) + "  " + data.displayName + "\nx" + thrower.AmmoOf(i);

            Color c = data.color;
            c.a = isSelected ? 0.95f : 0.35f;
            slotImages[i].color = c;
        }
    }

    private void UpdateFuse()
    {
        modeText.text = "Throw: " + thrower.ModeLabel.ToUpper() + "   (Q to change)";

        fuseRoot.SetActive(thrower.IsCooking);
        if (thrower.IsCooking) fuseFill.anchorMax = new Vector2(thrower.FuseLeft01, 1f);
    }
}
