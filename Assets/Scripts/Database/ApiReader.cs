using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Collections;

public class ApiReader : MonoBehaviour
{
    public GameObject rowPrefab;
    public Transform tableContainer;

    private string apiUrl = "http://localhost:5225/api/users"; // Your API URL

    public void OnClickedReceiveButton()
    {
        StartCoroutine(FetchDataFromAPI()); // Start fetching data
    }

    IEnumerator FetchDataFromAPI()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest(); // Wait for the request to complete

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text; // Get the response
                Debug.Log("JSON Response: " + jsonResponse); // Log the raw JSON response

                try
                {
                    JArray dataArray = JArray.Parse(jsonResponse); // Parse the JSON response

                    // Clear previous rows
                    foreach (Transform child in tableContainer)
                    {
                        Destroy(child.gameObject);
                    }

                    foreach (var item in dataArray)
                    {
                        // Instantiate a new row from the prefab
                        GameObject row = Instantiate(rowPrefab, tableContainer);

                        // Get TextMeshProUGUI components from the row prefab
                        TextMeshProUGUI idText = row.transform.Find("IDText")?.GetComponent<TextMeshProUGUI>();
                        TextMeshProUGUI nameText = row.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
                        TextMeshProUGUI surnameText = row.transform.Find("SurnameText")?.GetComponent<TextMeshProUGUI>();
                        TextMeshProUGUI ageText = row.transform.Find("AgeText")?.GetComponent<TextMeshProUGUI>();

                        if (idText == null || nameText == null || surnameText == null || ageText == null)
                        {
                            Debug.LogError("One or more TextMeshProUGUI components are missing in the row prefab.");
                            continue;
                        }

                        // Assign the data to the TextMeshProUGUI components
                        idText.text = item["id"]?.ToString() ?? "Unknown ID";
                        nameText.text = item["name"]?.ToString() ?? "Unknown Name";
                        surnameText.text = item["surname"]?.ToString() ?? "Unknown Surname";
                        ageText.text = item["age"]?.ToString() ?? "Unknown Age";
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Failed to parse JSON: " + ex.Message);
                }
            }
            else
            {
                Debug.LogError("Failed to fetch data: " + request.error);
            }
        }
    }
}
