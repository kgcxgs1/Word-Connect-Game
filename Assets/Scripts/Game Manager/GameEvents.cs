using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEvents : MonoBehaviour
{
    int currentSceneIndex;
    public bool LetsRotate, stopTimer;
    public static bool rewardStarsBool;


    Animator timerAnimator;
    [NonSerialized] public GameObject winPanel, pausePanel, blackBG, blackBGEnd, hintPanel, gameOverPanel, clockObject, bulbObject, notification;
    Animator[] cubeAnimators;
    [NonSerialized] public Animator winPanelAnimator, pausePanelAnimator, hintPanelAnimator, clockObjectAnimator, bulbObjectAnimator;
    WordSelector wordSelectorScript;
    CoinManager coinManagerScript;
    LevelManager levelsManagerScript;
    TextBackground textBackgroundScript;
    Ad_Manager adManagerScript;

    private void Start() 
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        wordSelectorScript = FindObjectOfType<WordSelector>().GetComponent<WordSelector>();
        coinManagerScript = FindObjectOfType<CoinManager>().GetComponent<CoinManager>();
        levelsManagerScript = FindObjectOfType<LevelManager>();
        textBackgroundScript = FindObjectOfType<TextBackground>();
        adManagerScript = FindObjectOfType<Ad_Manager>();
        blackBG = GameObject.FindWithTag("BlackBG");
        blackBGEnd = GameObject.FindWithTag("BlackBGEnd");
        pausePanel = GameObject.FindWithTag("PausePanel");
        winPanel = GameObject.FindWithTag("WinPanel");
        hintPanel = GameObject.FindWithTag("HintPanel");
        gameOverPanel = GameObject.FindWithTag("GameOverPanel");
        timerAnimator = GameObject.FindWithTag("Timer").GetComponent<Animator>();
        clockObject = GameObject.Find("Clock");
        bulbObject = GameObject.Find("Hint button");
        notification = GameObject.Find("Notification");

        winPanelAnimator = winPanel.GetComponent<Animator>();
        pausePanelAnimator = pausePanel.GetComponent<Animator>();
        hintPanelAnimator = hintPanel.GetComponent<Animator>();
        clockObjectAnimator = clockObject.GetComponent<Animator>();
        bulbObjectAnimator = bulbObject.GetComponent<Animator>();

        blackBG.SetActive(false);
        blackBGEnd.SetActive(false);
        winPanel.SetActive(false);
        pausePanel.SetActive(false);
        hintPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        FindAllCubeAnimators();

        rewardStarsBool = false;

        RotateCubes();
        StartCoroutine(AfterRandomTime());

       // StartCoroutine(adManagerScript.InterstitialAdShow());
        StartCoroutine(ShowBannerAd());

    }

    private void Update() 
    {
        if (coinManagerScript.TotalSavedCoins >= 250)
            notification.SetActive(true);
        else{
            notification.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public IEnumerator ShowBannerAd()
    {
        float randomDelay = UnityEngine.Random.Range(4f, 10f);
        yield return new WaitForSeconds(randomDelay);

        adManagerScript.BannerAdLoad();
    }

    public IEnumerator ShowInterstitialAd()
    {
        float randomDelay = UnityEngine.Random.Range(4f, 10f);
        yield return new WaitForSeconds(randomDelay);

        adManagerScript.InterstitialAdShow(1f);
    }

    private void FindAllCubeAnimators()
    {
        CubeController[] cubeControllers = FindObjectsOfType<CubeController>();
        cubeAnimators = new Animator[cubeControllers.Length];

        for (int i = 0; i < cubeControllers.Length; i++)
        {
            cubeAnimators[i] = cubeControllers[i].transform.parent.GetComponent<Animator>();
        }
    }

    public void RotateCubes()
    {
        if (! LetsRotate) return;
        foreach (var animator in cubeAnimators)
        {
            animator.SetTrigger("Rotate");
        }
        Invoke("TimeerAnimation", 3f);
    }

    public void TimeerAnimation()
    {
        if (! timerAnimator) return;
        timerAnimator.SetTrigger("Timer");
    }

    public void ShowPausePanel()
    {
        coinManagerScript.savedCoinsTextInPausedPanel.text = coinManagerScript.TotalSavedCoins.ToString();
        if (pausePanel.activeInHierarchy) return;
        pausePanel.SetActive(true);
        blackBG.SetActive(true);
        wordSelectorScript.playGameAllow = false;
        stopTimer = true;

        textBackgroundScript.AdjustTextSizeInPausePanel();

        StartCoroutine(adManagerScript.RewardedAdShow(0.4f));
    }

    public IEnumerator ShowWinPanel(float waitDelay)
    {
        coinManagerScript.finalCoinsTextInWinPanel.text = coinManagerScript.TotalSavedCoins.ToString();
        yield return new WaitForSeconds(waitDelay);
        winPanel.SetActive(true);
        blackBGEnd.SetActive(true);
        wordSelectorScript.playGameAllow = false;
        stopTimer = true;
        levelsManagerScript.UnloackingNextLevel();

        wordSelectorScript.GetJustCompletedLevel();
        Debug.Log(wordSelectorScript.justCompletedLevel);
        rewardStarsBool = true;

        yield return new WaitForSeconds(1f);
        coinManagerScript.MeasuringLevelTime();
        StartCoroutine(adManagerScript.RewardedAdShow(0.001f));
    }

    public IEnumerator ShowGameOverPanel()
    {
        coinManagerScript.savedCoinsTextInGameOverPanel.text = coinManagerScript.TotalSavedCoins.ToString();
        gameOverPanel.SetActive(true);
        blackBG.SetActive(true);
        wordSelectorScript.playGameAllow = false;
        stopTimer = true;

        textBackgroundScript.AdjustTextSizeInGameOverPanel();
        yield return new WaitForSeconds(1f);

        StartCoroutine(adManagerScript.RewardedAdShow(0.001f));
    }

    public void OnHintButtonClicked()
    {
        if (coinManagerScript.TotalSavedCoins >= 250)
        {
            hintPanel.SetActive(true);
            blackBG.SetActive(true);
            wordSelectorScript.playGameAllow = false;
            stopTimer = true;

            bulbObjectAnimator.SetBool("BulbScale", false);
            wordSelectorScript.wrongAttamtsInt = 0;
        }else
        {
            coinManagerScript.ShowWarningHintLayer();
        }
        coinManagerScript.savedCoinsTextInHintPanel.text = coinManagerScript.TotalSavedCoins.ToString();
        textBackgroundScript.AdjustTextSizeInHintPanel();
    }

    public void OnGetButtonClicked()
    {
        coinManagerScript.TotalSavedCoins -= 250;
        hintPanelAnimator.SetBool("GoBack", true);
        blackBG.SetActive(false);
        StartCoroutine(GotHint(0.4f));
    }

    public IEnumerator GotHint(float delay)
    {
        yield return new WaitForSeconds(delay);
        coinManagerScript.ShowLostHintCoins();
        StartCoroutine(wordSelectorScript.ShowHint());

        wordSelectorScript.playGameAllow = true;
        stopTimer = false;
        StartCoroutine(HidePanels(0.4f));
    }

    public void ReloadLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void LoadScene(string SceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneName);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pausePanelAnimator.SetBool("GoBack", true);
        hintPanelAnimator.SetBool("GoBack", true);
        blackBG.SetActive(false);
        wordSelectorScript.playGameAllow = true;
        stopTimer = false;

        StartCoroutine(HidePanels(0.4f));
    }

    IEnumerator HidePanels(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        pausePanel.SetActive(false);
        winPanel.SetActive(false);
        hintPanel.SetActive(false);
    }

    public IEnumerator BulbAnimationStart()
    {
        if (wordSelectorScript.wrongAttamtsInt >= 3 && coinManagerScript.TotalSavedCoins >= 250)
        {
            if (!stopTimer)
            {
                wordSelectorScript.wrongAttamtsInt = 0;
                bulbObjectAnimator.SetBool("BulbScale", true);
                yield return new WaitForSeconds(5f);
                bulbObjectAnimator.SetBool("BulbScale", false);
                wordSelectorScript.wrongAttamtsInt = 0;
            }
            
        }
    }

    public IEnumerator AfterRandomTime()
    {
        while (true)
        {
            if (!stopTimer)
            {
                float randomDelay = UnityEngine.Random.Range(3f, 10f);
                yield return new WaitForSeconds(randomDelay);

                clockObjectAnimator.SetBool("ClockShake", true);
                yield return new WaitForSeconds(1f);
                clockObjectAnimator.SetBool("ClockShake", false);
            }else
            {
                yield return new WaitForSeconds(1f);
            }

        }
    }
    
}
