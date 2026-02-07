using UnityEngine;
using TMPro;

[ExecuteInEditMode]  // Allows you to see letters in Editor without Play mode
public class EyeTestPrefabGenerator : MonoBehaviour
{
    [Header("Canvas & Prefab")]
    public Canvas worldCanvas;       // Assign your World Space Canvas here
    public TMP_Text letterPrefab;    // Assign TMP_Text prefab here

    [Header("Characters")]
    public string characters = "AB124C89aZTU"; // Mixed letters and numbers

    [Header("Position & Spacing")]
    public float startX = 0f;        // Starting X position
    public float startY = 0f;        // Y position of the row
    public float horizontalSpacing = 0.1f; // Spacing in Unity meters (world space)

    [Header("Font Settings")]
    public float startingFontSize = 0.1f;  // Font size in meters
    public float fontSizeDecrement = 0.01f; // Decrease per character

    [Header("Magnifier Mode")]
    public bool magnifierMode = false; // Doubles the font size for magnifier

    void OnValidate()
    {
        if (worldCanvas == null || letterPrefab == null) return;

        // Clear previous letters
        for (int i = worldCanvas.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(worldCanvas.transform.GetChild(i).gameObject);
        }

        GenerateMixedRow(characters);
    }

    public void GenerateMixedRow(string chars)
    {
        float currentFontSize = startingFontSize;
        float currentX = startX;

        foreach (char c in chars)
        {
            TMP_Text character = Instantiate(letterPrefab, worldCanvas.transform);
            character.text = c.ToString();

            // Adjust for magnifier
            float fontSizeToUse = magnifierMode ? currentFontSize * 2f : currentFontSize;
            character.fontSize = fontSizeToUse;

            character.alignment = TextAlignmentOptions.Center;

            RectTransform rt = character.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(currentX, startY, 0f); // world-space positioning
            rt.localScale = Vector3.one; // keep scale correct

            currentX += horizontalSpacing;   // move next letter
            currentFontSize -= fontSizeDecrement; // reduce font size
        }
    }
}
