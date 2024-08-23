using TMPro;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public TextMeshPro frontText;
    public TextMeshPro backText;
    public TextMeshPro leftText;
    public TextMeshPro rightText;
    public TextMeshPro topText;
    public TextMeshPro bottomText;

    private char[] letters = new char[6];
    private TextMeshPro[] allTexts;

    void Start()
    {
        AssignRandomLetters();
    }

    public void AssignRandomLetters()
    {
        allTexts = GetComponentsInChildren<TextMeshPro>();

        TextMeshPro[] letterTexts = System.Array.FindAll(allTexts, text => text.CompareTag("Letter"));

        for (int i = 0; i < letterTexts.Length; i++)
        {
            letters[i] = (char)('A' + Random.Range(0, 26));
            letterTexts[i].text = letters[i].ToString();
        }
    }

    public char GetFrontLetter()
    {
        return frontText.text[0];
    }
}
