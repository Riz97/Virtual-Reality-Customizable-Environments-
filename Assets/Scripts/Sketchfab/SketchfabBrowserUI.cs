using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SketchfabBrowserUI : MonoBehaviour
{
    public SketchfabAPI sketchfabAPI;

    public TMP_InputField searchInputField;
    public Button searchButton;
    public Transform resultsContainer;
    public GameObject resultPrefab; // Un prefab per visualizzare i modelli trovati

    private void Start()
    {
        searchButton.onClick.AddListener(() =>
        {
            string query = searchInputField.text;
            if (!string.IsNullOrEmpty(query))
                SearchModels(query);
        });
    }

    private void SearchModels(string query)
    {
        sketchfabAPI.SearchModels(query, OnSearchResults);
    }

    public void OnSearchResults(List<SketchfabModel> models)
    {
        // Pulisci i risultati precedenti
        foreach (Transform child in resultsContainer)
        {
            Destroy(child.gameObject);
        }

        // Visualizza solo modelli scaricabili
        foreach (var model in models)
        {
            GameObject resultObject = Instantiate(resultPrefab, resultsContainer);
            resultObject.GetComponentInChildren<Text>().text = model.name;

            Button downloadButton = resultObject.GetComponentInChildren<Button>();
            downloadButton.onClick.AddListener(() =>
            {
                // Ottieni il link di download per il modello
                sketchfabAPI.GetDownloadLink(model.uid, (downloadUrl) =>
                {
                    // Scarica il modello
                    string savePath = "Assets/DownloadedModels/" + model.name + ".zip";  // Percorso di salvataggio
                    sketchfabAPI.DownloadModelFile(downloadUrl, savePath);
                });
            });
        }
    }
}