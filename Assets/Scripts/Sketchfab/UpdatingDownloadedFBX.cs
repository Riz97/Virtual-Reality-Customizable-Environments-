using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UpdatingDownloadedFBX : MonoBehaviour
{
    // Path della cartella da monitorare
    public string folderPath = "C:\\Users\\ricky\\Desktop\\Framework\\Virtual-Reality-Customizable-Environments-\\Assets\\ImportedFBX";

    // Path del file di testo dove salvare i nomi degli oggetti
    public string logFilePath = "C:\\Users\\ricky\\Desktop\\folderContents.txt";

    // Intervallo di aggiornamento in secondi
    public float updateInterval = 5.0f;

    private float timer;

    void Start()
    {
        // Verifica che la cartella esista
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError($"La cartella '{folderPath}' non esiste!");
            return;
        }

        // Crea il file di log se non esiste
        if (!File.Exists(logFilePath))
        {
            File.Create(logFilePath).Dispose(); // Dispose rilascia immediatamente le risorse
        }

        // Inizializza il timer
        timer = updateInterval;

        // Scrivi i nomi iniziali degli oggetti
        UpdateFileLog();
    }

    void Update()
    {
        // Aggiorna il timer
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            // Aggiorna il file di log
            UpdateFileLog();

            // Resetta il timer
            timer = updateInterval;
        }
    }

    void UpdateFileLog()
    {
        // Ottieni i nomi di tutti i file nella cartella
        string[] files = Directory.GetFiles(folderPath);

        // Scrivi i nomi dei file nel log
        using (StreamWriter writer = new StreamWriter(logFilePath, false)) // False per sovrascrivere
        {
            foreach (string file in files)
            {
                 if (!file.Contains(".meta"))
                {
                    writer.WriteLine(Path.GetFileName(file.Replace(".fbx","")));
                }
                
               
            }
        }
    }
}