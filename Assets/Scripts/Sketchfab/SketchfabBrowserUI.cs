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

public class SketchfabBrowser : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private string SketchfabPy = "/k python C:\\Users\\ricky\\Desktop\\Framework\\Virtual-Reality-Customizable-Environments-\\PythonServer\\SketchfabServer\\SketchfabDownloader.py";


    [Header("UI Elements")]
    public TMP_InputField keywordInput; 
    public Button searchButton;    
    public Transform contentParent; // Scrollview Content
    public GameObject modelItemPrefab; // Prefab , 2 Texts and 1 button

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
        }
        else
        {
            UnityEngine.Debug.LogError("Errore nella richiesta API: " + request.error);
        }
    }

    private void DisplayModels(string jsonResponse)
    {
        
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        //JSON Parsing
        JObject response = JObject.Parse(jsonResponse);
        JArray models = (JArray)response["results"];

        foreach (var model in models)
        {
            //Extract the Model Name, author and URL
            string modelName = model["name"].ToString();
            string modelAuthor = model["user"]["username"].ToString();
            string modelUrl = model["viewerUrl"].ToString();

            //Creates a new prefab that represents one Sketchfab model record
            GameObject modelItem = Instantiate(modelItemPrefab, contentParent);

            //Find all the gameobjects needed
            TMP_Text nameText = modelItem.transform.Find("ModelName").GetComponent<TMP_Text>();
            TMP_Text authorText = modelItem.transform.Find("ModelAuthor").GetComponent<TMP_Text>();
            Button openButton = modelItem.transform.Find("OpenButton").GetComponent<Button>();

            //Assign the name taken from the JSON to the prefab
            nameText.text = modelName;
            authorText.text =  modelAuthor;
            

            //Button behaviour if pressed
            openButton.onClick.AddListener(() => OpenModelUrl(modelUrl));
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
}