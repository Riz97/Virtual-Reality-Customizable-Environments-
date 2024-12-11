using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using UnityEditor.PackageManager;
using System.Threading.Tasks;
using System.Text;
using UnityEditor;
using System;
using Debug = UnityEngine.Debug;
using UnityEditor.PackageManager.Requests;

public class SketchfabBrowser : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private string SketchfabPy = "/k python C:\\Users\\ricky\\Desktop\\Framework\\Virtual-Reality-Customizable-Environments-\\PythonServer\\SketchfabServer\\SketchfabDownloader.py";
    string path = "C:\\Users\\ricky\\Desktop\\Framework\\Virtual-Reality-Customizable-Environments-\\Assets\\Imported";
    public static string ImagePreview;
    public bool preserveAspect = true; // Mantieni le proporzioni dell'immagine

    [Header("UI Elements")]
    public TMP_InputField keywordInput; 
    public Button searchButton;    
    public Transform contentParent; // Scrollview Content
    public GameObject modelItemPrefab; // Prefab , 2 Texts and 1 button
    public TMP_Text UpdatingText;

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

    //Web Request for obtaining the JSON of the searched model list
    private IEnumerator FetchModels(string keyword)
    {
        string url = $"{apiUrl}&q={UnityWebRequest.EscapeURL(keyword)}&downloadable=true";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", $"Bearer {apiToken}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            DisplayModels(request.downloadHandler.text);
            Debug.ClearDeveloperConsole();
        }
        else
        {
            UnityEngine.Debug.LogError("Errore nella richiesta API: " + request.error);
        }
    }

    private void DisplayModels(string jsonResponse)
    {
        // Pulisce la lista di modelli precedenti
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // Parsing del JSON
        JObject response = JObject.Parse(jsonResponse);
        JArray models = (JArray)response["results"];

        foreach (var model in models)
        {
            // Estrae il nome del modello, autore e URL
            string modelName = model["name"].ToString();
            string modelAuthor = model["user"]["username"].ToString();
            string modelUrl = model["viewerUrl"].ToString();
            string imagePreview = model["thumbnails"]["images"][0]["url"].ToString();

            // Aggiungi "https://" se manca nel link dell'immagine
            if (!imagePreview.StartsWith("http://") && !imagePreview.StartsWith("https://"))
            {
                imagePreview = "https:" + imagePreview; // Aggiungi https:// al link se non presente
            }

            // Crea il prefab che rappresenta un record del modello Sketchfab
            GameObject modelItem = Instantiate(modelItemPrefab, contentParent);

            // Trova tutti i gameobject necessari
            TMP_Text nameText = modelItem.transform.Find("ModelName").GetComponent<TMP_Text>();
            TMP_Text authorText = modelItem.transform.Find("ModelAuthor").GetComponent<TMP_Text>();
            Button openButton = modelItem.transform.Find("OpenButton").GetComponent<Button>();
            RawImage targetRawImage = modelItem.transform.Find("Preview").GetComponent<RawImage>();

            // Assegna il nome preso dal JSON al prefab
            nameText.text = modelName;
            authorText.text = modelAuthor;

         


            // Scarica e applica l'immagine come texture
            StartCoroutine(DownloadAndApplyImage(imagePreview, targetRawImage));

            // Imposta il comportamento del bottone se premuto
            openButton.onClick.AddListener(() => OpenModelUrl(modelName.Replace(" ", "") + " " + modelUrl));
        }
    }

    //TODO - Manage the Download 
    public async Task OpenModelUrl(string message)
    {
        message = message.Replace("https://sketchfab.com/3d-models/none-","");
        byte[] data = Encoding.UTF8.GetBytes(message);
        await stream.WriteAsync(data, 0, data.Length);
    }

    public void SketchfabServerConnection()
    {
        Process.Start("cmd.exe", SketchfabPy);
        client = new TcpClient("127.0.1.10", 12321);
        stream = client.GetStream();

    }
    IEnumerator DownloadAndApplyImage(string url,RawImage targetRawImage)
    {
        // Avvia il download dell'immagine
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            webRequest.disposeDownloadHandlerOnDispose = false;

            // Aspetta il completamento della richiesta
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
            }
            else
            {
                // Ottieni la texture
                Texture2D downloadedTexture = DownloadHandlerTexture.GetContent(webRequest);

                if (downloadedTexture != null)
                {


                    // Assegna la texture alla RawImage
                    if (targetRawImage != null)
                    {
                        targetRawImage.texture = downloadedTexture;

                        // Adatta l'immagine alla forma della RawImage
                        if (preserveAspect)
                        {
                            AdjustAspectRatio(targetRawImage, downloadedTexture);
                        }
                    }
                }

            }
        }
    }
    void AdjustAspectRatio(RawImage rawImage, Texture2D texture)
    {
        // Ottieni il RectTransform della RawImage
        RectTransform rectTransform = rawImage.GetComponent<RectTransform>();

        // Calcola il rapporto tra larghezza e altezza
        float textureAspectRatio = (float)texture.width / texture.height;
        float rawImageAspectRatio = rectTransform.rect.width / rectTransform.rect.height;

        if (textureAspectRatio > rawImageAspectRatio)
        {
            // L'immagine è più larga: ridimensiona l'altezza
            rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, rectTransform.rect.width / textureAspectRatio);
        }
        else
        {
            // L'immagine è più alta: ridimensiona la larghezza
            rectTransform.sizeDelta = new Vector2(rectTransform.rect.height * textureAspectRatio, rectTransform.rect.height);
        }
    }



}

#if UNITY_EDITOR


[InitializeOnLoad]
public static class PlayModeStateHandler
{
    static PlayModeStateHandler()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    //When the Scene is stopped, all the zip folders are deleted
    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {

        //Code executed when the scene is stopped

        if (state == PlayModeStateChange.ExitingPlayMode)
        {
          
            string folderPath = "C:\\Users\\ricky\\Desktop\\Framework\\Virtual-Reality-Customizable-Environments-\\Assets\\Imported";

            string[] zipFiles = Directory.GetFiles(folderPath, "*.zip");

            // Elimina ogni file .zip
            foreach (string zipFile in zipFiles)
            {
                File.Delete(zipFile);
                
            }
        }
    }
}
#endif


