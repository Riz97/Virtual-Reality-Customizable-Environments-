using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Collections;

public class SketchfabBrowser : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField keywordInput; // Campo per inserire la keyword
    public Button searchButton;    // Bottone per avviare la ricerca
    public Transform contentParent; // Il Content della ScrollView
    public GameObject modelItemPrefab; // Prefab dell'elemento del modello (Text + Button)

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

            // Crea un nuovo item per il modello
            GameObject modelItem = Instantiate(modelItemPrefab, contentParent);

            // Trova gli elementi di testo e bottone nel prefab
            TMP_Text nameText = modelItem.transform.Find("ModelName").GetComponent<TMP_Text>();
            TMP_Text authorText = modelItem.transform.Find("ModelAuthor").GetComponent<TMP_Text>();
            Button openButton = modelItem.transform.Find("OpenButton").GetComponent<Button>();

            // Imposta il testo del modello
            nameText.text = modelName;
            authorText.text =  modelAuthor;
            // Aggiungi il comportamento al bottone
            openButton.onClick.AddListener(() => OpenModelUrl(modelUrl));
        }
    }
    private void OpenModelUrl(string url)
    {
        Debug.Log(url); // Apri l'URL nel browser
    }
}