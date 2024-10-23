using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon;
using TMPro;
using UnityEngine.UI;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Runtime;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
public class AmazonBedrockConnection : MonoBehaviour
{

    [SerializeField] private string accessKeyId;
 
    [SerializeField] private string secretAccessKey;

    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private TextMeshProUGUI responseText;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button submitButton;


    [SerializeField] private string userPrompt = "Answer in one sentence please";

    private AmazonBedrockRuntimeClient client;
    private const string ModelId = "eu.meta.llama3-2-1b-instruct-v1:0";
    private static readonly RegionEndpoint RegionEndpoint = RegionEndpoint.EUWest3;

    private void Awake()
    {
        var credentials = new BasicAWSCredentials(accessKeyId, secretAccessKey);
        client = new AmazonBedrockRuntimeClient(credentials,RegionEndpoint);

        submitButton.onClick.AddListener(() => SendPrompt(inputField.text));
    }

    public async void SendPrompt(string prompt)
    {
        promptText.text = $"User: {prompt}";
        var fullPrompt = $"user\n{userPrompt} {prompt}\n\nassistant\n";

        var request = new InvokeModelRequest()
        {
            ModelId = ModelId,
            Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
            {
                prompt = fullPrompt,
                max_gen_len = 512,
                temperature = 0.5

            }))),
            ContentType = "application/json"
        };

    var response = await client.InvokeModelAsync(request);
    var responsBody = await new StreamReader(response.Body).ReadToEndAsync();
    var modelResponse = JObject.Parse(responsBody);

    var assisstantResponse = modelResponse["generation"]?.ToString();
        responseText.text = assisstantResponse;


    }

}
