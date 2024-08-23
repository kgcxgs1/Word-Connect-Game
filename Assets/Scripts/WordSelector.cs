using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class WordSelector : MonoBehaviour
{
    public float TotallevelTime, timeRemaining, duration, cubeOutsideMoveDistance;
    public TextMeshProUGUI timerText, ValidWordsCountDisplay;
    public TextMeshProUGUI[] wordDisplayTexts; // Array of TextMeshProUGUI objects for current word display

    public List<string> validWords;
    private HashSet<string> processedWords = new HashSet<string>(); // Track processed words
    private Camera mainCamera;

    private List<GameObject> selectedCubes = new List<GameObject>();
    private HashSet<GameObject> foundWordCubes = new HashSet<GameObject>();
    private HashSet<GameObject> AgainSelectGreenCube = new HashSet<GameObject>();
    private HashSet<GameObject> hintExposedCubes = new HashSet<GameObject>();
    private Vector2Int direction;
    public bool directionLocked = false, playGameAllow;
    private int currentTextIndex = 0; // Index to track the current TextMeshProUGUI being used
    [NonSerialized] public int justCompletedLevel, wrongAttamtsInt;

    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();

    GameEvents gameEventScript;
    CoinManager coinManagerScript;

    void Start()
    {
        playGameAllow = false;
        mainCamera = Camera.main;
        timeRemaining = TotallevelTime;
        currentTextIndex = 0;
        wrongAttamtsInt = 0;
        gameEventScript = FindObjectOfType<GameEvents>();
        coinManagerScript = FindObjectOfType<CoinManager>();

        // Initialize all word display texts
        foreach (var text in wordDisplayTexts)
        {
            text.text = "";
            text.gameObject.SetActive(false);
        }

        if (wordDisplayTexts.Length > 0)
        {
            wordDisplayTexts[currentTextIndex].gameObject.SetActive(true);
        }

        ValidWordsCountDisplay.text = processedWords.Count.ToString() + "/" + validWords.Count.ToString();

        Invoke("PlayGameAllow", 3f);
        UpdateTimer();
    }

    void Update()
    {
        Invoke("UpdateTimer", 3f);

        if (!playGameAllow) return;
        HandleMouseInput();
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectedCubes.Clear();
            directionLocked = false;
            wordDisplayTexts[currentTextIndex].text = "";
        }

        if (Input.GetMouseButton(0))
            ProcessCubeSelection();

        if (Input.GetMouseButtonUp(0))
            FinalizeSelection();
    }

    void ProcessCubeSelection()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.TryGetComponent<CubeController>(out var cubeController) && !selectedCubes.Contains(hitObject))
            {
                if (selectedCubes.Count > 0)
                {
                    if (directionLocked)
                    {
                        Vector2Int currentDirection = GetDirection(selectedCubes[selectedCubes.Count - 1], hitObject);
                        if (currentDirection != direction)
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (selectedCubes.Count == 1)
                        {
                            direction = GetDirection(selectedCubes[0], hitObject);
                            directionLocked = true;
                        }
                    }
                }
                selectedCubes.Add(hitObject);
                HighlightCube(hitObject, Color.yellow);
                
                if (!originalPositions.ContainsKey(hitObject))
                {
                    originalPositions[hitObject] = hitObject.transform.position;
                }

                StartCoroutine(MoveCube(hitObject, hitObject.transform.position, hitObject.transform.position + new Vector3(0, 0, cubeOutsideMoveDistance), duration));

                // Update the current word text
                wordDisplayTexts[currentTextIndex].text += hitObject.GetComponent<CubeController>().GetFrontLetter();
            }
        }
    }

    void FinalizeSelection()
    {
        string word = FormWordFromSelection();
        CheckWord(word);

        ResetCubeColors(); // Pass the word to ResetCubeColors
        selectedCubes.Clear();
    }

    string FormWordFromSelection()
    {
        string word = "";
        foreach (var cube in selectedCubes)
        {
            word += cube.GetComponent<CubeController>().GetFrontLetter();
        }
        return word;
    }

    void CheckWord(string word)
    {
        if (validWords.Contains(word) && !processedWords.Contains(word))
        {
            Debug.Log("Valid Word: " + word);
            foreach (var cube in selectedCubes)
            {
                AgainSelectGreenCube.Remove(cube);
                HighlightCube(cube, Color.green);
                foundWordCubes.Add(cube);
            }

            // Make the current word display text static and move it
            StartCoroutine(MakeTextStaticAndMove(word));

            processedWords.Add(word);

            if (processedWords.Count == validWords.Count)
            {
                playGameAllow = false;
                StartCoroutine(gameEventScript.ShowWinPanel(2f));
            }

            ValidWordsCountDisplay.text = processedWords.Count.ToString() + "/" + validWords.Count.ToString();
            coinManagerScript.SpawnCoinsOnCorrectCubes(selectedCubes);
            gameEventScript.bulbObjectAnimator.SetBool("BulbScale", false);
        }
        else
        {
            Debug.Log("Invalid Word: " + word);
            TextMeshProUGUI currentText = wordDisplayTexts[currentTextIndex];
            TextBackground textBackground = currentText.GetComponent<TextBackground>();
            textBackground.animatorOfParent.SetTrigger("Vibrate");
            wrongAttamtsInt++;
            StartCoroutine(gameEventScript.BulbAnimationStart());
        }
    }

    IEnumerator MakeTextStaticAndMove(string word)
    {
        TextMeshProUGUI currentText = wordDisplayTexts[currentTextIndex];
        TextBackground textBackground = currentText.GetComponent<TextBackground>();

        yield return new WaitForSeconds(0.2f);

        // Set the correct word text and keep it active
        currentText.text = word;

        // Activate the next text for new word display
        currentTextIndex = (currentTextIndex + 1) % wordDisplayTexts.Length;
        wordDisplayTexts[currentTextIndex].gameObject.SetActive(true);

        // Move the current text smoothly to the specified position
        textBackground.MoveText();
    }

    void ResetCubeColors()
    {
        foreach (var cube in selectedCubes)
        {
            if (originalPositions.ContainsKey(cube))
                StartCoroutine(MoveCube(cube, cube.transform.position, originalPositions[cube], duration));

            if (foundWordCubes.Contains(cube) && !AgainSelectGreenCube.Contains(cube))
            {
                HighlightCube(cube, Color.green);
                AgainSelectGreenCube.Add(cube);
            }
            else
            {
                HighlightCube(cube, Color.red);
                StartCoroutine(DefaultColorChange(cube, 0.4f));
            }
        }
    }

    IEnumerator DefaultColorChange(GameObject cube, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (AgainSelectGreenCube.Contains(cube))
        {
            HighlightCube(cube, Color.green);
        }else
        {
            if (hintExposedCubes.Contains(cube))
            {
                HighlightCube(cube, Color.cyan);
            }else
            {
                HighlightCube(cube, Color.white);
            }
        }

        wordDisplayTexts[currentTextIndex].text = "";
    }

    IEnumerator MoveCube(GameObject cube, Vector3 startPosition, Vector3 endPosition, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            cube.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cube.transform.position = endPosition;
    }

    void HighlightCube(GameObject cube, Color color)
    {
        cube.GetComponent<Renderer>().material.color = color;
    }

    public IEnumerator ShowHint() // running in GameManegerScript
    {
        foreach (string word in validWords)
        {
            if (!processedWords.Contains(word))
            {
                List<GameObject> hintCubes = GetCubesForWord(word);

                if (hintCubes.Count == word.Length)
                {
                    foreach (var cube in hintCubes)
                    {
                        HighlightCube(cube, Color.cyan); // Highlight cubes in cyan for the hint
                        yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds between highlights
                        hintExposedCubes.Add(cube);
                    }
                    break;
                }
            }
        }
    }

List<GameObject> GetCubesForWord(string word)
{
    List<GameObject> cubesForWord = new List<GameObject>();
    GameObject[] allCubes = GameObject.FindGameObjectsWithTag("Correct");

    // Try to find the starting cube and direction
    foreach (GameObject startCube in allCubes)
    {
        CubeController startCubeController = startCube.GetComponent<CubeController>();
        if (startCubeController != null && startCubeController.GetFrontLetter() == word[0])
        {
            // Try all possible directions from the start cube
            foreach (Vector2Int direction in GetPossibleDirections())
            {
                List<GameObject> candidateCubes = new List<GameObject> { startCube };
                bool valid = true;

                for (int i = 1; i < word.Length; i++)
                {
                    Vector3 nextPosition = startCube.transform.position + new Vector3(direction.x * i, direction.y * i, 0);
                    GameObject nextCube = FindCubeAtPosition(nextPosition, allCubes);

                    if (nextCube == null || nextCube.GetComponent<CubeController>().GetFrontLetter() != word[i] || candidateCubes.Contains(nextCube))
                    {
                        valid = false;
                        break;
                    }

                    candidateCubes.Add(nextCube);
                }

                if (valid)
                {
                    cubesForWord = candidateCubes;
                    return cubesForWord;
                }
            }
        }
    }

    return cubesForWord;
}

Vector2Int[] GetPossibleDirections()
{
    return new Vector2Int[]
    {
        new Vector2Int(1, 0),   // right
        new Vector2Int(-1, 0),  // left
        new Vector2Int(0, 1),   // up
        new Vector2Int(0, -1),  // down
        new Vector2Int(1, 1),   // diagonal right up
        new Vector2Int(-1, -1), // diagonal left down
        new Vector2Int(1, -1),  // diagonal right down
        new Vector2Int(-1, 1)   // diagonal left up
    };
}

GameObject FindCubeAtPosition(Vector3 position, GameObject[] allCubes)
{
    foreach (GameObject cube in allCubes)
    {
        if (Vector3.Distance(cube.transform.position, position) < 0.1f)
        {
            return cube;
        }
    }
    return null;
}



    Vector2Int GetDirection(GameObject firstCube, GameObject secondCube)
    {
        Vector3 pos1 = firstCube.transform.position;
        Vector3 pos2 = secondCube.transform.position;

        int xDir = Mathf.RoundToInt(pos2.x - pos1.x);
        int yDir = Mathf.RoundToInt(pos2.y - pos1.y);

        if (Mathf.Abs(xDir) > Mathf.Abs(yDir))
        {
            yDir = 0;
            xDir = (xDir > 0) ? 1 : -1;
        }
        else if (Mathf.Abs(xDir) < Mathf.Abs(yDir))
        {
            xDir = 0;
            yDir = (yDir > 0) ? 1 : -1;
        }
        else
        {
            xDir = (xDir > 0) ? 1 : -1;
            yDir = (yDir > 0) ? 1 : -1;
        }

        return new Vector2Int(xDir, yDir);
    }

    void UpdateTimer()
    {
        if (gameEventScript.stopTimer) return;
        
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            timeRemaining = 0;
           // HighlightAllCorrectCubes();
            StartCoroutine(gameEventScript.ShowGameOverPanel());
        }

        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void HighlightAllCorrectCubes()
    {
        GameObject[] correctCubes = GameObject.FindGameObjectsWithTag("Correct");
        foreach (GameObject cube in correctCubes)
        {
            HighlightCube(cube, Color.green);
        }
        playGameAllow = false;
    }

    public void PlayGameAllow()
    {
        playGameAllow = true;
    }

    public void GetJustCompletedLevel()
    {
        justCompletedLevel = SceneManager.GetActiveScene().buildIndex - 1 ;
        PlayerPrefs.SetInt("JustCompletedLevel", justCompletedLevel);
        PlayerPrefs.Save();

        string sceneTimeKey = "TotallevelTime_" + SceneManager.GetActiveScene().name;
        PlayerPrefs.SetFloat(sceneTimeKey, TotallevelTime);
        PlayerPrefs.SetString("LastLoadedScene", SceneManager.GetActiveScene().name);
        Debug.Log(TotallevelTime);

        PlayerPrefs.SetFloat("RemainingTime", timeRemaining);
        PlayerPrefs.Save();
    }
}



