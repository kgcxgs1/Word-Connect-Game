using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class CoinManager : MonoBehaviour
{
    float thirdPartOfLevelTime;
    [NonSerialized] public int currentCoins, TotalCoinAmount, TotalSavedCoins;
    public GameObject coinPrefab;
    [NonSerialized] GameObject displayTotalCoinAmountLayer, LostHintCoinAmountLayer, warningHintLayer;
    GameObject glow;
    public GameObject[] allStars;
    public TextMeshProUGUI coinText, finalCoinsTextInWinPanel;
    public TextMeshProUGUI displayTotalCoinAmountText, savedCoinsTextInGameOverPanel, savedCoinsTextInPausedPanel, savedCoinsTextInHintPanel, savedCoinsTextInWinPanel;
    public Vector2 coinFinalDestinationTopRight, coinFinalDestinationInPanel;  // Changed to Vector2
    public Canvas canvas;
    public RectTransform shapeTransformInGameOverPanel, shapeTransformInPausePanel, shapeTransformInHintPanel, shapeTransformInWinPanel;

    WordSelector wordSelectorScript;

    void Start()
    {
        wordSelectorScript = FindObjectOfType<WordSelector>();
        glow = GameObject.FindWithTag("Glow");
        displayTotalCoinAmountLayer = GameObject.FindWithTag("DisplayCoinAmountText");
        LostHintCoinAmountLayer = GameObject.FindWithTag("LostHintCoins");
        warningHintLayer = GameObject.FindWithTag("WarningHintMessage");

        displayTotalCoinAmountText = displayTotalCoinAmountLayer.GetComponentInChildren<TextMeshProUGUI>();

        UpdateCoinDisplay();
        displayTotalCoinAmountLayer.SetActive(false);
        LostHintCoinAmountLayer.SetActive(false);
        warningHintLayer.SetActive(false);
        glow.SetActive(false);
        DeactivateAllStars();
        TotalSavedCoins = PlayerPrefs.GetInt("SavedCoins", 0);
        finalCoinsTextInWinPanel.text = TotalSavedCoins.ToString();

        currentCoins = 0;
    }

    void UpdateCoinDisplay()
    {
        if (coinText != null)
        {
            coinText.text = currentCoins.ToString(); // Display only the coin amount
        }
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateCoinDisplay();
    }

    public void SubtractCoins(int amount)
    {
        currentCoins -= amount;
        UpdateCoinDisplay();
    }

    public void SpawnCoinsOnCorrectCubes(List<GameObject> correctCubes, int numberOfCoins = 5, float delayBetweenCoins = 0.03f)
    {
        foreach (var cube in correctCubes)
        {
            Vector2 screenPosition = WorldToScreenPosition(cube.transform.position);

            for (int i = 0; i < numberOfCoins; i++)
            {
                // Instantiate coin prefab as a UI element
                GameObject coin = Instantiate(coinPrefab, canvas.transform);
                RectTransform coinRect = coin.GetComponent<RectTransform>();

                // Set initial position of the coin
                coinRect.anchoredPosition = screenPosition;

                // Start the animation coroutine
                StartCoroutine(AnimateCoinFromCubeToCollect(coinRect, coinFinalDestinationTopRight, delayBetweenCoins * i));
            }
        }
        Invoke("ShowCoinDisplayTextLayer", 0.6f);
    }

    IEnumerator AnimateCoinFromCubeToCollect(RectTransform coinRect, Vector2 targetPosition, float delay)
    {
        yield return new WaitForSeconds(delay);

        float duration = 0.4f; // Duration of the animation
        Vector2 startPosition = coinRect.anchoredPosition;
        Vector2 endPosition = targetPosition;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            coinRect.anchoredPosition = Vector2.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it ends exactly at the target position
        coinRect.anchoredPosition = endPosition;

        Destroy(coinRect.gameObject);
        AddCoins(2);
        TotalCoinAmount += 2;
    }

    public void ShowCoinDisplayTextLayer()
    {
        displayTotalCoinAmountLayer.SetActive(true);
        displayTotalCoinAmountText.text = "+" + TotalCoinAmount.ToString();
        Invoke("HideCoinDisplayTextLayer", 0.8f);
    }

    public void HideCoinDisplayTextLayer()
    {
        displayTotalCoinAmountLayer.SetActive(false);
        TotalCoinAmount = 0;
        LostHintCoinAmountLayer.SetActive(false);
        warningHintLayer.SetActive(false);
    }

    public void ShowLostHintCoins()
    {
        LostHintCoinAmountLayer.SetActive(true);
        Invoke("HideCoinDisplayTextLayer", 1f);
    }

    public void ShowWarningHintLayer()
    {
        warningHintLayer.SetActive(true);
        Invoke("HideCoinDisplayTextLayer", 1f);
    }

    public void MeasuringLevelTime()
    {
        float TotallevelTime = wordSelectorScript.TotallevelTime;

        float timeRemaining = PlayerPrefs.GetFloat("RemainingTime", wordSelectorScript.timeRemaining);

        thirdPartOfLevelTime = TotallevelTime / 3;

        Debug.Log("TotallevelTime: " + TotallevelTime);
        Debug.Log("TimeRemaining: " + timeRemaining);

        if (timeRemaining < TotallevelTime && timeRemaining > thirdPartOfLevelTime * 2)
        {
            Debug.Log("1st statement ..... 3 Stars");
            StartCoroutine(ActivateStars(3));
        }
        else if (timeRemaining < thirdPartOfLevelTime * 2 && timeRemaining > thirdPartOfLevelTime)
        {
            Debug.Log("2nd statement ..... 2 Stars");
            StartCoroutine(ActivateStars(2));
        }
        else if (timeRemaining < thirdPartOfLevelTime && timeRemaining > 0f)
        {
            Debug.Log("3rd statement ..... 1 Stars");
            StartCoroutine(ActivateStars(1));
        }

    }


    IEnumerator ActivateStars(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (i < allStars.Length && allStars[i] != null)
            {
                allStars[i].SetActive(true);
                yield return new WaitForSeconds(0.6f);
            }
        }
        StartCoroutine(IncreasingSavedCoins());
        glow.SetActive(true);
    }

    void DeactivateAllStars()
    {
        foreach (GameObject star in allStars)
        {
            if (star != null)
            {
                star.SetActive(false);
            }
        }
    }


    public IEnumerator IncreasingSavedCoins()
    {
        finalCoinsTextInWinPanel.text = TotalSavedCoins.ToString();

        StartCoroutine(SpawnAndAnimateCoinsToSave(10, 0.001f, 0.001f));  // Adjust the spawn delay here

        int startCoins = currentCoins;
        int targetCoins = 0;
        int savedTargetCoins = TotalSavedCoins + currentCoins;

        while (startCoins >= targetCoins)
        {
            startCoins -= 3;
            TotalSavedCoins += 3;

            // Update the displays
            coinText.text = startCoins.ToString();
            finalCoinsTextInWinPanel.text = "+" + TotalSavedCoins.ToString();

            yield return new WaitForSeconds(0.001f);
        }

        finalCoinsTextInWinPanel.text = TotalSavedCoins.ToString();
        coinText.text = 0.ToString();

        // Ensure the final values are set correctly
        currentCoins = targetCoins;
        TotalSavedCoins = savedTargetCoins;

        PlayerPrefs.SetInt("SavedCoins", TotalSavedCoins);
    }

    private IEnumerator SpawnAndAnimateCoinsToSave(int coinPrefabsToSpawn, float coinAnimationDuration, float coinDelayBetweenSpawns)
    {
        for (int i = 0; i < coinPrefabsToSpawn; i++)
        {
            // Instantiate the coinPrefab at the start position
            GameObject coin = Instantiate(coinPrefab, canvas.transform);
            RectTransform coinRect = coin.GetComponent<RectTransform>();
            coinRect.anchoredPosition = coinFinalDestinationTopRight;

            // Start the animation coroutine
            StartCoroutine(AnimateCoinFromCollectToSave(coinRect, coinFinalDestinationInPanel, 0f));

            // Wait before spawning the next coin
            yield return new WaitForSeconds(coinDelayBetweenSpawns);
        }
    }

    IEnumerator AnimateCoinFromCollectToSave(RectTransform coinRect, Vector2 targetPosition, float delay)
    {
        yield return new WaitForSeconds(delay);

        float duration = 0.4f; // Duration of the animation
        Vector2 startPosition = coinRect.anchoredPosition;
        Vector2 endPosition = targetPosition;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            coinRect.anchoredPosition = Vector2.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it ends exactly at the target position
        coinRect.anchoredPosition = endPosition;

        Destroy(coinRect.gameObject);
    }

    Vector2 WorldToScreenPosition(Vector3 worldPosition)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        // Adjust the screen position to be relative to the canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPosition, null, out Vector2 localPoint);
        return localPoint;
    }
}
