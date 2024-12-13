
using System.IO;
using TMPro;
using UnityEngine;

public class UpdatingDownloadedFBX : MonoBehaviour
{
    //Path of the folder to monitor
    public string folderPath = "C:\\Users\\ricky\\Desktop\\Framework\\Virtual-Reality-Customizable-Environments-\\Assets\\ImportedFBX";

    //File where we save the name of the saved files
    public string logFilePath = "C:\\Users\\ricky\\Desktop\\Framework\\Virtual-Reality-Customizable-Environments-\\Assets\\folderContents.txt";

    //Updating Interval
    public float updateInterval = 5.0f;

    private float timer;
    public TMP_Text UpdatingText;
    public TMP_Text text;
    
    void Start()
    {
        //Check if the Folder exists
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError($"La cartella '{folderPath}' non esiste!");
            return;
        }

        //If the Log file does not exist, create it 
        if (!File.Exists(logFilePath))
        {
            File.Create(logFilePath).Dispose(); 
        }

        //Initialize the timer
        timer = updateInterval;

    }

    void Update()
    {

        // Update the Timer
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            //Update File and the ScrollView that contains the name of the downloaded files
            UpdateFileLog();

            UpdateScrollView();

            // Reset the Timer
            timer = updateInterval;
        }


    }

    //Method for Updating the Scroll View responsible for displaying the downloaded models' names
    void UpdateScrollView()
    {
        string txtPath = "C:\\Users\\ricky\\Desktop\\Framework\\Virtual-Reality-Customizable-Environments-\\Assets\\folderContents.txt";


        if (File.Exists(txtPath))
        {
            string txtContent = File.ReadAllText(txtPath);
            text.text = txtContent;
        }
    }

    void UpdateFileLog()
    {
        //Process to get all the files' names inside the folder, discard the .meta files
        string[] files = Directory.GetFiles(folderPath);
        using (StreamWriter writer = new StreamWriter(logFilePath, false)) // False per sovrascrivere
        {
            foreach (string file in files)
            {
                if (!file.Contains(".meta"))
                {
                    
                    writer.WriteLine(Path.GetFileName(file.Replace(".fbx", "")));
                    //If the 3D object name is inserted inside the file the download was successful
                    UpdatingText.text = "Successful Download";
                }


            }
        }
    }
}