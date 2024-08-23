using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SpecificLevel : MonoBehaviour
{
    float thirdPartOfLevelTime;
    public float animationDuration;

    public Sprite previousLevelsSprite, reachedLevelSprite, lockedLevelSprite;

    public GameObject start1Prefab, start2Prefab, start3Prefab;

    LevelManager levelsManagerScript;
    WordSelector wordSelectorScript;


    public List<Button> levelButtons;

    private const string StarKeyPrefix = "LevelStar_"; 


    private void Start() 
    {
        levelsManagerScript = FindObjectOfType<LevelManager>();
        wordSelectorScript = FindObjectOfType<WordSelector>();

        levelButtons = levelsManagerScript.levelButtons;

        UpdateButtonImages();
        RewardingStars();

        // Load stars after initializing levels
        LoadStars();
        
    }

    void UpdateButtonImages()
    {
        int levelReached = PlayerPrefs.GetInt("LevelReached", 1);

        for (int i = 0; i < levelButtons.Count; i++)
        {
            int levelIndex = i + 1;
            Image buttonImage = levelButtons[i].GetComponent<Image>();

            if (levelIndex < levelReached)
            {
                buttonImage.sprite = previousLevelsSprite;
            }
            else if (levelIndex == levelReached)
            {
                buttonImage.sprite = reachedLevelSprite;
            }
            else
            {
                buttonImage.sprite = lockedLevelSprite;
            }
        }
    }

    public void RewardingStars()
    {
        if (!GameEvents.rewardStarsBool) return;

        int levelReached = PlayerPrefs.GetInt("LevelReached", 1);
        int justCompletedLevel = PlayerPrefs.GetInt("JustCompletedLevel", 0);

        for (int i = 0; i < levelButtons.Count; i++)
        {
            int levelIndex = i + 1;

            if (levelIndex == levelReached)
            {
                MeasuringLevelTime(levelButtons[justCompletedLevel]);
            }
        }

        GameEvents.rewardStarsBool = false;
    }

    public void MeasuringLevelTime(Button levelButton)
    {
        string lastLoadedScene = PlayerPrefs.GetString("LastLoadedScene", "DefaultScene");
        string totalLevelTimeKey = "TotallevelTime_" + lastLoadedScene;
        float TotallevelTime = PlayerPrefs.GetFloat(totalLevelTimeKey, 1500f);

        float timeRemaining = PlayerPrefs.GetFloat("RemainingTime", wordSelectorScript.timeRemaining);

        thirdPartOfLevelTime = TotallevelTime / 3;

        Debug.Log("TotallevelTime: " + TotallevelTime);
        Debug.Log("TimeRemaining: " + timeRemaining);

        if (timeRemaining < TotallevelTime && timeRemaining > thirdPartOfLevelTime * 2)
            {
                Debug.Log("1st statement ..... 3 Stars");
                StartCoroutine(AnimateStart1(levelButton));
                StartCoroutine(AnimateStart2(levelButton));
                StartCoroutine(AnimateStart3(levelButton));
            }
            else if (timeRemaining < thirdPartOfLevelTime * 2 && timeRemaining > thirdPartOfLevelTime)
            {
                Debug.Log("2nd statement ..... 2 Stars");
                StartCoroutine(AnimateStart1(levelButton));
                StartCoroutine(AnimateStart2(levelButton));
            }
            else if (timeRemaining < thirdPartOfLevelTime && timeRemaining > 0f)
            {
                Debug.Log("3rd statement ..... 1 Stars");
                StartCoroutine(AnimateStart1(levelButton));
            }

    }

    IEnumerator AnimateStart1(Button levelButton)
    {
        int levelIndex = levelButtons.IndexOf(levelButton) + 1;
        if (PlayerPrefs.GetInt(StarKeyPrefix + levelIndex + "_1", 0) == 0)
        {
            GameObject start1 = Instantiate(start1Prefab, levelButton.transform);
            start1.name = "start1";

            DontDestroyOnLoad(start1);

            // Store star data 
            levelIndex = levelButtons.IndexOf(levelButton) + 1;
            PlayerPrefs.SetInt(StarKeyPrefix + levelIndex + "_1", 1);
        }
        yield return null;
    }

    IEnumerator AnimateStart2(Button levelButton, float delay = 0.6f)
    {
        yield return new WaitForSeconds(delay);

        int levelIndex = levelButtons.IndexOf(levelButton) + 1;
        if (PlayerPrefs.GetInt(StarKeyPrefix + levelIndex + "_2", 0) == 0)
        {
            GameObject start2 = Instantiate(start2Prefab, levelButton.transform);
            start2.name = "start2"; 

            DontDestroyOnLoad(start2);

            // Store star data 
            levelIndex = levelButtons.IndexOf(levelButton) + 1;
            PlayerPrefs.SetInt(StarKeyPrefix + levelIndex + "_2", 1); 
        }

    }

    IEnumerator AnimateStart3(Button levelButton, float delay = 1.2f)
    {
        yield return new WaitForSeconds(delay);

        int levelIndex = levelButtons.IndexOf(levelButton) + 1;
        if (PlayerPrefs.GetInt(StarKeyPrefix + levelIndex + "_3", 0) == 0)
        {
            GameObject start3 = Instantiate(start3Prefab, levelButton.transform);
            start3.name = "start3"; 

            DontDestroyOnLoad(start3);

            // Store star data
            levelIndex = levelButtons.IndexOf(levelButton) + 1;
            PlayerPrefs.SetInt(StarKeyPrefix + levelIndex + "_3", 1);
        }
        
    }

    private void LoadStars()
    {
        for (int i = 0; i < levelButtons.Count; i++)
        {
            int levelIndex = i + 1;
            LoadStarForLevel(levelButtons[i], levelIndex);
        }
    }

    private void LoadStarForLevel(Button levelButton, int levelIndex)
    {
        for (int starNum = 1; starNum <= 3; starNum++)
        {
            if (PlayerPrefs.GetInt(StarKeyPrefix + levelIndex + "_" + starNum, 0) == 1)
            {
                CreateStar(levelButton, starNum); 
            }
        }
    }

    private void CreateStar(Button levelButton, int starNum)
    {
        GameObject starPrefab = null;

        switch (starNum)
        {
            case 1:
                starPrefab = start1Prefab;
                break;
            case 2:
                starPrefab = start2Prefab;
                break;
            case 3:
                starPrefab = start3Prefab;
                break;
        }

        if (starPrefab != null)
        {
            GameObject star = Instantiate(starPrefab, levelButton.transform);
            if (star != null) { star.GetComponent<Animator>().enabled = false; }
            DontDestroyOnLoad(star); 
        }
    }
}


