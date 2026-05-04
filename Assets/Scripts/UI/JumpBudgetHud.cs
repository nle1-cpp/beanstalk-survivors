using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JumpBudgetHud : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private PlayerMovement playerMovement;

    [Header("UI Elements")]
    [SerializeField] private Image radialFillImage;
    [SerializeField] private TextMeshProUGUI countText;

    [Header("Layout")]
    [SerializeField] private float size = 96f;
    [SerializeField] private Vector2 bottomRightOffset = new Vector2(-40f, 40f);

    private static Sprite generatedCircleSprite;

    public PlayerMovement PlayerMovement => playerMovement;
    public Image RadialFillImage => radialFillImage;
    public TextMeshProUGUI CountText => countText;

    private void Awake()
    {
        EnsurePlayerReference();
        EnsureRectTransformLayout();
        EnsureVisuals();
        Refresh();
    }

    private void Update()
    {
        EnsurePlayerReference();
        Refresh();
    }

    private void EnsurePlayerReference()
    {
        if (playerMovement == null)
        {
            playerMovement = Object.FindFirstObjectByType<PlayerMovement>();
        }
    }

    private void EnsureRectTransformLayout()
    {
        RectTransform rectTransform = transform as RectTransform;
        if (rectTransform == null)
        {
            return;
        }

        rectTransform.anchorMin = new Vector2(1f, 0f);
        rectTransform.anchorMax = new Vector2(1f, 0f);
        rectTransform.pivot = new Vector2(1f, 0f);
        rectTransform.anchoredPosition = bottomRightOffset;
        rectTransform.sizeDelta = new Vector2(size, size);
    }

    private void EnsureVisuals()
    {
        if (radialFillImage == null)
        {
            radialFillImage = CreateRadialFillImage();
        }

        radialFillImage.type = Image.Type.Filled;
        radialFillImage.fillMethod = Image.FillMethod.Radial360;
        radialFillImage.fillOrigin = (int)Image.Origin360.Top;
        radialFillImage.fillClockwise = true;
        radialFillImage.sprite = radialFillImage.sprite != null ? radialFillImage.sprite : GetGeneratedCircleSprite();
        radialFillImage.color = new Color(0.35f, 0.85f, 1f, 0.75f);

        if (countText == null)
        {
            countText = CreateCountText();
        }

        countText.alignment = TextAlignmentOptions.Center;
        countText.fontSize = 42f;
        countText.color = Color.white;
        countText.enableAutoSizing = false;
    }

    private Image CreateRadialFillImage()
    {
        GameObject imageObject = new GameObject("Air Jump Cooldown Pie", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imageObject.transform.SetParent(transform, false);

        RectTransform imageRect = imageObject.GetComponent<RectTransform>();
        imageRect.anchorMin = Vector2.zero;
        imageRect.anchorMax = Vector2.one;
        imageRect.offsetMin = Vector2.zero;
        imageRect.offsetMax = Vector2.zero;

        return imageObject.GetComponent<Image>();
    }

    private TextMeshProUGUI CreateCountText()
    {
        GameObject textObject = new GameObject("Air Jump Count", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(transform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return textObject.GetComponent<TextMeshProUGUI>();
    }

    private void Refresh()
    {
        if (playerMovement == null || radialFillImage == null || countText == null)
        {
            return;
        }

        radialFillImage.fillAmount = playerMovement.AirJumpRefreshProgress01;
        countText.text = playerMovement.AvailableAirJumps.ToString();
    }

    private static Sprite GetGeneratedCircleSprite()
    {
        if (generatedCircleSprite != null)
        {
            return generatedCircleSprite;
        }

        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false)
        {
            name = "JumpBudgetHudCircleTexture"
        };
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        generatedCircleSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        generatedCircleSprite.name = "JumpBudgetHudCircleSprite";
        return generatedCircleSprite;
    }
}
