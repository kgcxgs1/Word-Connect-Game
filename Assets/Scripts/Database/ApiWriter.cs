using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ApiWriter : MonoBehaviour
{
    private string apiUrl = "http://localhost:5225/api/users"; // Update with your API URL

    public string name;
    public string surname;
    public int age;


    public void OnSubmitButtonClicked()
    {
        Debug.Log($"Submitting data: Name={name}, Surname={surname}, Age={age}");
        PostUser(name, surname, age);
    }

    // Call this method to send data
    public void PostUser(string name, string surname, int age)
    {
        Debug.Log("Starting coroutine to send data...");
        StartCoroutine(SendData(name, surname, age));
    }

    private IEnumerator SendData(string name, string surname, int age)
    {
        // Create the JSON data
        string json = JsonUtility.ToJson(new User { Name = name, Surname = surname, Age = age });
        Debug.Log($"JSON data: {json}");

        // Create the request
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("Sending POST request to API...");

        // Send the request and wait for a response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Data successfully sent. Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error sending data: " + request.error);
            Debug.LogError("Response Code: " + request.responseCode);
        }
    }

    // Define a class to match the API model
    [System.Serializable]
    public class User
    {
        public string Name;
        public string Surname;
        public int Age;
    }
}
