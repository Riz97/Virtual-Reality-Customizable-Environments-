

using RoslynCSharp;
using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Domain : MonoBehaviour
{
    [Header("Fundamental UI Elements")]

    [SerializeField] TMP_Text Output_Text;

    [SerializeField] TMP_Text Input_Text;

    [SerializeField] public Button Generate_Script_Button;

    [SerializeField] public TMP_InputField InputField;

    [SerializeField] TMP_Text NumberPositions_Text;

    [Header("Pop Error Panel")]
    
    [SerializeField] public GameObject popup;

    public static int errorcount = 0;
    int temp = 0;

    //-------------------- SYSTEM MESSAGES----------------------------------------------------------

    private const string Welcome_Message = "static void Main()";
    private const string Error_Message = "Error : The environment you asked is not implemented yet, sorry";
    private const string Wait_Message = "Sorry, the AI was not able to generate a correct script. Wait! The AI is trying to generate another one :)";
    private const string Computing_Message = "Computing the script , just wait!!!!";
    private const string Error = "Error : you have to ask for the exactly amount of models requested  for this simulation";
    private const string Error_Position = "Error: You have to position all the models that you have requested!";

    //------------------------------------------------------------------------------------------------

    private ScriptDomain domain = null;
    int FaultyScriptCount = 0;

    private string sourceCode;
    public static bool isExecutable = false;
    static string s_time = System.DateTime.Now.ToString("dd-MM-hh-mm-ss");
    string path = Application.dataPath + "/Logs/" + s_time + ".txt";
    string faultypath = Application.dataPath + "/Logs/Faulty Scripts/Faulty_Scripts.txt";
    private Chat chat = new Chat();
    
    public void Start()
    {
        //Waiter
        if (Output_Text.text.ToString() != Welcome_Message && Output_Text.text.ToString()
           != Error_Message && Output_Text.text.ToString() != Wait_Message && Output_Text.text != "Executing......" 
           && Output_Text.text.ToString() != Error_Position)
        {
            PrintAI_Thoughts();
        }

    }

    public void DoScript()
    {
        
        PrintAI_Thoughts();

        //Script Executed, Reset of the Number of objects positioned by the user
        Chat.counter = 0;
        NumberPositions_Text.SetText("Number of models is : " + Chat.Number_of_Objects + "(" + (Chat.counter).ToString() + ")");


    }

    //---------------------------------------------- SCRIPT EXECUTOR -----------------------------

    public void PrintAI_Thoughts()
    {
        { StartCoroutine(WaitIA()); }
    }


    IEnumerator WaitIA()
    {
  
        //The system is put in wait, until the script is found and printed in the output text window

        while ((Output_Text.text.ToString() == Wait_Message || Output_Text.text.ToString() == Computing_Message))
        {
            yield return new WaitForSeconds(1);
            //Debug.Log("I'm waiting for the executable script");
        }

        if (Output_Text.text != Error_Position && Output_Text.text != Welcome_Message && Output_Text.text != Error_Message && Output_Text.text.ToString() != Wait_Message && Output_Text.text != "Executing......" && Output_Text.text != Error && Output_Text.text != Computing_Message)
        {
            sourceCode = Output_Text.text.ToString();

        //----------------------------------------------- C# RUNTIME COMPILER ROSLYN USAGE ------------------------------------------------------------------------------------------------------------------------------------------------
            // Create domain
            domain = ScriptDomain.CreateDomain("Example Domain");

            // Compile and load code - Note that we use 'CompileAndLoadMainSource' which is the same as 'CompileAndLoadSource' but returns the main type in the compiled assembly

            ScriptType type = domain.CompileAndLoadMainSource(sourceCode, ScriptSecurityMode.UseSettings);

            // Create an instance of 'Example'
            ScriptProxy proxy = type.CreateInstance(gameObject);

            proxy.SafeCall(sourceCode);

            //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


           isExecutable = true;

          //If the script does not have any kind of errors, the Log File can be refreshed and the button can be interactable again

            if (isExecutable)
            {
               
                CreateLogFile(sourceCode, Input_Text, FaultyScriptCount);
                Generate_Script_Button.interactable = true;
                FaultyScriptCount = 0;
                errorcount = 0;
                Chat.CustomCoordinatesX.Clear();
                Chat.CustomCoordinatesZ.Clear();
                

            }

            //------------------------------------------------- LOG FILES GENEREATION -------------------------------------------------------------------------------------------------------------------------

            void CreateLogFile(string sourcecode, TMP_Text Input_Text, int FaultyScriptCount)
            {
                 temp = Chat.tries + FaultyScriptCount;

                if (!File.Exists(path))
                {
                    File.WriteAllText(path, "LOG GENERATED FOR THE SESSION" + "\n");
                }

                File.AppendAllText(path, "\nModel - " + Chat.ModelName + "\nNumber of models in the scene ~ "
                + Chat.Number_of_Objects + "\nYou wrote the following  sentence : " +
                Input_Text.text + "\n" + "\n" + "The script generated by the AI is the following: \n " + sourcecode + "\n" +
                "Elapsed time for the generation of the script took " + Chat.elapsed_time + " seconds"
                + "\n" + "The IA required " + (temp - FaultyScriptCount) + " tries , for obtaining an accetable script \n" + "The number of faulty script for this environment were " + FaultyScriptCount + "\n");
                Chat.tries = 0;
                FaultyScriptCount = 0;
            }
            
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
   
    //-------------------------------------------------------------------- FAULTY SCRIPTS HANDLER --------------------------------------------------------------------------------------------------

    //It deletes the faulty script inserted in the Log  
    private void DeleteFaultyScript(string path, string sourcecode)
    {
        if (File.Exists(path))
        {
            string fileContent = File.ReadAllText(path);
            string updatedContent = fileContent.Replace("\nModel - " + Chat.ModelName + "\nNumber of models in the scene ~ "
                    + Chat.Number_of_Objects + "\nYou wrote the following  sentence : " +
                    Input_Text.text + "\n" + "\n" + "The script generated by the AI is the following: \n " + sourcecode + "\n" +
                    "Elapsed time for the generation of the script took " + Chat.elapsed_time + " seconds"
                    + "\n" + "The IA required " + (temp - FaultyScriptCount) + " tries , for obtaining an accetable script \n" + "The number of faulty script for this environment were " + FaultyScriptCount + "\n", "");



            File.WriteAllText(path, updatedContent);
        }

        else { }
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

//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

//------------------------------------ POP UP ------------------------------------------------------------------------
    IEnumerator showPopup()
    {
        popup.SetActive(true); // Mostra il pop-up
        yield return new WaitForSeconds(5); // Aspetta 5 secondi
        popup.SetActive(false); // Nasconde il pop-up
    }
//----------------------------------------------------------------------------------------------------------------------

//-------------------------------- UNITY CONSOLE ERROR CHECKER --------------------------------------------------------------
    private void OnEnable()
    {
        // LogMessageReceived Activated when the script is used
        Application.logMessageReceived += OnLogMessageReceived;
    }

    private void OnDisable()
    {
        // LogMessageReceived deactivated when the script is deactivated
        Application.logMessageReceived -= OnLogMessageReceived;
    }
    private void OnLogMessageReceived(string logString, string stackTrace, LogType type)
    {
        //It checks if there are errors in the Unity Console
        if (type == LogType.Error || type == LogType.Exception)
        {
            
            //It execute CodeErrorExecution only one time
            if(errorcount == 0)
            {
                CodeErrorExecution();
                errorcount++;
            }

            
        }
    }
    private void CodeErrorExecution()
    {
           //We get access to the ReadStringInput Method inside Chat.cs
            GameObject IA_Manager = GameObject.Find("Ai_Manager");
            chat = IA_Manager.GetComponent<Chat>();
            Debug.Log("The AI generated script contains syntax compilation errors or Unity Exceptions");
            isExecutable = false;
            
            //Add the Faulty Script to Faulty_Script.txt
            CreateFaultyScriptsFile(sourceCode, Input_Text);
            //Delete it from the session Log File
            DeleteFaultyScript(path,sourceCode);

            FaultyScriptCount++;//Increase the number of faulty scripts generated for an environment
            StartCoroutine(showPopup());//It shows a 5 seconds pop up error
            
            //Start the process again
            chat.ReadStringInput(InputField);
            DoScript();
            Generate_Script_Button.interactable = false;
            
    }
    //--------------------------------------------------------------------------------------------------------------------------------
}//END OF THE SCRIPT













