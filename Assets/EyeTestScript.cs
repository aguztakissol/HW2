using UnityEngine;
using TMPro;

public class EyeTestPrefabGenerator : MonoBehaviour
{
    public Canvas worldCanvas;       // Assign your EyeTestCanvas here
    public TMP_Text letterPrefab;    // Assign LetterPrefab here
    public string letters = "ABCDEF"; // Letters to display
    public float startY = 0f;         // Y-position of top letter
    public float verticalSpacing = -100f; // Spacing between letters
    public float startingFontSize = 200f; // Biggest letter
    public float fontSizeDecrement = 30f; // Decrease per row

    void Start()
    {
        GenerateEyeTest();
    }

    public void GenerateEyeTest()
    {
        float currentFontSize = startingFontSize;
        float currentY = startY;

        foreach (char c in letters)
        {
            TMP_Text letter = Instantiate(letterPrefab, worldCanvas.transform);
            letter.text = c.ToString();
            letter.fontSize = currentFontSize;
            letter.alignment = TextAlignmentOptions.Center;

            RectTransform rt = letter.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, currentY);

            currentY += verticalSpacing;
            currentFontSize -= fontSizeDecrement;
        }
    }
}
