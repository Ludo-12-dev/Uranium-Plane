using UnityEngine;
using TMPro;

public class ResponsiveTMPPopup : MonoBehaviour
{
    public TextMeshPro popupText;

    [Header("Size")]
    public float widthPercent = 0.45f;
    public float heightPercent = 0.10f;

    [Header("Position")]
    public Vector2 anchoredPosition = new Vector2(0f, -80f);

    void Start()
    {
        if (popupText == null)
            popupText = GetComponent<TextMeshPro>();

        RectTransform rt = popupText.GetComponent<RectTransform>();

        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);

        rt.anchoredPosition = anchoredPosition;
        rt.sizeDelta = new Vector2(
            Screen.width * widthPercent,
            Screen.height * heightPercent
        );

        rt.localScale = Vector3.one;

        popupText.enableAutoSizing = true;
        popupText.fontSizeMin = 18;
        popupText.fontSizeMax = 64;
        popupText.alignment = TextAlignmentOptions.Center;
    }
}