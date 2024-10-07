using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Test : MonoBehaviour
{
    [SerializeField] private string apiKey = "AIzaSyANvh8Ovudo65m0wwqKKqdHZ9H4nTag3es"; // Sostituisci con la tua chiave API
    [SerializeField] private string prompt = "Qual è la capitale della Francia?";

    public void SendRequest()
    {
        StartCoroutine(GetRequest());
    }

    IEnumerator GetRequest()
    {
        // Costruisci l'URL della richiesta
        string url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.0-pro:generateContent?key=" + apiKey;

        // Crea la richiesta
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes("{\"prompt\":\"" + prompt + "\"}");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");


        // Invia la richiesta
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Processa la risposta
            string response = request.downloadHandler.text;
            Debug.Log("Risposta da Gemini: " + response);
            // Qui puoi parsare la risposta JSON per estrarre il testo generato
        }
        else
        {
            Debug.LogError("Errore nella richiesta: " + request.error);
        }
    }
}