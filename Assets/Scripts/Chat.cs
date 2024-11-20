using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using OpenAIClient.ChatEndpoint;
using OpenAI.Models;
using OpenAI;
using OpenAI.Chat;
using System;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;
using Button = UnityEngine.UI.Button;
using NUnit.Framework;
using UnityEngine.XR;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
public class Chat : MonoBehaviour

    
{
  public static string result;

    public Transform xrOriginTransform;

    private string input ;
    public static string input_aux;
    public static string input_auxx;
    public static string input_auxx3;

    public static bool Bases = false;
    public static bool Custom = false;
    private bool check = false;
    string sceneName;
    public Camera uiCamera;
    private int n;

    private const string Computing_Message = "Computing the script , just wait!!!!";

    public static float elapsed_time;
    public static int tries = 0;

    [SerializeField] public InputDevice rightController;
    private bool isTriggerPressed = false;

    public static List<float> CustomCoordinatesX = new List<float>();
    public static List<float> CustomCoordinatesZ = new List<float>();

    List<string> Mandatory_Words = new List<string>() {"Find(",".name"};

    List<string> Atleast2_Words = new List<string>() { "Furniture/", "Cars/", "Nature/", "City/", "Industrial/" };

    List<string> Material_Words = new List<string>() {
           "\"" + "Furniture/Material"+ "\"" , 
           "\"" + "Cars/Material"+ "\"" , 
           "\"" + "Nature/Material"+ "\"",
           "\"" + "City/Material"+ "\"",
           "\"" + "Industrial/Material"+ "\"" };
       
    List<string> Directions = new List<string>() { "Right", "Left" , "Middle","right", "left" , "center","Center","middle"};


    List<string> Furniture_Strings = new List<string>() { "Office" };
    List<string> Apartment_Strings = new List<string>() { "Apartment" };
    List<string> Furniture_Models = new List<string>() {"Desk", "Chair" , "Bed" , "Table" ,"Drawer","Shower", "Sink"};

    List<string> Car_Strings = new List<string>() { "Cars","Grid" };
    List<string> Car_Models = new List<string>() { "Cops", "Sedan", "Sport" , "Suv", "Taxi" , "Sport" };

    List<string> Nature_Strings = new List<string>() {"Nature"};
    List<string> Forest_Strings = new List<string>() { "Forest" };
    List<string> Nature_Models = new List<string>() { "Oak", "Bush", "Mushroom", "Wood", "Stone" , "Pine", "Flower"};

    List<string> Industrial_Strings = new List<string>() {"Industry","Industrial"};
    List<string> Industrial_Models = new List<string>() {"Cable","Garbage","Pallet", "Car" , "Plank", "Tank" , "Tubes"};

    List<string> City_Strings = new List<string>() {"City"};
    List<string> City_Models = new List<string>() {"Barrel" , "Bench" , "Bin" , "Dumpster" , "Hydrant", "Mailbox" , "Stoplight"};

    List<string> All = new List<string>() { "Barrel\"", "Bench\"", "Bin\"", "Dumpster\"", "Hydrant\"", "Mailbox\"", "Stoplight\"", "Cable\"", "Garbage\"", "Pallet\"", 
                                            "Car\"", "Plank\"", "Tank\"", "Tubes\"", "Oak\"", "Bush\"", "Mushroom\"", 
                                            "Wood\"", "Stone\"", "Pine\"", "Flower\"", "Cops\"", "Sedan\"", "Sport\"", "Suv\"", "Taxi\"", 
                                            "Sport\"", "Desk\"", "Chair\"", "Bed\"", "Table\"", "Drawer\"", "Shower\"", "Sink\"" };


    [SerializeField] public TMP_Text Text;

    [SerializeField] public TMP_InputField InputField;

    [SerializeField] public List<string> Reminders_List = new List<string>();

    [SerializeField] public string First_Reminder;

    [SerializeField] public static int Number_of_Objects;

    [SerializeField] public  Button Generate_Script_Button;

    [SerializeField] TMP_Text Output_Text;

    [SerializeField] TMP_Text Info_Text; //User Mode Text

    [SerializeField] public TMP_Text Number_Models_Text;

    [SerializeField] public  Material material;

    [SerializeField] public TMP_Dropdown dropdown;

    [SerializeField] public Toggle Coordinates_Toggle;

    [SerializeField] public GameObject Ballon;
 
    public GameObject Models;

   // Riferimento al controller XR
    public GameObject planeObject; // Riferimento al Plane (o altro GameObject)


    int counter = 0;

    //------------------------- NETWORK MANAGERS FOR ALL THE PYTHON MANAGERS ----------------------------------------------------

    public GeminiNetworkManager GeminiNetwork = new GeminiNetworkManager();
    public LlamaNetworkManager LlamaNetwork = new LlamaNetworkManager();
    public CodexNetworkManager CodexNetwork = new CodexNetworkManager();
    public QwenNetworkManager QwenNetwork  = new QwenNetworkManager();
    public Codegeex4NetworkManager Codegeex4NetworkManager = new Codegeex4NetworkManager();
    public CodeLlamaNetworkManager CodeLlamaNetworkManager = new CodeLlamaNetworkManager();

    //----------------------------------------------------------------------------------------------------------------------------

    //-------------------- OPEN AI CLIENT INFO ------------------------

    //public static Model model = Model.GPT3_5_Turbo_16k;
    public static Model model = Model.GPT3_5_Turbo;
    //public static Model model = Model.GPT4;

    public static string ModelName = model.ToString();

    //--------------------------------------------------------------------

    // Update is called once per frame
    public async void Start() {




        Number_Models_Text.SetText("Number of models is : " + Number_of_Objects.ToString() + "(" + counter.ToString() + ")");

        //Reset of the Plane's material
        GameObject plane = GameObject.Find("Plane");
        plane.GetComponent<Renderer>().material = material;
      
        sceneName = SceneManager.GetActiveScene().name;

        //-----------------------INVISIBLE STRINGS HANDLER-----------------------
        if (check)
        {

            input = First_Reminder + input;
            input_aux = First_Reminder + input_aux;

            for (int i = 0; i < Reminders_List.Count; i++)
            {

                input = input + "," + Reminders_List[i];

                input_aux = input_aux + "," + Reminders_List[i];

            }
            check = false;
        }

        //--------------------------------------------------------------------------


        if (input != null && input != input_aux)
        {        

            //Time of execution Start
            float start_time = Time.time;
            string result_aux, result_auxx;

            //-----------------------OpenAI API Usage----------------------------------

            if (dropdown.options[dropdown.value].text == "GPT")
            {

                var messages = new List<OpenAI.Chat.Message>
            {
                new OpenAI.Chat.Message(Role.User, input)
            };

                var api = new OpenAIClient();
                var chatRequest = new ChatRequest(messages, model);
                result_aux = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
                result = result_aux.Replace("C#", "").Replace("`", "");
                char firstNonWhiteSpaceChar = result.FirstOrDefault(c => !Char.IsWhiteSpace(c));
                //Debug.Log(result);
                AIList(result, firstNonWhiteSpaceChar, Number_of_Objects, start_time);
                tries++;

            }

            //-----------------------------------------------------------------------

            //---------------------------- GEMINI Python Server Usage----------------------------------------------

            if (dropdown.options[dropdown.value].text == "GEMINI")
            {

                await GeminiNetwork.SendMessageToServer(input);
                result_aux = await GeminiNetwork.ReceiveMessages();

                result_aux = RemoveTextBeforeUsing(result_aux);
                result_aux = TrimAfterLastBrace(result_aux);

                result_auxx = result_aux.Replace("`", "");
                result = result_auxx.Replace("C#", "").Replace("csharp", "").Replace("c#", "");
               
                char firstNonWhiteSpaceChar = result.FirstOrDefault(c => !Char.IsWhiteSpace(c));
                ModelName = "Gemini-Pro-1.0"; //The actual Google Gemini LLM must be changed inside the Python Server
                //Debug.Log(result);
                AIList(result, firstNonWhiteSpaceChar, Number_of_Objects, start_time);
                tries++;

            }


            //-------------------------------------------------------------------------------------------------

            //  --------------------------------- LLAMA Python Server Usage ----------------------------------

            if (dropdown.options[dropdown.value].text == "LLAMA")
            {

                await LlamaNetwork.SendMessageToServer(input);
                result_aux = await LlamaNetwork.ReceiveMessages();
                result_aux = RemoveTextBeforeUsing(result_aux);
                result_aux = TrimAfterLastBrace(result_aux);

                result_auxx = result_aux.Replace("`", "");
                result = result_auxx.Replace("C#", "").Replace("csharp", "").Replace("c#", "");
                char firstNonWhiteSpaceChar = result.FirstOrDefault(c => !Char.IsWhiteSpace(c));
                ModelName = "Llama3.1"; //The actual Meta Llama LLM must be changed inside the Python Server
                //Debug.Log(result)
                AIList(result, firstNonWhiteSpaceChar, Number_of_Objects, start_time);
                tries++; 
               
            }

            // -------------------------------------------------------------------------------------------------

            //------------------------------- CODEX ------------------------------------------------------------------

            if (dropdown.options[dropdown.value].text == "CODEX")
            {

                await CodexNetwork.SendMessageToServer(input);
                result_aux = await CodexNetwork.ReceiveMessages();
                result_aux = RemoveTextBeforeUsing(result_aux);
                result_aux = TrimAfterLastBrace(result_aux);

                result_auxx = result_aux.Replace("`", "");
                result = result_auxx.Replace("C#", "").Replace("csharp", "").Replace("c#", "");
                
                char firstNonWhiteSpaceChar = result.FirstOrDefault(c => !Char.IsWhiteSpace(c));
                ModelName = "gpt-4o-mini"; //The actual Codex LLM must be changed inside the Python Server
                //Debug.Log(result);
                AIList(result, firstNonWhiteSpaceChar, Number_of_Objects, start_time);

                tries++;

            }

            //-----------------------------------------------------------------------------------------------------

            //------------------------------ QWEN --------------------------------------------------------

            if (dropdown.options[dropdown.value].text == "QWEN")
            {
                await QwenNetwork.SendMessageToServer(input);
                result_aux = await QwenNetwork.ReceiveMessages();

                result_aux = RemoveTextBeforeUsing(result_aux);
                result_aux = TrimAfterLastBrace(result_aux);

                result_auxx = result_aux.Replace("`", "");
                result = result_auxx.Replace("C#", "").Replace("csharp", "").Replace("c#", "");
                
                
                char firstNonWhiteSpaceChar = result.FirstOrDefault(c => !Char.IsWhiteSpace(c));
                ModelName = "qwen2.5-coder"; //The actual  LLM must be changed inside the Python Server
                //Debug.Log(result);
                AIList(result, firstNonWhiteSpaceChar, Number_of_Objects, start_time);

                tries++;

            }

            //------------------------------ CODEGEEX4 --------------------------------------------------------

            if (dropdown.options[dropdown.value].text == "CODEGEEX4")
            {
                await Codegeex4NetworkManager.SendMessageToServer(input);
                result_aux = await Codegeex4NetworkManager.ReceiveMessages();

                result_aux = RemoveTextBeforeUsing(result_aux);
                result_aux = TrimAfterLastBrace(result_aux);
          
                result_auxx = result_aux.Replace("`", "");
                result = result_auxx.Replace("C#", "").Replace("csharp", "").Replace("c#", "");
            
                char firstNonWhiteSpaceChar = result.FirstOrDefault(c => !Char.IsWhiteSpace(c));
                ModelName = "codegeex4"; //The actual LLM must be changed inside the Python Server
                //Debug.Log(result);
                AIList(result, firstNonWhiteSpaceChar, Number_of_Objects, start_time);

                tries++;

            }

            //-------------------------------------------------------------------------------------------------

            //------------------------------ CODELLAMA --------------------------------------------------------

            if (dropdown.options[dropdown.value].text == "CODELLAMA")
            {
                await CodeLlamaNetworkManager.SendMessageToServer(input);
                result_aux = await CodeLlamaNetworkManager.ReceiveMessages();

                result_aux = RemoveTextBeforeUsing(result_aux);
                result_aux = TrimAfterLastBrace(result_aux);

                result_auxx = result_aux.Replace("`", "");
                result = result_auxx.Replace("C#", "").Replace("csharp", "").Replace("c#", "");

                char firstNonWhiteSpaceChar = result.FirstOrDefault(c => !Char.IsWhiteSpace(c));
                ModelName = "codellama"; //The actual LLM must be changed inside the Python Server
                //Debug.Log(result);
                AIList(result, firstNonWhiteSpaceChar, Number_of_Objects, start_time);

                tries++;

            }

            //-------------------------------------------------------------------------------------------------

            //-------------------------------------------------------------------------------------------------
        }
    }




    public void AIList(string result, char firstNonWhiteSpaceChar, int Number_Of_Objects, float start_time)
    {
        //Debug.Log(ContainsAll(result, Mandatory_Words));
        //Debug.Log(ContainsAny(result, Material_Words));
        //Debug.Log(firstNonWhiteSpaceChar == 'u');
        //Debug.Log(ContainsAny(result, All));
        //Debug.Log(CheckContainsTwoStrings(result, All));
        //Debug.Log(ContieneSottoStringaAlmenoDueVolte(result, "Nature"));
        Domain.errorcount = 0;

        //EXECUTION CHECKS
        //The generated script must pass all these checks
        if (ContainsAll(result, Mandatory_Words) && ContainsAny(result, Material_Words) && (firstNonWhiteSpaceChar == 'u')
            && CheckContainsTwoStrings(result, All) && (SubStringin2Times(result, "Nature") || SubStringin2Times(result, "Furniture")
            || SubStringin2Times(result, "City") || SubStringin2Times(result, "Industry") || SubStringin2Times(result, "Cars")))
        {

            //Elapsed time for the generation of the script
            elapsed_time = Time.time - start_time;

            //It sets the text of the scroll view
            Text.color = new Color32(27, 255, 0, 255);
            Text.SetText(result.ToString());

            // If we arrive at this point, we know that the script generated is acceptable. So we must set the input string to STOP in order to
            // block the communication between the python server and the GEMINI LLM
            
               input = "STOP";

            //--------------------------------------- User Mode Information ---------------------------------------

            if (sceneName == "VR_User_Scene" || sceneName == "User_Scene")
            {
                Info_Text.text = ("Executing......");

            }

            //-----------------------------------------------------------------------------------------------------

        }
        else if (input != null)
        {

            Text.color = new Color32(27, 255, 0, 255);

            //--------------------------- User Mode Information -----------------------------------------

            if (sceneName == "VR_User_Scene" || sceneName == "User_Scene")
            {
                Info_Text.text = ("Sorry, the AI was not able to generate a correct script. Wait! The AI is trying to generate another one :)");

            }

            //-------------------------------------------------------------------------------------

            Text.SetText("Sorry, the AI was not able to generate a correct script. Wait! The AI is trying to generate another one :)");

            Start();
        }

        else
        {
            return;
        }
    }

    //It handles the InputField string written by the user

    //Method called with On End Edit (writing ended) attached to "Generate Script" Button
    public void ReadStringInput(TMP_InputField InputField)

    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        //While the algorithm is running the button for genearating a script is not interactable, It will be interactable again when the script has been executed
        Generate_Script_Button.interactable = false;

        //-----------------------------------Deletion of the objects of the old customized or bases scenes -------------------------------

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Clone")|| obj.name.Contains("Model_"))
            {
                Destroy(obj);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------


        input = InputField.text.ToString().ToLower();
        input_auxx = InputField.text.ToString().ToLower();


        List<string> words_Furniture = isIn(input, Furniture_Models);
        List<string> words_Nature = isIn(input, Nature_Models);
        List<string> words_Cars = isIn(input, Car_Models);
        List<string> words_Industrial = isIn(input, Industrial_Models);
        List<string> words_City = isIn(input, City_Models);

        List<string> allWords = words_Cars.Concat(words_City).Concat(words_Industrial).Concat(words_Cars).Concat(words_Nature).Concat(words_Furniture).ToList(); //Auxiliary list for the isDirection method


        List<string> list_Directions_aux = isIn_Direction(input,Directions,allWords); // Example of the final List : Right Chair , Left bed , table

        //String list handler for the specified position

      

        for (int i =0; i < list_Directions_aux.Count; i++)
        {
            if (ContainsAny(list_Directions_aux[i], Directions)) 
            {
                list_Directions_aux.Remove(list_Directions_aux[i+1]); //if in the list the model is preceded by a position, remove the model string from the list 
            }


            else
            {
                list_Directions_aux[i] = " "; // otherwise no position is asked and fill  list with a blank
            }

        }

        List<string> list_Directions = list_Directions_aux;

        Text.color = new Color32(27, 255, 0, 255);
        Text.SetText(Computing_Message);

        //---------------------User Mode Information --------------------------------------

        if (sceneName == "VR_User_Scene" || sceneName == "User_Scene")
        {
            Info_Text.text = (Computing_Message);

        }

        //--------------------------------------------------------------------------------



        check = true;

        //------------------------------------------------------------------------------------ BASE CASES ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

     
        
        // ------ OFFICE -----------

        if (ContainsAny(input, Furniture_Strings))
        {
            createModels(5);

            Number_of_Objects = 5; // In this way the global variable is set with the exact amount of objects for this environment


            input = " Unity C# script code that follow drastically these numbered steps " +
                    " STEP ONE -- Find with the Find() method the objects called  called 'Model_0','Model_1', 'Model_2' 'Model_3 'Model_4 and destroy them " +
                    " STEP TWO -- Find with the Find() method the gameobject 'Plane' and change its material with the following code Resources.Load<Material>(Furniture/Material) " +
                    " STEP THREE -- Substitute them with objects loaded from the Furniture folder, the gameobjects to be uploaded are 'Desk' 'Table' 'Chair' 'Chair'  " +
                    " and rename them 'Model_0', 'Model_1', 'Model_2' 'Model_3 'Model_4 " +
                    " 'Model_0' (Desk) at Vector3(0.08,-0.47,7.13), 'Model_1' (Chair) at Vector3(0.13,-0.47,9.25) " +
                    " 'Model_2' (Table) at Vector3(-2.64,-0.47,4.62) " +
                    " 'Model_3' (Chair) at Vector3(-2.76,-0.47,6.28) 'Model_4' (Chair) at Vector3(-4.37,-0.47,4.81) and  rotation Y equals -97.34 only for this object " +
                    " STEP FOUR -- Add a simple collider for every object " +
                    " STEP FIVE -- Use the method caleld Start() e no auxiliary methods";

            Start();

        }



        // ------ APARTMENT -----------

        else if (ContainsAny(input, Apartment_Strings))
        {
            createModels(7);

            Number_of_Objects = 7;


            input = " A complete C# Unity Script that follow correctly these numbered steps : " +
                    " STEP ONE -- Find with the Find() method the objects called 'Model_0', 'Model_1', 'Model_2' 'Model_3 'Model_4 'Model_5 'Model_6' and destroy them"+
                    " STEP TWO -- Find with the Find() method the gameobject 'Plane' and change its material with the material loaded from Furniture/Material folder" +
                    " STEP THREE -- Substitute them with the objects loaded from the Resources/Furniture, the gameobjects to be uploaded are 'Bed' 'Drawer' 'Desk' 'Chair' 'Drawer' 'Shower' 'Sink'"+
                    " and rename them 'Model_0' 'Model_1', 'Model_2', 'Model_3' 'Model_4' 'Model_5' 'Model_6' " +
                    " Model_0' (Bed) at Vector3(-0.64,-0.47,9.99), 'Model_1' (Drawer) at Vector3(-3.30,-0.47,12.38) " +
                    " (Desk) at Vector3(-4.35,-0.47,6.35) and Y rotation equals to 87.809 only for this object" +
                    " 'Model_3' (Chair) at Vector3(-3.31,-0.47,6.09) and Y rotation equals 97.00 only for this object " +
                    " 'Model_4' (Drawer) at Vector3(1-42,-0.47,12.1) " +
                    " 'Model_5' (Shower) at  Vector3(4.69,-0.47,10.72) and " +
                    " 'Model_6' (Sink) at Vector3(6.34,-0.47,10.02) " +
                    " STEP FOUR -- Add a collider for every object" +
                    " STEP FIVE -- Use a method called Start";

            Start();
            
        }

        // ------ FOREST -----------

        else if (ContainsAny(input, Forest_Strings))
        {

            createModels(6);

            Number_of_Objects = 6;


            input = " A complete C# Unity Script that follow correctly these numbered steps : " +
                    " STEP ONE -- Find with the Find() method the objects called 'Model_0', 'Model_1', 'Model_2' 'Model_3 'Model_4 'Model_5 and destroy them " +
                    " STEP TWO -- Find with the Find() method, the gameobject 'Plane' and change its material with the material loaded from Nature/Material folder" +
                    " STEP THREE -- Substitute them with the objects loaded from the Resources/Nature, the gameobjects to be uploaded are 'Oak' 'Pine' 'Pine' 'Mushroom' 'Oak' 'Stone'" +
                    " and rename them 'Model_0' 'Model_1', 'Model_2', 'Model_3' 'Model_4' 'Model_5' " +
                    " 'Model_0' (Oak) at Vector3(-4.25,-0.47,10.48), 'Model_1' (Pine) at Vector3(-1.48,-0.47,7.49) " +
                    " 'Model_2' (Pine) at Vector3(-0.77,-0.47,9.61) Model_3' (Mushroom) at Vector3(-2.31,-0.47,7.68) " +
                    " 'Model_4' (Oak) at Vector3(1.11,-0.47,7.74) " +
                    " 'Model_5' (Stone) at Vector3(-2.98,-0.47,13.72) " +
                    " STEP FOUR -- Add a collider for every object" +
                    " STEP FIVE use a method called Start";

            Start();

        }

        // ------ NATURE -----------

        else if (ContainsAny(input, Nature_Strings))
        {

            createModels(5);

            Number_of_Objects = 5;

            input = " A complete C# Unity Script that follow correctly these numbered steps :  " +
                    " STEP ONE -- Find with the Find() method the objects called  called 'Model_0','Model_1', 'Model_2' 'Model_3 'Model_4 and destroy them " +
                    " STEP TWO -- Find with the Find() method the gameobject 'Plane' and change its material with the material loaded from Nature/Material folder " +
                    " STEP THREE -- Substitute them with the objects loaded previously from the Resources/Nature, the gameobjects to be uploaded are 'Pine' 'Flower' 'Mushroom' 'Oak' 'Wood' " +
                    " and rename them 'Model_0' 'Model_1', 'Model_2', 'Model_3' 'Model_4' " +
                    " 'Model_0' (Pine) at Vector3(-3.52,-0.43,7.48), 'Model_1' (Flower) at Vector3(-4.17,-0.43,7.25) " +
                    " 'Model_2' (Mushroom) at Vector3(-2.70,-0.43,7.41), 'Model_3' (Oak) at Vector3(-1.7,-0.43,7.68) " +
                    " 'Model_4' (Wood) at Vector3(1.11,-0.43,7.74) " +
                    " STEP FOUR -- Add a collider for every object " +
                    " STEP FIVE -- Use a method called Start";

            Start();

        }

        // ------ GRID -----------

        else if (ContainsAny(input, Car_Strings))

        {

            createModels(5);

            Number_of_Objects = 5;

            input = " A complete C# Unity Script that follow correctly these numbered steps : " +
                    " STEP ONE --  Find with the Find() method the objects called  called 'Model_0','Model_1', 'Model_2' 'Model_3 'Model_4 and destroy them " +
                    " STEP TWO --  Find with the Find() method the gameobject 'Plane' and change its material with the material loaded from Cars/Material folder " +
                    " STEP THREE -- Substitute them with the objects loaded from the Resources/Cars, the gameobjects to be uploaded are 'Pine' 'Flower' 'Mushroom' 'Oak' 'Wood' " +
                    " and rename them 'Model_0' 'Model_1', 'Model_2', 'Model_3' 'Model_4' " +
                    " 'Model_0' (Sport) at Vector3(-3.74,-0.47,17.69) , 'Model_1' (Cops) at Vector3(-0.59,-0.47,22.06) " +
                    " 'Model_2' (Suv) at Vector3(2.52,-0.47,17.69) 'Model_3' (Taxi) at Vector3(-7.33,-0.47,21.52) " +
                    " 'Model_4' (Sedan) at Vector3(6.53,-0.47,21.18) " +
                    " STEP FOUR -- Add a collider for every object " +
                    " STEP FIVE -- Use a method called Start";

            Start();
        }

        // ------ CITY -----------

        else if (ContainsAny(input, City_Strings))
        {

            createModels(7);

            Number_of_Objects = 7;

            input = " A complete C# Unity Script that follow correctly these numbered steps : " +
                    " STEP ONE -- Find with the Find() method the objects called 'Model_0', 'Model_1', 'Model_2' 'Model_3 'Model_4 'Model_5 'Model_6' and destroy them" +
                    " STEP TWO -- Find with the Find() method the gameobject 'Plane' and change its material with the material loaded from City/Material folder" +
                    " STEP THREE -- Substitute them with the objects loaded from the Resources/City, the gameobjects to be uploaded are 'Bench' 'Bin' 'Mailbox' 'Stoplight' 'Dumpster' 'Barrel' 'Barrel' " +
                    " and rename them 'Model_0' 'Model_1', 'Model_2', 'Model_3' 'Model_4' 'Model_5' 'Model_6' " +
                    " 'Model_0' (Bench)  at Vector3(-3.29,-0.47,7.85) and Y rotation equals to 88.27 only for this object, 'Model_1' (Bin) at Vector3(-3.25,-0.47,5.99)  " +
                    " 'Model_2' (Mailbox) at Vector3(-3.28,-0.47,9.66) and Y rotation equals to -78.88 only for this object " +
                    " 'Model_3' (Chair) at Vector3(-3.31,-0.47,6.09) and Y rotation equals 97.00 only for this object, 'Model_3' (Stoplight) at Vector3(1.31,-0.47,25.09) and Y rotation equals -176.29 only for this object" +
                    " 'Model_4' (Dumpster) at Vector3(6.66,-0.47,8.08) and Y rotation equals -92.135 only for this object, 'Model_5' (Barrel) at Vector3(6.24,-0.47,6) " +
                    " 'Model_5' (Barrel) at  Vector3(6.24,-0.47,6.43) Y position equals to -0.47 " +
                    " 'Model_6' (Sink) at Vector3(6.34,-0.47,10.02) and 'Model_6' (Barrel) at Vector3(7.04,-0.47,6.54) " +
                    " STEP FOUR -- Add a collider for every object" +
                    " STEP FIVE -- use a method called Start";
            

            Start();

        }

        // ------ INDUSTRY -----------

        else if (ContainsAny(input, Industrial_Strings))
        {

            createModels(6);

            Number_of_Objects = 6;

            input = " A complete C# Unity Script that follow correctly these numbered steps : " +
                    " STEP ONE -- Find with the Find() method, the gameobjects that are called  'Model_0', 'Model_1', 'Model_2' 'Model_3 'Model_4 'Model_5 and destroy them " +
                    " STEP TWO -- Find with the Find() method the gameobject 'Plane' and change its material with the material loaded from Industrial/Material folder" +
                    " STEP THREE Substitute them with the objects loaded from the Resources/Industrial, the gameobjects to be uploaded are 'Tubes' 'Plank' 'Garbage' 'Pallet' 'Pallet' 'Car' " +
                    " and rename them 'Model_0' 'Model_1', 'Model_2', 'Model_3' 'Model_4' 'Model_5' " +
                    " 'Model_0' (Tubes) at Vector3(-4.56,-0.47,7.55) 'Model_1' (Plank) at Vector3(-1.81,-0.47,11.90) " +
                    " 'Model_2' (Garbage) at Vector3(1.64,-0.47,11.61) 'Model_3' (Pallet) at Vector3(2.90,-0.47,9.70)  " +
                    " 'Model_4' (Pallet) at  Vector3(4.11,-0.47,9.28) " +
                    " 'Model_5' (Car) at Vector3(6.66,-0.47,8.86) " +
                    " STEP FOUR -- Add a collider for every object " +
                    " STEP FIVE -- Use a method called Start";

            
            Start();

        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        //-----------------------------------------------------------------------   CUSTOM ENVIRONMENT -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------



        // -------------------------  FURNITURE   ---------------


        else if (words_Furniture.Count() == Number_of_Objects && words_Furniture.Count()!=0 )
        {
           
           createModels(Number_of_Objects);

            input = Input_Request(input, Number_of_Objects, words_Furniture , "Furniture",list_Directions);


            Start();

  
        }

        // ---------------------------  CARS  -------------------

        else if (words_Cars.Count() == Number_of_Objects && words_Cars.Count() != 0)
        {

            createModels(Number_of_Objects);

            input = Input_Request(input, Number_of_Objects, words_Cars , "Cars", list_Directions);

            Start();

        }

        // ---------------------------  NATURE  ------------------

        else if (words_Nature.Count() == Number_of_Objects && words_Nature.Count() != 0)
        {
            
            createModels(Number_of_Objects);

            input = Input_Request(input, Number_of_Objects, words_Nature, "Nature", list_Directions);

            Start();
          
        }

        // -----------------------------  CITY  --------------------

        else if (words_City.Count() == Number_of_Objects && words_City.Count() != 0 ) 
        {
            createModels(Number_of_Objects);

            input = Input_Request(input, Number_of_Objects, words_City, "City",list_Directions);

            Start();

        }


        // ---------------------------  INDUSTRIAL  ---------------------

        else if (words_Industrial.Count() == Number_of_Objects && words_Industrial.Count() != 0) 
        {
            createModels(Number_of_Objects);

            input = Input_Request(input, Number_of_Objects, words_Industrial,"Industrial", list_Directions);

            Start();

        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        // Error Handling : if the user does not ask for the correct number of models , an error is thrown

     
        else if ((words_City.Count() != Number_of_Objects || words_Cars.Count() != Number_of_Objects || words_Industrial.Count() != Number_of_Objects  
                 || words_Nature.Count() != Number_of_Objects  || words_Furniture.Count() != Number_of_Objects)) 
        {
            Text.color = new Color(255, 0, 0);
            Text.SetText("Error : you have to ask for the exactly amount of models requested  for this simulation");
            Generate_Script_Button.interactable = true;
            input = null; // in this way the execution is blocked
            
            if (sceneName == "VR_User_Scene" || sceneName == "User_Scene")
        {
                Info_Text.text = ("Error : you have to ask for the exactly amount of models requested  for this simulation");
                Generate_Script_Button.interactable = true;
                input = null; // in this way the execution is blocked

            }
            return;
        }


        // Otherwise the user has asked for a model or environment which is not implemented yet

         if(!ContainsAny(input,City_Models) && !ContainsAny(input, Furniture_Models) && !ContainsAny(input, Car_Models) 
            && !ContainsAny(input, Nature_Models) && !ContainsAny(input, Industrial_Models) && !ContainsAny(input,Industrial_Strings) 
            && !ContainsAny(input,Nature_Strings) && !ContainsAny(input,Forest_Strings) && !ContainsAny(input,City_Strings) 
            && !ContainsAny(input,Furniture_Strings) && !ContainsAny(input, Apartment_Strings) && !ContainsAny(input, Car_Strings))
        
        {
            Text.color = new Color(255, 0, 0);
            Text.SetText("Error : The environment you asked is not implemented yet, sorry");
            Generate_Script_Button.interactable = true;

          

            //----------------------------------User Mode Information----------------------------------------------------------------------

            if (sceneName == "VR_User_Scene" || sceneName == "User_Scene")
            {
                Info_Text.text = ("Error : The environment you asked is not implemented yet, sorry");
                Generate_Script_Button.interactable = true;

                input = null; //in this way the execution is blocked
            }
            input = null; // in this way the execution is blocked

            //------------------------------------------------------------------------------------------------------------------
        }

    }

    //----------------------------AUXILIARIES FUNCTIONS-------------------------------------------------------
    public void createModels(int Number)
    {
    
        n = Number;

        for (int i = 0; i < n; i++)
        {
            GameObject obj = Instantiate(Models, transform.position, Quaternion.identity);
            obj.name = "Model_" + i.ToString();
            
        }
    }

    //Defines the Y position , if it is asked for right or left or center position follows some specific numbers , otherwise the position is random
    public float Random_PositionZ(List<string> list, int i)
    {
        float randomCoordinate = 0;
      
        if (list[i].ToLower() == "right")
        {
            randomCoordinate = UnityEngine.Random.Range(9f, 23f);
        } 

        else if (list[i].ToLower() == "left")
        {
            randomCoordinate = UnityEngine.Random.Range(10f, 20f);
        }

        else if (list[i].ToLower() == "center" || list[i].ToLower() == "middle" )
        {
            randomCoordinate = UnityEngine.Random.Range(10f, 20f);
        }

        else
        {
            randomCoordinate = UnityEngine.Random.Range(2f, 30f);
        }
        return randomCoordinate;
    }
    
    //Defines the X position , if it is asked for right or left or center position follows some specific numbers , otherwise the position is random
    public float Random_PositionX(List<string> list, int i)
    {
        float randomCoordinate = 0;
        

        if (list[i].ToLower() == "right")
        {
            randomCoordinate = UnityEngine.Random.Range(14f, 17f);
        }

        else if (list[i].ToLower() == "left")
        {
            randomCoordinate = UnityEngine.Random.Range(-9f, -11f);
        }

        else if (list[i].ToLower() == "center")
        {
            randomCoordinate = UnityEngine.Random.Range(-4f, 5f);
        }

        else
        {
            randomCoordinate = UnityEngine.Random.Range(-18f, 16f);
        }

        return randomCoordinate;
    }


    void Update()
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, devices);

        if (devices.Count > 0)
        {
            rightController = devices[0];
        }

        // Verifica se il controller è valido
        if (rightController.isValid)
        {
            bool triggerValue;

            // Leggi lo stato del grilletto
            if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out triggerValue) && triggerValue)
            {
                // Salva la posizione solo quando il trigger viene premuto per la prima volta
                if (!isTriggerPressed && counter != Number_of_Objects)
                {
                    isTriggerPressed = true;

                  
                        SaveCoordinateXZ(xrOriginTransform.position);
                
                }
            }
            else
            {
                // Se il trigger viene rilasciato, resetta il flag
                isTriggerPressed = false;
            }
        }
    }



    private void SaveCoordinateXZ(Vector3 position)
    {

        float x, z;
        x = position.x;
        z = position.z;
        CustomCoordinatesX.Add(x);
        CustomCoordinatesZ.Add(z);
        Instantiate(Ballon, new Vector3(x, 0.97f, z), Quaternion.identity);
        Number_Models_Text.SetText("Number of models is : " + Number_of_Objects + "(" + (counter+1).ToString() +")");
        counter++;

    }

    //Input Request function definition for the customized environments
    public string Input_Request(string input, int Number_of_Objects, List<string> list, string Material, List<string> list_Directions)
{

        input = "A complete C# Unity Script that follow correctly these numbered steps, DO NOT USE TAGS, :" +
        " STEP ONE -- Find with the Gameobject.Find() method the gameobject called 'Plane' and change its material by using the following code Resources.Load<Material>(" + Material + "/Material) "+
        " STEP TWO -- Find with the method Gameobject.Find(), DO NOT USE FindGameObjectsWithTag() or similar, the gameobjects that are called ";
        input = Define_Models(Number_of_Objects, input) + " and destroy them" +
        " STEP THREE -- Instantiate the new object loaded from the Resources/" + Material + "folder; the objects to be instantiated are the following";
        input = Enum_Objects(list, Number_of_Objects, input) + " with this code, for every object, Resources.Load<GameObject>(" + "\"" + Material + "/nameoftheobject\")"+
        " STEP FOUR -- It is mandatory to rename the freshly created models with .name in the following way: ";
        input = Define_Models(Number_of_Objects, input) +
        " STEP FIVE -- The positions of every object are the follwing and they must be inserted: ";
        input = Define_Models_Coordinates(list, Number_of_Objects, input, list_Directions) + "all the values must be float " +
        " STEP SIX -- Add a box collider for every object";




       return input;
    }

    //It defines which object Name must be inserted in the input for CHATGPT
    public string Enum_Objects(List<string> objects, int Number_of_Objects, string input){
        
        for(int i = 0; i < Number_of_Objects; i++)
        {
            input +=  "'" + objects[i]+ "'" + " ";
        }

        return input;
        }

   //It defines all the model requested 
    public string Define_Models(int Number_of_Objects,string input)
    {

        for (int ii = 0; ii < Number_of_Objects; ii++)
        {
            input += " Model_" + ii.ToString();
            
        }

        return input;
    }

    //Auxiliary function for building the input for CHATGPT , for each objects gives the X and Z coordinates, depending on the position requested by the user
    // list contains the direction of the request model

    public string Define_Models_Coordinates(List<string> objects, int Number_of_Objects, string input, List<string> list_Directions)
    {
        

        if (Coordinates_Toggle.isOn)
        {
            for (int ii = 0; ii < Number_of_Objects; ii++)
            {
                input += " Model_" + ii.ToString() + " is a " + objects[ii] + " at Vector3(" + CustomCoordinatesX[ii].ToString().Replace(",", ".") + ",-0.47," + CustomCoordinatesZ[ii].ToString().Replace(",", ".") + ")";
            }

          
            return input;
        }

        else
        {

            for (int ii = 0; ii < Number_of_Objects; ii++)
            {
                input += " Model_" + ii.ToString() + " is a " + objects[ii] + " at Vector3(" + Random_PositionX(list_Directions, ii).ToString().Replace(",", ".") + ",-0.47," + Random_PositionZ(list_Directions, ii).ToString().Replace(",", ".") + ")";
            }

            return input;
        }
    }

    //------------------------------------------------- Numbers Of Models Increase - Decrease -----------------------------------------------------------------------------------------
    
    //Functions attached to the Plus and Minus Buttons
    public void Add()
    {

        Number_Models_Text.SetText("Number of models is : " + (Number_of_Objects + 1).ToString() + "(" + counter.ToString() + ")");
       Number_of_Objects += 1;
    }

    public void Subtract()
    {
        Number_Models_Text.SetText("Number of models is : " + (Number_of_Objects -1).ToString() + "(" + counter.ToString() + ")");
        Number_of_Objects -= 1;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------- DROPDOWN MENU OPTIONS MANAGER ---------------------------------------------------------------------

    public void GetDropDownValue()
    {
        dropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(dropdown); }); 
    }

    public string DropdownValueChanged(TMP_Dropdown change)

    {
        string selectedOption = dropdown.options[dropdown.value].text;
        Debug.Log(selectedOption);

        return selectedOption; 
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //----------------------------------------- META LANGUAGE AUXILIARY METHODS ---------------------------------------------------------------------------------

    //If the inside the string s there is at list one string of the substrings set return true
    //Otherwise , it returns false
    public static bool ContainsAny(string s, List<string> substrings)
    {
        if (string.IsNullOrEmpty(s) || substrings == null)
            return false;

        return substrings.Any(substring => s.Contains(substring, StringComparison.CurrentCultureIgnoreCase));
    }

    //If  inside the string s there are all the strings inside the list of substrings set return true
    //Otherwise , it returns false
    public static bool ContainsAll(string s, List<string> substrings)
    {
        if (string.IsNullOrEmpty(s) || substrings == null)
            return false;

        return substrings.All(substring => s.Contains(substring, StringComparison.CurrentCultureIgnoreCase));
    }


    //It returns a subset of string of the input that are inside the vocabulary of accepted words
    public static List<string> isIn(string s,List<string> Vocab)
    {
        List <string> subSet = new List<string>();

        //Subdivide the string in a List of substrings 
        List<string> aux = s.Split(' ').ToList();
   
        //For every string inside the list
        foreach (string str in aux)
        {

            //If it is accetable, add it to the final subset
        if(ContainsAny(str,Vocab))
            {
                
                subSet.Add(str.Substring(0, 1).ToUpper() + str.Substring(1).ToLower());
            }

        }
              
        return subSet; 
    }

    //It returns a subset of string of the input that are inside the vocabulary of accepted words with the direction asked
    //( It basically add all the accepted words and all the accepted directions)

    public static List<string> isIn_Direction(string s, List<string> Direction, List<string> allwords)
    {
        List<string> subSet = new List<string>();

        //Subdivide the string in a List of substrings 
        List<string> aux = s.Split(' ').ToList();


        //For every string inside the list
        foreach (string str in aux)
        {

            //If it is accetable, add it to the final subset
            if (ContainsAny(str, Direction) || ContainsAny(str,allwords))
            {

                subSet.Add(str.Substring(0, 1).ToUpper() + str.Substring(1).ToLower());
            }
        }

        return subSet;
    }

    //Checks if at least two strings in a list are contained inside a string of text
    bool CheckContainsTwoStrings(string input, List<string> list)
    {
        int count = 0;

        foreach (string word in list)
        {
            if (input.Contains(word))
            {
                count++;
                if (count >= 2)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool SubStringin2Times(string text, string substring)
    {
        int count = 0;
        int position = 0;

        // Search through the string the occurences of the substring
        while ((position = text.IndexOf(substring, position, StringComparison.OrdinalIgnoreCase)) != -1)
        {
            count++;
            position += substring.Length; // Go on through the string

            // If two occurences are found , return true
            if (count >= 2)
            {
                return true;
            }
        }

        //Otherwise we did not find two occurences and we must return false
        return false;
    }

    public static string TrimAfterLastBrace(string input)
    {
        //Find the last '}' index
        int lastBraceIndex = input.LastIndexOf('}');

        // "}" not found , return the original text
        if (lastBraceIndex == -1)
        {
            return input;
        }

        //Return the substring until the index of the last '}'
        return input.Substring(0, lastBraceIndex + 1);
    }

    static string RemoveTextBeforeUsing(string text)
    {
        //Find the index of the first occurence of the words "using"
        int index = text.IndexOf("using");

        // "using" not found , return the original text
        if (index == -1)
        {
            return text;
        }

        //Return the test from the first usinn occurence
        return text.Substring(index);
    }

    //---------------------------------------------------------------------------------------------------------

}//END OF THE SCRIPT

