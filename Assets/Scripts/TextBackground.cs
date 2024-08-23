using UnityEngine;
using TMPro;
using System.Collections;

public class TextBackground : MonoBehaviour
{
    public float moveDuration;
    public RectTransform shapeTransform;
    private TextMeshProUGUI textMeshPro;
    Vector2 padding = new Vector2(50, 0);
    public Vector2 moveToPosition; // The local position where the text's parent will move after becoming static

    public Animator animatorOfParent;
    private RectTransform fatherParentDisplayWord;

    CoinManager coinManagerScript;

    void Start()
    {
        animatorOfParent = GetComponentInParent<Animator>();
        textMeshPro = GetComponent<TextMeshProUGUI>();
        fatherParentDisplayWord = transform.parent.parent.GetComponent<RectTransform>();
        coinManagerScript = FindObjectOfType<CoinManager>();
    }

    void Update()
    {
        if (textMeshPro != null && shapeTransform != null)
        {
            AdjustShapeSizeAndPosition();
        }
        AdjustTextSizeInGameOverPanel();
        AdjustTextSizeInPausePanel();
        AdjustTextSizeInHintPanel();
        AdjustTextSizeInWinPanel();
    }

    void AdjustShapeSizeAndPosition()
    {
        // Get the size of the text
        Vector2 textSize = textMeshPro.GetPreferredValues();

        if (textMeshPro.text == "")
        {
            shapeTransform.sizeDelta = textSize;
        }
        else
        {
            // Set the size of the shape layer to match the text size plus the size offset
            shapeTransform.sizeDelta = textSize + padding;

            // Set the shape layer's position to match the text position plus the position offset
            shapeTransform.position = textMeshPro.rectTransform.position;

            // Optionally, adjust the pivot of the shape layer if needed
            shapeTransform.pivot = textMeshPro.rectTransform.pivot;
        }
    }

    public void AdjustTextSizeInGameOverPanel()
    {
        Vector2 imageSize = coinManagerScript.shapeTransformInGameOverPanel.rect.size;

        // Adjust the text size based on the image size and padding
        coinManagerScript.savedCoinsTextInGameOverPanel.rectTransform.sizeDelta = new Vector2(imageSize.x , imageSize.y);

        // Dynamically adjust font size to fit the text inside the target image
        coinManagerScript.savedCoinsTextInGameOverPanel.enableAutoSizing = true;
        coinManagerScript.savedCoinsTextInGameOverPanel.fontSizeMin = 10; // Minimum font size
        coinManagerScript.savedCoinsTextInGameOverPanel.fontSizeMax = 100; // Maximum font size
    }

    public void AdjustTextSizeInPausePanel()
    {
        Vector2 imageSize = coinManagerScript.shapeTransformInPausePanel.rect.size;

        // Adjust the text size based on the image size and padding
        coinManagerScript.savedCoinsTextInPausedPanel.rectTransform.sizeDelta = new Vector2(imageSize.x , imageSize.y);

        // Dynamically adjust font size to fit the text inside the target image
        coinManagerScript.savedCoinsTextInPausedPanel.enableAutoSizing = true;
        coinManagerScript.savedCoinsTextInPausedPanel.fontSizeMin = 10; // Minimum font size
        coinManagerScript.savedCoinsTextInPausedPanel.fontSizeMax = 100; // Maximum font size
    }

    public void AdjustTextSizeInHintPanel()
    {
        Vector2 imageSize = coinManagerScript.shapeTransformInHintPanel.rect.size;

        // Adjust the text size based on the image size and padding
        coinManagerScript.savedCoinsTextInHintPanel.rectTransform.sizeDelta = new Vector2(imageSize.x , imageSize.y);

        // Dynamically adjust font size to fit the text inside the target image
        coinManagerScript.savedCoinsTextInHintPanel.enableAutoSizing = true;
        coinManagerScript.savedCoinsTextInHintPanel.fontSizeMin = 10; // Minimum font size
        coinManagerScript.savedCoinsTextInHintPanel.fontSizeMax = 100; // Maximum font size
    }

    public void AdjustTextSizeInWinPanel()
    {
        Vector2 imageSize = coinManagerScript.shapeTransformInWinPanel.rect.size;

        // Adjust the text size based on the image size and padding
        coinManagerScript.savedCoinsTextInWinPanel.rectTransform.sizeDelta = new Vector2(imageSize.x , imageSize.y);

        // Dynamically adjust font size to fit the text inside the target image
        coinManagerScript.savedCoinsTextInWinPanel.enableAutoSizing = true;
        coinManagerScript.savedCoinsTextInWinPanel.fontSizeMin = 10; // Minimum font size
        coinManagerScript.savedCoinsTextInWinPanel.fontSizeMax = 300; // Maximum font size
    }

    public void MoveText()
    {
        StartCoroutine(MoveParentCoroutine(moveDuration));
    }

    private IEnumerator MoveParentCoroutine(float moveDuration)
    {
        if (fatherParentDisplayWord == null)
        {
            Debug.LogError("The textandShape GameObject does not have a RectTransform.");
            yield break;
        }

        Vector2 startPosition = fatherParentDisplayWord.anchoredPosition;
        Vector2 endPosition = moveToPosition;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            fatherParentDisplayWord.anchoredPosition = Vector2.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fatherParentDisplayWord.anchoredPosition = endPosition;
    }
}


