using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public List<Button> levelButtons; 
    public UnityEvent onLevelLocked;

    private int currentLevelIndex;

    void Start()
    {
        InitializeLevels();
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
    }

    void InitializeLevels()
    {
        int levelReached = PlayerPrefs.GetInt("LevelReached", 1); // Default to the first level if no progress is saved

        for (int i = 0; i < levelButtons.Count; i++)
        {
            int levelIndex = i + 1; // Level index starts from 1

            // Set button image based on level status
            Image buttonImage = levelButtons[i].GetComponent<Image>();
            levelButtons[i].onClick.AddListener(() => OnLevelButtonClicked(levelIndex));
        }
    }

    void OnLevelButtonClicked(int levelIndex)
    {
        int levelReached = PlayerPrefs.GetInt("LevelReached", 1);

        if (levelIndex <= levelReached)
        {
            currentLevelIndex = levelIndex; // Set the current level index
            LoadScene(); // Load the scene for the selected level
        }
        else
        {
            onLevelLocked?.Invoke(); // Invoke the event if the level is locked
        }
    }

    public void LoadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(currentLevelIndex); // Use the stored currentLevelIndex
    }

    public void UnloackingNextLevel()
    {
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        int levelReached = PlayerPrefs.GetInt("LevelReached", 1);
        
        // Check if the current level is the highest reached
        if (currentLevelIndex == levelReached)
        {
            PlayerPrefs.SetInt("LevelReached", levelReached + 1);
            PlayerPrefs.Save(); // Ensure changes are saved
            Debug.Log("Next level unlocked!");
        }
        else
        {
            Debug.Log("Not unlocking the next level");
        }
    }
}
