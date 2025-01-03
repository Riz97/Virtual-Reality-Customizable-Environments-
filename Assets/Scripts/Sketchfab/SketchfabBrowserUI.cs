using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;
using UnityEditor;
using Debug = UnityEngine.Debug;


public class SketchfabBrowser : MonoBehaviour
{
    public string message;
    private TcpClient client;
    private NetworkStream stream;
    private string SketchfabPy = "/k python C:\\Users\\ricky\\Desktop\\Framework\\Virtual-Reality-Customizable-Environments-\\PythonServer\\SketchfabServer\\SketchfabDownloader.py";
    public static string ImagePreview;
    public bool preserveAspect = true;
    public bool ServerActivation = false;

    [Header("UI Elements")]
    public TMP_InputField keywordInput; 
    public Button searchButton;    
    public Transform contentParent; // Scrollview Content
    public TMP_Text UpdatingText;

    [Header("Item Prefab")]
    public GameObject modelItemPrefab; // Prefab , 2 Texts and 1 button

    private string apiToken = "a2cba13cba97b522dfba8241b25334cf"; // API Key, can be found in the sketchfab website 
    private string apiUrl = "https://api.sketchfab.com/v3/search?type=models";
    
    private void Start()
    {
        searchButton.onClick.AddListener(SearchModels);
    }

    //Receive the Message from the server and save it in a variable
    public async Task<string> ReceiveMessages()
    {

        byte[] buffer = new byte[102400];
        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

        if (bytesRead == 0)
        {
            // Handle disconnection
            UnityEngine.Debug.Log("Disconnected from server!");
            return "";
        }

        message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        return message;
    }

    public void SearchModels()
    {
        //Added .fbx to the input string in order to get better chanche of displaying fbx 3D objects
        string keyword = keywordInput.text.Trim() + " .fbx";
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
        //Delete all the previous displayed models
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // JSON Parsing
        JObject response = JObject.Parse(jsonResponse);
        JArray models = (JArray)response["results"];

        foreach (var model in models)
        {
            //Save the Model name, the author, the Url and the Preview image url
            string modelName = model["name"].ToString();
            string modelAuthor = model["user"]["username"].ToString();
            string modelUrl = model["viewerUrl"].ToString();
            string imagePreview = model["thumbnails"]["images"][0]["url"].ToString();

           

            //Create the prefab that represent a specific record
            GameObject modelItem = Instantiate(modelItemPrefab, contentParent);

            //Found all the gameobjects attached to the specific prefab
            TMP_Text nameText = modelItem.transform.Find("ModelName").GetComponent<TMP_Text>();
            TMP_Text authorText = modelItem.transform.Find("ModelAuthor").GetComponent<TMP_Text>();
            Button openButton = modelItem.transform.Find("OpenButton").GetComponent<Button>();
            RawImage targetRawImage = modelItem.transform.Find("Preview").GetComponent<RawImage>();

            //Save the Model name and the author from the JSON
            nameText.text = modelName;
            authorText.text = modelAuthor;

            // Download and apply the image to the RawImage
            StartCoroutine(DownloadAndApplyImage(imagePreview, targetRawImage));

            //Download the 3D model when openButton is pressed
            openButton.onClick.AddListener(() => OpenModelUrl(modelName.Replace(" ", "") + " " + modelUrl));
          
        }
    }

    //Download Requester
    public async Task OpenModelUrl(string message)
    {
        string response;

        UpdatingText.alignment = (TextAlignmentOptions)TextAlignment.Left;

        //If the server is not active, a message is printed 

        if (!ServerActivation)
        {
            UpdatingText.text = "Firstly You must activate the Server!!";
        }

        //Otherwise, we say to the user that the download is started
        else
        {

            UpdatingText.text = "Downloading the required 3D model!!";

            //Send to the server the desired 3D object labelled with an Uid
            message = message.Replace("https://sketchfab.com/3d-models/none-", "");
            byte[] data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);
            response = await ReceiveMessages(); //We await for a response from the server, that will say us if the download was ok or not
            
            //Depending on the response received from the server, we communicate to the server whether the download was succesful or not
            if(response == "OK") 
            {
                UpdatingText.text = "Succesfull Download";
            }

            if (response == "NOT OK")
            {
                UpdatingText.text = "The Requested Object cannot be imported";
            }
        }

       

    }

    //Method attched to Sketchfab Server button 
    public void SketchfabServerConnection()
    {
        Process.Start("cmd.exe", SketchfabPy);
        client = new TcpClient("127.0.1.10", 12321);
        stream = client.GetStream();
        ServerActivation = true;

    }

    //Download the image and apply it to the RawImage
    IEnumerator DownloadAndApplyImage(string url,RawImage targetRawImage)
    {
        // Start the downlaod
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            webRequest.disposeDownloadHandlerOnDispose = false;

            // Wait for the completion of the request
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
            }
            else
            {
                // Got the texture
                Texture2D downloadedTexture = DownloadHandlerTexture.GetContent(webRequest);

                if (downloadedTexture != null)
                {


                    // Assign the texture to the RawImage
                    if (targetRawImage != null)
                    {
                        targetRawImage.texture = downloadedTexture;

                        // Adapt the image to the frame 
                        if (preserveAspect)
                        {
                            AdjustAspectRatio(targetRawImage, downloadedTexture);
                        }
                    }
                }

            }
        }
    }
    //Adapt the image to the corresponding frame aspect
    void AdjustAspectRatio(RawImage rawImage, Texture2D texture)
    {
        // Save the RectTransform of the RawImage
        RectTransform rectTransform = rawImage.GetComponent<RectTransform>();

        
        float textureAspectRatio = (float)texture.width / texture.height;
        float rawImageAspectRatio = rectTransform.rect.width / rectTransform.rect.height;

        if (textureAspectRatio > rawImageAspectRatio)
        {
            //Image wider
            rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, rectTransform.rect.width / textureAspectRatio);
        }
        else
        {
            //Image higher
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
        string path = "C:\\Users\\ricky\\Desktop\\Framework\\Virtual-Reality-Customizable-Environments-\\Assets\\Imported";
        string fbxPath = "C:\\Users\\ricky\\Desktop\\Framework\\Virtual-Reality-Customizable-Environments-\\Assets\\Resources\\ImportedFBX";

        string txtPath = "C:\\Users\\ricky\\Desktop\\Framework\\Virtual-Reality-Customizable-Environments-\\Assets\\folderContents.txt";

        string[] subdirectories = Directory.GetDirectories(path);
        string[] fbxFiles = Directory.GetFiles(fbxPath,"*.fbx");
        string[] metafiles = Directory.GetFiles(path,"*.meta");
        //Code executed when the scene is stopped

        if (state == PlayModeStateChange.ExitingPlayMode)
        { 
            // Delete all the subdirectories in Imported
            foreach (string subdirectory in subdirectories)
            {
                Directory.Delete(subdirectory, true);
               
            }
            //Delete also the .meta files otherwise the folder won't be deleted completely
            foreach(string metafile in metafiles)
            {
                File.Delete(metafile);
            }

            foreach(string fbxFile in  fbxFiles)
            {
                File.Delete(fbxFile);
            }

            File.WriteAllText(txtPath,string.Empty);

        }
    }
}
#endif


