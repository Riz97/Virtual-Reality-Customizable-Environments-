using UnityEngine;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class NetworkManager : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    public static string message;
    private void Start()
    {
        StartCoroutine(ConnectToServer());

    }

    IEnumerator ConnectToServer()
    {
        client = new TcpClient();
        yield return client.ConnectAsync("127.0.0.1", 65432);
        stream = client.GetStream();

        Debug.Log("Connessione stabilita");

        // Avvia un nuovo thread per la ricezione dei messaggi
        receiveThread = new Thread(ReceiveMessage);
        receiveThread.Start();
    }

    public void ReceiveMessage()
    {
        while (client.Connected)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                     message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    Debug.Log("Messaggio ricevuto: " + message);
                }
            }
            catch (SocketException ex)
            {
                Debug.LogError("Errore di rete: " + ex.Message);
                // Gestisci la disconnessione
                Disconnect();
                break;
            }
        }
    }

    public void sendMessage(string message)
    {
        if (client.Connected)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
        else
        {
            Debug.LogError("Non connesso al server");
        }
    }

    private void Disconnect()
    {
        if (client != null)
        {
            client.Close();
            client = null;
        }
        if (receiveThread != null)
        {
            receiveThread.Abort();
            receiveThread = null;
        }
    }
}