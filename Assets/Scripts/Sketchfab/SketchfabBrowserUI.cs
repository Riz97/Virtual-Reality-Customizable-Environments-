using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using TMPro;

public class SketchfabBrowser : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField keywordInput; // Campo per inserire la keyword
    public Button searchButton;    // Bottone per avviare la ricerca
    public Transform contentParent; // Genitore degli elementi nella Scroll View

    private string apiToken = "a2cba13cba97b522dfba8241b25334cf"; // Sostituisci con la tua API Key
    private string apiUrl = "https://api.sketchfab.com/v3/search?type=models";

    private void Start()
    {
        searchButton.onClick.AddListener(SearchModels);
    }

    public void SearchModels()
    {
        string keyword = keywordInput.text.Trim();
        if (!string.IsNullOrEmpty(keyword))
        {
            StartCoroutine(FetchModels(keyword));
        }
    }

    private IEnumerator FetchModels(string keyword)
    {
        string url = $"{apiUrl}&q={UnityWebRequest.EscapeURL(keyword)}&downloadable=true";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", $"Bearer {apiToken}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            DisplayModels(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Errore nella richiesta API: " + request.error);
        }
    }

    private void DisplayModels(string jsonResponse)
    {
        // Pulisci la lista precedente
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // Parsifica il JSON
        JObject response = JObject.Parse(jsonResponse);
        JArray models = (JArray)response["results"];

        foreach (var model in models)
        {
            string modelName = model["name"].ToString();
            string modelAuthor = model["user"]["username"].ToString();
            string modelUrl = model["viewerUrl"].ToString();

            // Crea un nuovo oggetto testo per il modello
            GameObject textObj = new GameObject("ModelText");
            textObj.transform.SetParent(contentParent);
            textObj.AddComponent<Text>();

            // Configura il testo
            Text textComponent = textObj.GetComponent<Text>();
            textComponent.text = $"Name: {modelName}\nAuthor: {modelAuthor}\nURL: {modelUrl}";
            textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textComponent.fontSize = 14;
            textComponent.color = Color.black;

            // Imposta il layout
            RectTransform rectTransform = textObj.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(500, 100);
        }
    }
}