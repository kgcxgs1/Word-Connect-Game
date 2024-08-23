
using System.Collections;
using TMPro;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    Vector2 padding = new Vector2(150, 30);
    public TextMeshProUGUI totalSavedCoinsTopRight;
    public RectTransform shapeRectTransform;

    Ad_Manager adManagerScript;


    private void Start() 
    {
        int TotalSavedCoins = PlayerPrefs.GetInt("SavedCoins", 0);
        totalSavedCoinsTopRight.text = TotalSavedCoins.ToString();
        AdjustShapeSizeAndPosition();
        adManagerScript = FindObjectOfType<Ad_Manager>();

        StartCoroutine(adManagerScript.RewardedAdShow(8f));
        StartCoroutine(LoadBannerAd());

    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void AdjustShapeSizeAndPosition()
    {
        Vector2 textSize = totalSavedCoinsTopRight.GetPreferredValues();
        shapeRectTransform.sizeDelta = textSize + padding;
    }

    public IEnumerator LoadBannerAd()
    {
        float randomDelay = Random.Range(1f, 4f);
        yield return new WaitForSeconds(randomDelay);

        adManagerScript.BannerAdLoad();
    }
}
