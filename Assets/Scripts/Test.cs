using UnityEngine;
using System.Net.Sockets;
using System.Text;

public class UnityClient : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;

    public void SendMessageToServer(string message)
    {
        // Assumiamo che il server stia ascoltando sulla porta 12345 e sull'indirizzo "localhost"
        client = new TcpClient("localhost", 12345);
        stream = client.GetStream();

        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);

        // Ricezione della risposta (semplificata)
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);


        Debug.Log("Risposta dal server: " + response);

        // Chiusura della connessione
        stream.Close();
        client.Close();
    }
}