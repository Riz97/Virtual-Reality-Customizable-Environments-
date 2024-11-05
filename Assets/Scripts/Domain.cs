
using RoslynCSharp;
using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Reflection;
using JetBrains.Annotations;

public class Domain : MonoBehaviour
{

    [SerializeField] TMP_Text Output_Text;

    [SerializeField] TMP_Text Input_Text;

    [SerializeField] public Button Generate_Script_Button;

    [SerializeField] public TMP_InputField InputField;

    [SerializeField] public  GameObject popup;

   

    //-------------------- SYSTEM MESSAGES----------------------------------------------------------

    private const string Welcome_Message = "static void Main()";
    private const string Error_Message = "Error : The environment you asked is not implemented yet, sorry";
    private const string Wait_Message = "Sorry, the AI was not able to generate a correct script. Wait! The AI is trying to generate another one :)";
    private const string Computing_Message = "Computing the script , just wait!!!!";
    private const string Error = "Error : you have to ask for the exactly amount of models requested  for this simulation";
    
    //------------------------------------------------------------------------------------------------

    private ScriptDomain domain = null;
    int FaultyScriptCount = 0;
    int totaltries;
    private string sourceCode;
    public static bool isExecutable = true;
    static string s_time = System.DateTime.Now.ToString("dd-MM-hh-mm-ss");
    string path = Application.dataPath + "/Logs/" + s_time + ".txt";
    string faultypath = Application.dataPath + "/Logs/Faulty Scripts/Faulty_Scripts.txt";
    private Chat chat = new Chat();
   
    public void Start()
    {
        //Waiter
        if (Output_Text.text.ToString() != Welcome_Message && Output_Text.text.ToString()
           != Error_Message && Output_Text.text.ToString() != Wait_Message && Output_Text.text != "Executing......")
        {
            PrintAI_Thoughts();
        }

    }

    public void DoScript()
    {
        PrintAI_Thoughts();

    }

    //---------------------------------------------- SCRIPT EXECUTOR -----------------------------

    public void PrintAI_Thoughts()
    {
        { StartCoroutine(WaitIA()); }
    }


    IEnumerator WaitIA()
    {
        //In this way we wait 20 seconds only the first time the app is launched
        //, in these  seconds the ai should be able to 
        //provide a correct script that Roslyn will compile at runtime

        yield return new WaitForSeconds(20);

        totaltries = Chat.tries;


        //The system is put in wait, until the script is found and printed in the output text window

        while ((Output_Text.text.ToString() == Wait_Message || Output_Text.text.ToString() == Computing_Message))
        {
            yield return new WaitForSeconds(1);
            Debug.Log("I'm waiting for the executable script");
        }

        if (Output_Text.text != Welcome_Message && Output_Text.text != Error_Message && Output_Text.text.ToString() != Wait_Message && Output_Text.text != "Executing......" && Output_Text.text != Error && Output_Text.text != Computing_Message)
        {
            sourceCode = Output_Text.text.ToString();

            // Create domain
            domain = ScriptDomain.CreateDomain("Example Domain");
            
                // Compile and load code - Note that we use 'CompileAndLoadMainSource' which is the same as 'CompileAndLoadSource' but returns the main type in the compiled assembly
                 ScriptType type = domain.CompileAndLoadMainSource(sourceCode, ScriptSecurityMode.UseSettings);

                // Create an instance of 'Example'
                ScriptProxy proxy = type.CreateInstance(gameObject);

                proxy.SafeCall(sourceCode);
   
          

                isExecutable = true;

   


            //If the user has asked for a Bases Environment we have to set the flag to true , in this way when another environment is asked , the system knows the 
            //exact amount of models to destroy.

            if (Chat.input_auxx.ToLower() == "office" ||
                 Chat.input_auxx.ToLower() == "apartment" ||
                 Chat.input_auxx.ToLower() == "nature" ||
                 Chat.input_auxx.ToLower() == "forest" ||
                 Chat.input_auxx.ToLower() == "grid" ||
                 Chat.input_auxx.ToLower() == "city" ||
                 Chat.input_auxx.ToLower() == "industry")

            {

                Chat.Bases = true;

            }

            else
            {
                Chat.Custom = true;

            }
            //Script executed , the button is now interactable and the correct Log File can be created
                Generate_Script_Button.interactable = true;
           
            if(isExecutable)
            {
                CreateLogFile(sourceCode, Input_Text,FaultyScriptCount);
            }

            yield return new WaitForSeconds(2);



            //------------------------------------------------- LOG FILES FUNCTION ------------------------------------------

            void CreateLogFile(string sourcecode, TMP_Text Input_Text,int FaultyScriptCount)
            {

                if (!File.Exists(path))
                {

                    File.WriteAllText(path, "LOG GENERATED FOR THE SESSION" + "\n");

                }

                File.AppendAllText(path, "\nModel - " + Chat.ModelName + "\nNumber of models in the scene ~ "
                + Chat.Number_of_Objects + "\nYou wrote the following  sentence : " +
                Input_Text.text + "\n" + "\n" + "The script generated by the AI is the following: \n " + sourcecode + "\n" +
                "Elapsed time for the generation of the script took " + Chat.elapsed_time + " seconds"
                + "\n" + "The IA required " + Chat.tries + " tries , for obtaining an accetable script \n" + "The number of faulty script for this environemnt is £ " + FaultyScriptCount + "\n");
             
                Chat.tries = 0;
                FaultyScriptCount = 0;
            }
//--------------------------------------------------------------------------------------------------------------
//------------------------------------------------- LOG FAULTY SCRIPTS FILES FUNCTION ------------------------------------------


        }
    }
//-------------------------------------------------------------------------------------------------------------------------------------------

//-------------------------------------- POP UP HANDLER ------------------------------------------------------------------------------------
    IEnumerator showPopup()
    {
        popup.SetActive(true); // Mostra il pop-up
        yield return new WaitForSeconds(5); // Aspetta 5 secondi
        popup.SetActive(false); // Nasconde il pop-up
    }

    private void OnEnable()
    {
        // Sottoscrivi l'evento logMessageReceived quando lo script viene attivato
        Application.logMessageReceived += OnLogMessageReceived;
    }

    private void OnDisable()
    {
        // Annulla la sottoscrizione dell'evento quando lo script viene disattivato
        Application.logMessageReceived -= OnLogMessageReceived;
    }

    private void OnLogMessageReceived(string logString, string stackTrace, LogType type)
    {
      bool errorHandled = false;

     
            // Verifica se il log è un errore o un'eccezione
            if ((type == LogType.Error || type == LogType.Exception) && !errorHandled )
        {

            
            errorHandled = true;

            // Esegui il codice specifico in caso di errore
            EseguiCodiceSuErrore();
        }
    }

    private void EseguiCodiceSuErrore()
    {
        if (Chat.input_auxx.ToLower() == "office" ||
    Chat.input_auxx.ToLower() == "apartment" ||
    Chat.input_auxx.ToLower() == "nature" ||
    Chat.input_auxx.ToLower() == "forest" ||
    Chat.input_auxx.ToLower() == "grid" ||
    Chat.input_auxx.ToLower() == "city" ||
    Chat.input_auxx.ToLower() == "industry")

        {

            Chat.Bases = true;

        }

        else
        {
            Chat.Custom = true;
        }
        //We get access to the ReadStringInput Method inside Chat.cs
        GameObject IA_Manager = GameObject.Find("Ai_Manager");
        chat = IA_Manager.GetComponent<Chat>();

        Debug.Log("The AI generated script contains syntax compilation errors");

        isExecutable = false;

        //Add the Faulty Script to Faulty_Script.txt
        CreateFaultyScriptsFile(sourceCode, Input_Text);

        FaultyScriptCount++;//Increase the number of faulty scripts generated for an environment

        Debug.Log(FaultyScriptCount);

        StartCoroutine(showPopup());//It shows a 5 seconds pop up error

        chat.ReadStringInput(InputField);//Send again the request to the LLM
        DoScript();//Execute the code otherwise wait for an acceptable script
        Generate_Script_Button.interactable = false;

    }
    void CreateFaultyScriptsFile(string sourcecode, TMP_Text Input_Text)
    {
        int c = 0;

        if (!File.Exists(faultypath))
        {
            File.WriteAllText(faultypath, "LIST OF ALL THE FAULTY SCRIPT FOUND DURING THE TESTS " + "\n"
            + "Copy the script into an empty C# script file for checking all the syntax errors");
        }

        //If a faulty script is already inside the file, it is not written in.
        string[] rows = File.ReadAllLines(faultypath);

        bool scriptAlreadyIn = false;
        foreach (string row in rows)
        {
            if (row == sourcecode)
            {
                scriptAlreadyIn = true;
                break;
            }

            if (!scriptAlreadyIn && c == 0)
            {

                File.AppendAllText(faultypath, "\nModel - " + Chat.ModelName + "\nNumber of models in the scene ~ "
                + Chat.Number_of_Objects + "\nYou wrote the following  sentence : " +
                Input_Text.text + "\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                "\n" + "\n" + "The faulty script generated by the AI is the following: \n " + sourcecode + "\n" +
                "Elapsed time for the generation of the script took " + Chat.elapsed_time + " seconds \n"
                + "\n");
                c++;

            }
        }

    }
}
  //-------------------------------------------------------------------------------------------------------------------------------------------
















