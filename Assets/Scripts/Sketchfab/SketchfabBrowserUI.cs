using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SketchfabUIManager : MonoBehaviour
{
    public SketchfabAPI sketchfabAPI;  // Riferimento allo script SketchfabAPI
    public TMP_InputField searchInputField;
    public Button searchButton;
    public Transform resultsContainer;
    public GameObject resultPrefab;  // Prefab per ogni modello trovato
    public string savePath = "C:/DownloadedModels/";  // Percorso in cui salvare i modelli

    private void Start()
    {
        searchButton.onClick.AddListener(OnSearchButtonClicked);
    }

    private void OnSearchButtonClicked()
    {
        string query = searchInputField.text;
        if (!string.IsNullOrEmpty(query))
        {
            sketchfabAPI.SearchModels(query, OnSearchResultsReceived);
        }
    }

    private void OnSearchResultsReceived(List<SketchfabModel> models)
    {
        // Pulisci i risultati precedenti
        foreach (Transform child in resultsContainer)
        {
            Destroy(child.gameObject);
        }

        // Mostra i nuovi risultati
        foreach (var model in models)
        {
            GameObject resultObject = Instantiate(resultPrefab, resultsContainer);
            resultObject.GetComponentInChildren<Text>().text = model.name;

            Button downloadButton = resultObject.GetComponentInChildren<Button>();
            downloadButton.onClick.AddListener(() =>
            {
                OnDownloadButtonClicked(model.uid);
            });
        }
    }

    private void OnDownloadButtonClicked(string modelUid)
    {
        sketchfabAPI.GetDownloadLink(modelUid, (downloadUrl) =>
        {
            string fullPath = $"{savePath}{modelUid}.zip";
            sketchfabAPI.DownloadModelFile(downloadUrl, fullPath);
        });
    }
}