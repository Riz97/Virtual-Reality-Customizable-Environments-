
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;

public class SketchfabAPI : MonoBehaviour
{
    private const string ApiBaseUrl = "https://api.sketchfab.com/v3";
    private string apiToken = "XUnZY9S8XJNI0bGx3J2VtHjRY6qpkesAaT5k9CqYAAyv6k6OOxs0Rb6pSM1POqy8XxbrlHTvfCwdQ0GzfjzUD4Y9NM7AIQjCRXKprUHb3Iv0k1hx1xQT3CPSu2MbSbGr";  // Inserisci qui il tuo API Token

    // Esegui la ricerca per modelli
    public void SearchModels(string query, Action<List<SketchfabModel>> callback)
    {
        string url = $"{ApiBaseUrl}/search?type=models&q={query}";
        StartCoroutine(SendRequest(url, callback));
    }

    // Invia la richiesta HTTP per ottenere i risultati della ricerca
    private IEnumerator SendRequest(string url, Action<List<SketchfabModel>> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", $" {apiToken}");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Parsa la risposta JSON e filtra i modelli
            string responseJson = request.downloadHandler.text;
            SketchfabSearchResponse response = JsonUtility.FromJson<SketchfabSearchResponse>(responseJson);

            // Filtra i modelli con isDownloadable == true
            List<SketchfabModel> downloadableModels = new List<SketchfabModel>();
            foreach (var result in response.results)
            {
                if (result.isDownloadable)
                {
                    
                    downloadableModels.Add(result);
                    Debug.Log(downloadableModels.ToString());

                }
            }

            callback?.Invoke(downloadableModels);
        }
        else
        {
            Debug.LogError($"Errore API: {request.error}");
        }
    }

    // Recupera il link di download per un modello
    public void GetDownloadLink(string modelUid, Action<string> callback)
    {
        
        string url = $"{ApiBaseUrl}/models/{modelUid}/download";
        Debug.Log(url);
       
    }

    // Invia la richiesta HTTP per ottenere il link di download
    private IEnumerator SendDownloadRequest(string url, Action<string> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", $"Bearer {apiToken}");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Parsa la risposta JSON per ottenere il link di download
            string responseJson = request.downloadHandler.text;
            SketchfabDownloadResponse response = JsonUtility.FromJson<SketchfabDownloadResponse>(responseJson);
            string downloadUrl = response.url;
            callback?.Invoke(downloadUrl);  // Invoca il callback con il link di download
        }
        else
        {
            Debug.LogError($"Errore nel recupero del link di download: {request.error}");
        }

    }
    public void DownloadModelFile(string downloadUrl, string savePath)
    {
        StartCoroutine(DownloadFile(downloadUrl, savePath));
    }

    private IEnumerator DownloadFile(string url, string savePath)
    {
        UnityWebRequest downloadRequest = UnityWebRequest.Get(url);
        yield return downloadRequest.SendWebRequest();

        if (downloadRequest.result == UnityWebRequest.Result.Success)
        {
            System.IO.File.WriteAllBytes(savePath, downloadRequest.downloadHandler.data);
            Debug.Log("Modello scaricato con successo!");
            // Qui puoi gestire l'estrazione del file ZIP o altre operazioni
        }
        else
        {
            Debug.LogError($"Errore durante il download del modello: {downloadRequest.error}");
        }
    }
}



[Serializable]
public class SketchfabSearchResponse
{
    public List<SketchfabModel> results;
}

[Serializable]
public class SketchfabModel
{
    public string name;
    public string uid;
    public bool isDownloadable;  // Aggiungi il campo per isDownloadable
}

[Serializable]
public class SketchfabDownloadResponse
{
    public string url;  // URL del file di download
}