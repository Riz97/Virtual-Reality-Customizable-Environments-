using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI;

public class GeminiNetworkManager : MonoBehaviour
{ 
    public string message;
    private TcpClient client;
    private NetworkStream stream;
    [SerializeField] Button ServerButton;


    //----------------------------- Python Scripts Location ---------------------------------------------------------------------------------------------

    //Location must be checked in different devices

    private string geminiPy = "/k python C:\\Users\\ricky\\Desktop\\Framework\\Virtual-Reality-Customizable-Environments-\\PythonServer\\gemini.py";

    //----------------------------------------------------------------------------------------------------------------------------------------------------

    public void Start()
    {
  

        // Start receiving messages in a separate task
    }

    //Method that switch on the Gemini Python Server
    public void GeminiServerConnection()
    {
        Process.Start("cmd.exe", geminiPy);
        client = new TcpClient("127.0.1.1", 12346);
        stream = client.GetStream();
        ServerButton.interactable = false;

    }

    //It Contains the answered received from GEMINI
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
           // Debug.Log(message);
            return message;
    }

    public async Task SendMessageToServer(string message)
    {
        
            byte[] data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);
        
    }
}