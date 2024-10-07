using System.Net.Sockets;
using System.Text;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    int count = 0;
    public void Start()
    {
        client = new TcpClient("127.0.0.1", 1234);
        stream = client.GetStream();

        // Start receiving messages in a separate task
        Task receiveTask = Task.Run(async () => await ReceiveMessages());
    }

       //It Contains the answered received from GEMINI
     public async Task ReceiveMessages()
    {
        
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

            if (bytesRead == 0)
            {
                // Handle disconnection
                Debug.Log("Disconnected from server!");
               
            }

            if(count == 0)
        {
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Debug.Log("Messaggio dal server: " + message);
            count++;
        }
            
        
    }

    public async Task SendMessageToServer(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        await stream.WriteAsync(data, 0, data.Length);
    }
}