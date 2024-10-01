using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using OpenAIClient.ChatEndpoint;
using OpenAI.Models;
using OpenAI;
using System.Threading.Tasks;
using OpenAI.Chat;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;
using System.Linq;
using System.Data.SqlTypes;
using UnityEngine.SceneManagement;
using System.Text;
using UnityEngine.XR;
using UnityEngine.InputSystem.Android;
using static System.Net.Mime.MediaTypeNames;
using Unity.XR.Oculus.Input;
using Button = UnityEngine.UI.Button;
using Utilities.Extensions;
using NUnit.Framework;
using UnityEngine.Windows;



public class Chat : MonoBehaviour

    
{
  public static string result;

    private string input ;
    public static string input_aux;
    public static string input_auxx;
    public static string input_auxx3;

    public static bool Bases = false;
    public static bool Custom = false;
    private bool check = false;
    string sceneName;

    private int n;
   

    private const string Computing_Message = "Computing the script , just wait!!!!";

    public static float elapsed_time;
    public static int tries = 0;
 

    List<string> Mandatory_Words = new List<string>() {"Find(", "Instantiate", ".name",  };

    List<string> Material_Words = new List<string>() {
           "\"" + "Furniture/Material"+ "\"" , 
           "\"" + "Cars/Material"+ "\"" , 
           "\"" + "Nature/Material"+ "\"",
           "\"" + "City/Material"+ "\"",
           "\"" + "Industrial/Material"+ "\"" };
       
    List<string> Directions = new List<string>() { "Right", "Left" , "Middle","right", "left" , "center"};


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

    List<string> All = new List<string>() { "Barrel\"", "Bench\"", "Bin\"", "Dumpster\"", "Hydrant\"", "Mailbox\"", "Stoplight\"", "Cable\"", "Garbage\"", "Pallet\"", "Car\"", "Plank\"", "Tank\"", "Tubes\"", "Oak\"", "Bush\"", "Mushroom\"", "Wood\"", "Stone\"", "Pine\"", "Flower\"", "Cops\"", "Sedan\"", "Sport\"", "Suv\"", "Taxi\"", "Sport\"", "Desk\"", "Chair\"", "Bed\"", "Table\"", "Drawer\"", "Shower\"", "Sink\"" };


    [SerializeField]
    public TMP_Text Text;

    [SerializeField]
    public TMP_InputField InputField;

    [SerializeField]
    public List<string> Reminders_List = new List<string>();

    [SerializeField]
    public string First_Reminder;

    [SerializeField]
    public static int Number_of_Objects;

    [SerializeField]
    public  Button Generate_Script_Button;

    [SerializeField]
    TMP_Text Output_Text;

    [SerializeField]
    TMP_Text Info_Text; //User Mode Text

    [SerializeField]
    public TMP_Text Number_Models_Text;

    [SerializeField]
    public Material material;

    
 
    public GameObject Models;


    //-------------------- OPEN AI CLIENT INFO ------------------------

 
    public static Model model = Model.GPT3_5_Turbo_16K;
    //public static Model model = Model.GPT3_5_Turbo;
    //public static Model model = Model.GPT4;



    //--------------------------------------------------------------------

    // Update is called once per frame
    async void Start()

    {
        



        Number_Models_Text.SetText("Number of models is : " + Number_of_Objects.ToString());

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



        // Debug.Log(input);



        if (input != null && input != input_aux)
        {



            //Time of execution Start
            float start_time = Time.time;
            string result_aux, result_auxx, result_auxxx;
            //-----------------------OpenAI API Usage----------------------------------

            var messages = new List<Message>
            {
                new Message(Role.User, input)
            };

            var api = new OpenAIClient();
            var chatRequest = new ChatRequest(messages, model);
            result_aux = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
            result_auxx = result_aux.Replace("`", "");
            result = result_auxx.Replace("C#", "");
            char firstNonWhiteSpaceChar = result.FirstOrDefault(c => !Char.IsWhiteSpace(c));
            //-----------------------------------------------------------------------


            Debug.Log(result);

            //If the algorithm run again , it means that the IA was not able to provide a correct script and so the counter is increased by 1
            tries++;

            if (ContainsAll(result, Mandatory_Words) && ContainsAny(result, Material_Words) && (firstNonWhiteSpaceChar == 'u')  && ContainsAny(result,All) && CheckContainsTwoStrings(result,All) && CheckIfWordContainedTwice(result,"Vector3",Number_of_Objects))
            {
             
                
                //Elapsed time for the generation of the script
                elapsed_time = Time.time - start_time;

                //It sets the text of the scroll view
                Text.color = new Color32(27, 255, 0, 255);
                Text.SetText(result.ToString());

          
                //--------------------------------------- User Mode Information ---------------------------------------

                if (sceneName == "VR_User_Scene" || sceneName == "User_Scene")
                {
                    Info_Text.text = ("Executing......");

                }

                //-----------------------------------------------------------------------------------------------------


            }
            else if(input != null)
            {

                Text.color = new Color32(27, 255, 0, 255);

                //--------------------------- User Mode Information -----------------------------------------

                if (sceneName == "VR_User_Scene" || sceneName == "User_Scene")
                {
                    Info_Text.text = ("Sorry, the AI was not able to generate a correct script. Wait! The IA is trying to generate another one :)");
                   


                }

                //-------------------------------------------------------------------------------------

                Text.SetText("Sorry, the AI was not able to generate a correct script. Wait! The IA is trying to generate another one :)");
                

                Start();

            }

            else
            {
                return;
            }

            
       }
       
    }



    //It handles the InputField string written by the user

    //Method called with On End Edit (writing ended)
    public void ReadStringInput(TMP_InputField InputField)

    {
        //While the algorithm is running the button for genearating a script is not interactable, It will be interactable again when the script has been executed
        Generate_Script_Button.interactable = false; 

        //-----------------------------------Deletion of the objects of the old customized or bases scenes -------------------------------

        //For bases scene we have a known number of models
        if (Bases)
        {
            for (int i = 0; i < 7; i++)
            {
                GameObject.Destroy(GameObject.Find("Model_" + i.ToString()));

            }
            Bases = false;
        }


        //For the customized scenes the number of models is determined by the user 
        if (Custom)
        {
           
            for (int i = 0; i < n; i++)
            {
                GameObject.Destroy(GameObject.Find("Model_" + i.ToString()));

            }
            Custom = false;
        }


        //-----------------------------------------------------------------------------------------------------------------------------------
    
      
        input = InputField.text.ToString().ToLower();
        input_auxx = InputField.text.ToString();


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


            input = " the first thing to do must be find using the Find() method the  gameobjects  called 'Model_0', 'Model_1', 'Model_2' 'Model_3 'Model_4  and  destroy them and YOU MUST  substitute them  with the gameobjects THAT YOU MUST   load  from the folder named 'Furniture' inside the folder  'Resources' called 'Desk' , " +
                "Chair' 'Table' 'Chair' 'Chair'  , You MUST RENAME THEM AS 'Model_0' 'Model_0', 'Model_1', 'Model_2' 'Model_3 'Model_4  in the unity hierarchy MANDATORY" +
                    " 'Model_0' (Desk) at Y position equals to -0.47, at X position 0.08 and Z position 7.13 , 'Model_1' (Chair) at Y position equals to -0.47, at X position 0.13 and Z position 9.25 'Model_2' (Table) at Y position equals to -0.47, at X position -2.64 and Z position 4.62 " +
                     "'Model_3' (Chair) at Y position equals to -0.47, at X position  -2.76 and Z position 6.28  'Model_4' (Chair) at Y position equals to -0.47, at X position -4.37 and Z position 4.81 and Y rotation equals -97.34"+        
                    " and add just one collider per gameobject, find the gameobject named Plane and change its" +
                    " material with the material   called 'Material'THAT MUST BE LOADED inside the 'Furniture' folder which is inside the folder Resources and do not destroy it, using a method called Start , avoid any type of comments , you must write only code";

            Start();
         

        }



        // ------ APARTMENT -----------

        else if (ContainsAny(input, Apartment_Strings))
        {
            createModels(7);

            Number_of_Objects = 7;


            input = " the first thing to do must be find using the Find() method the  gameobjects  called 'Model_0', 'Model_1', 'Model_2' 'Model_3 'Model_4 'Model_5 'Model_6 and  destroy them and YOU MUST  substitute them  with the gameobjects THAT YOU MUST   load  from the folder named 'Furniture' inside the folder  'Resources' called 'Bed' , " +
                "Drawer' 'Desk' 'Chair' 'Drawer' 'Shower' 'Sink' , You MUST RENAME THEM AS 'Model_0' 'Model_0', 'Model_1', 'Model_2' 'Model_3 'Model_4 'Model_5 'Model_6 in the unity hierarchy MANDATORY" +
                    " 'Model_0' (Bed) at Y position equals to -0.47, at X position -0.64 and Z position 9.99 , 'Model_1' (Drawer) at Y position equals to -0.47, at X position -3.30 and Z position 12.38 'Model_2' (Desk) at Y position equals to -0.47, at X position -4.35 and Z position 6.35 and Y rotation equals to 87.809" +
                     "'Model_3' (Chair) at Y position equals to -0.47, at X position  -3.31 and Z position 6.09 and Y rotation equals 97.00 'Model_4' (Drawer) at Y position equals to -0.47, at X position 1.42 and Z position 12.1 'Model_5' (Shower) at Y position equals to -0.47, at X position 4.69 " +
                     "and Z position 10.72 and 'Model_6' (Sink) at Y position equals to -0.47, at X position 6.34 and Z position 10.02" +
                    " and add just one collider per gameobject, find the gameobject named Plane and change its" +
                    " material with the material   called 'Material'THAT MUST BE LOADED inside the 'Furniture' folder which is inside the folder Resources and do not destroy it, using a method called Start , avoid any type of comments , you must write only code";
           
            Start();
            
        }

        // ------ FOREST -----------

        else if (ContainsAny(input, Forest_Strings))
        {

            createModels(6);

            Number_of_Objects = 6;


            input = " the first thing to do must be find using the Find() method the  gameobjects  called 'Model_0', 'Model_1', 'Model_2' 'Model_3 'Model_4 'Model_5  and  destroy them and YOU MUST  substitute them  with the gameobjects THAT YOU MUST   load  from the folder named 'Nature' inside the folder  'Resources' called 'Oak' , " +
                "Pine' 'Pine' 'Mushroom' 'Oak' 'Stone' , You MUST RENAME THEM AS 'Model_0' 'Model_0', 'Model_1', 'Model_2' 'Model_3 'Model_4 'Model_5  in the unity hierarchy MANDATORY" +
                    " 'Model_0' (Oak) at Y position equals to -0.47, at X position -4.25 and Z position 10.48, 'Model_1' (Pine) at Y position equals to -0.47, at X position -1.48 and Z position 7.49 'Model_2' (Pine) at Y position equals to -0.47, at X position -0.77 and Z position 9.61 " +
                     "'Model_3' (Mushroom) at Y position equals to -0.47, at X position  -2.31 and Z position 7.68  'Model_4' (Oak) at Y position equals to -0.47, at X position 1.11 and Z position 7.74 'Model_5' (Stone) at Y position equals to -0.47, at X position -2.98 " +
                     "and Z position 13.72 " +
                    " and add just one collider per gameobject, find the gameobject named Plane and change its" +
                    " material with the material   called 'Material'THAT MUST BE LOADED inside the 'Nature' folder which is inside the folder Resources and do not destroy it, using a method called Start , avoid any type of comments , you must write only code";

            Start();

        }

        // ------ NATURE -----------

        else if (ContainsAny(input, Nature_Strings))
        {

            createModels(5);

            Number_of_Objects = 5;


            input = " the first thing to do must be find using the Find() method the  gameobjects  called 'Model_0', 'Model_1', 'Model_2' 'Model_3 'Model_4   and  destroy them and YOU MUST  substitute them  with the gameobjects THAT YOU MUST   load  from the folder named 'Nature' inside the folder  'Resources' called 'Pine' , " +
                "Flower' 'Mushroom' 'Oak' 'Wood'  , You MUST RENAME THEM AS 'Model_0' 'Model_0', 'Model_1', 'Model_2' 'Model_3 'Model_4   in the unity hierarchy MANDATORY" +
                    " 'Model_0' (Pine) at Y position equals to -0.47, at X position -3.52 and Z position 7.48, 'Model_1' (Flower) at Y position equals to -0.47, at X position -4.17 and Z position 7.25 'Model_2' (Mushroom) at Y position equals to -0.47, at X position -2.70 and Z position 7.41 " +
                     "'Model_3' (Oak) at Y position equals to -0.47, at X position  -1.7 and Z position 7.68  'Model_4' (Wood) at Y position equals to -0.47, at X position 1.11 and Z position 7.74 " +      
                    " and add just one collider per gameobject, find the gameobject named Plane and change its" +
                    " material with the material   called 'Material'THAT MUST BE LOADED inside the 'Nature' folder which is inside the folder Resources and do not destroy it, using a method called Start , avoid any type of comments , you must write only code";

            Start();


        }

        // ------ GRID -----------

        else if (ContainsAny(input, Car_Strings))

        {

            createModels(5);

            Number_of_Objects = 5;


            input = " the first thing to do must be find using the Find() method the  gameobjects  called 'Model_0', 'Model_1', 'Model_2' 'Model_3 'Model_4   and  destroy them and YOU MUST  substitute them  with the gameobjects THAT YOU MUST   load  from the folder named 'Cars' inside the folder  'Resources' called 'Sport' , " +
                "Cops' 'Suv' 'Taxi' 'Sedan', You MUST RENAME THEM AS 'Model_0' 'Model_0', 'Model_1', 'Model_2' 'Model_3 'Model_4   in the unity hierarchy MANDATORY" +
                    " 'Model_0' (Sport) at Y position equals to -0.47, at X position -3.74 and Z position 17.69, 'Model_1' (Cops) at Y position equals to -0.47, at X position -0.59 and Z position 22.06 'Model_2' (Suv) at Y position equals to -0.47, at X position 2.52 and Z position 17.69 " +
                     "'Model_3' (Taxi) at Y position equals to -0.47, at X position  -7.33 and Z position 21.52  'Model_4' (Sedan) at Y position equals to -0.47, at X position 6.53 and Z position 21.18 " +
                    " and add just one collider per gameobject, find the gameobject named Plane and change its" +
                    " material with the material   called 'Material'THAT MUST BE LOADED inside the 'Cars' folder which is inside the folder Resources and do not destroy it, using a method called Start , avoid any type of comments , you must write only code";
            Start();
        }

        // ------ CITY -----------

        else if (ContainsAny(input, City_Strings))
        {

            createModels(7);

            Number_of_Objects = 7;


            input = " the first thing to do must be find using the Find() method the  gameobjects  called 'Model_0', 'Model_1', 'Model_2' 'Model_3 'Model_4 'Model_5 'Model_6 and  destroy them and YOU MUST  substitute them  with the gameobjects THAT YOU MUST   load  from the folder named 'City' inside the folder  'Resources' called 'Bench' , " +
               "Bin' 'Mailbox' 'Stoplight' 'Dumpster' 'Barrel' 'Barrel' , You MUST RENAME THEM AS 'Model_0' 'Model_0', 'Model_1', 'Model_2' 'Model_3 'Model_4 'Model_5 'Model_6 in the unity hierarchy MANDATORY" +
                   " 'Model_0' (Bench) at Y position equals to -0.47, at X position -3.29 and Z position 7.85 and Y rotation equals to 88.27 , 'Model_1' (Bin) at Y position equals to -0.47, at X position -3.25 and Z position 5.99 'Model_2' (Mailbox) at Y position equals to -0.47, at X position -3.28 and Z position 9.66 and Y rotation equals to -78.88" +
                    "'Model_3' (Stoplight) at Y position equals to -0.47, at X position  1.31 and Z position 25.09 and Y rotation equals -176.29 'Model_4' (Dumpster) at Y position equals to -0.47, at X position 6.66 and Z position 8.08 and Y rotation equals -92.135 'Model_5' (Barrel) at Y position equals to -0.47, at X position 6.24 " +
                    "and Z position 6.43 and 'Model_6' (Barrel) at Y position equals to -0.47, at X position 7.04 and Z position 6.54" +
                   " and add just one collider per gameobject, find the gameobject named Plane and change its" +
                   " material with the material   called 'Material'THAT MUST BE LOADED inside the 'City' folder which is inside the folder Resources and do not destroy it, using a method called Start , avoid any type of comments , you must write only code";

            Start();


        }

        // ------ INDUSTRY -----------

        else if (ContainsAny(input, Industrial_Strings))
        {

            createModels(6);

            Number_of_Objects = 6;


            input = " the first thing to do must be find using the Find() method the   gameobjects  called 'Model_0', 'Model_1', 'Model_2' 'Model_3 'Model_4 'Model_5  and  destroy them and YOU MUST  substitute them  with the gameobjects THAT YOU MUST   load  from the folder named 'Industrial' inside the folder  'Resources' called 'Tubes' , " +
                "Plank' 'Garbage' 'Pallet' 'Pallet' 'Car' , You MUST RENAME THEM AS 'Model_0' 'Model_0', 'Model_1', 'Model_2' 'Model_3 'Model_4 'Model_5  in the unity hierarchy MANDATORY" +
                    " 'Model_0' (Tubes) at Y position equals to -0.47, at X position -4.56 and Z position 7.55, 'Model_1' (Plank) at Y position equals to -0.47, at X position -1.81 and Z position 11.90 'Model_2' (Garbage) at Y position equals to -0.47, at X position 1.64 and Z position 11.61 " +
                     "'Model_3' (Pallet) at Y position equals to -0.47, at X position  2.90 and Z position 9.70  'Model_4' (Pallet) at Y position equals to -0.47, at X position 4.11 and Z position 9.28 'Model_5' (Car) at Y position equals to -0.47, at X position 6.66 " +
                     "and Z position 8.86 " +
                    " and add just one collider per gameobject, find the gameobject named Plane and change its" +
                    " material with the material   called 'Material'THAT MUST BE LOADED inside the 'City' folder which is inside the folder Resources and do not destroy it, using a method called Start , avoid any type of comments , you must write only code";

            Start();

        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------



        //----------------------------------------------------------------- CUSTOM ENVIRONMENT -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        // ------ FURNITURE -----------


        else if (words_Furniture.Count() == Number_of_Objects && words_Furniture.Count()!=0)
        {
           
           createModels(Number_of_Objects);

            input = Input_Request(input, Number_of_Objects, words_Furniture , "Furniture",list_Directions);


            Start();

  
        }


        // ------ CARS -----------

        else if (words_Cars.Count() == Number_of_Objects && words_Cars.Count() != 0)
        {

            createModels(Number_of_Objects);

            input = Input_Request(input, Number_of_Objects, words_Cars , "Cars", list_Directions);

     


            Start();

            
        }

        // ------ NATURE -----------

        else if (words_Nature.Count() == Number_of_Objects && words_Nature.Count() != 0)
        {

            

            createModels(Number_of_Objects);

            input = Input_Request(input, Number_of_Objects, words_Nature, "Nature", list_Directions);

            Start();

          
        }

        // ------ CITY ----------

        else if (words_City.Count() == Number_of_Objects && words_City.Count() != 0 ) 
        {
            createModels(Number_of_Objects);

            input = Input_Request(input, Number_of_Objects, words_City, "City",list_Directions);


            

            Start();

      

        }


        // ------ INDUSTRIAL -----------

        else if (words_Industrial.Count() == Number_of_Objects && words_Industrial.Count() != 0) 
        {
            createModels(Number_of_Objects);
            input = Input_Request(input, Number_of_Objects, words_Industrial,"Industrial", list_Directions);

            Start();

            
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        // Error Handling : if the user does not ask for the correct number of models , an error is thrown

      

        else if ((words_City.Count() != Number_of_Objects || words_Cars.Count() != Number_of_Objects || words_Industrial.Count() != Number_of_Objects  || words_Nature.Count() != Number_of_Objects  || words_Furniture.Count() != Number_of_Objects)) 
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

         if(!ContainsAny(input,City_Models) && !ContainsAny(input, Furniture_Models) && !ContainsAny(input, Car_Models) && !ContainsAny(input, Nature_Models) && !ContainsAny(input, Industrial_Models) && !ContainsAny(input,Industrial_Strings) && !ContainsAny(input,Nature_Strings) && !ContainsAny(input,Forest_Strings) && !ContainsAny(input,City_Strings) && !ContainsAny(input,Furniture_Strings) && !ContainsAny(input, Apartment_Strings) && !ContainsAny(input, Car_Strings))
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
       

        if (list[i] == "Right" || list[i] == "right")
        {
            randomCoordinate = UnityEngine.Random.Range(9f, 23f);
        } 

        else if (list[i] == "Left" || list[i] == "left")
        {
            randomCoordinate = UnityEngine.Random.Range(10f, 20f);
        }

        else if (list[i] == "Center" || list[i] == "center")
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
        

        if (list[i] == "Right" || list[i] == "right")
        {
            randomCoordinate = UnityEngine.Random.Range(14f, 17f);
        }

        else if (list[i] == "Left" || list[i] == "left")
        {
            randomCoordinate = UnityEngine.Random.Range(-9f, -11f);
        }

        else if (list[i] == "Center" || list[i] == "center")
        {
            randomCoordinate = UnityEngine.Random.Range(-4f, 5f);
        }

        else
        {
            randomCoordinate = UnityEngine.Random.Range(-18f, 16f);
        }

        return randomCoordinate;
    }

    //Input Request function definition
    public string Input_Request(string input, int Number_of_Objects, List<string> list, string Material, List<string> list_Directions)

    {

        input = "the first thing to do must be find using the Find() method the gameobjects  called ";

        input = Define_Models(Number_of_Objects, input) + "and  destroy them and YOU  MUST   substitute them , by using the same pattern of code for all the gameobjects,   with the gameobjects THAT YOU MUST   load  from the       11 folder named " + Material + " inside the folder  'Resources' called ";

        input = Enum_Objects(list, Number_of_Objects, input) + "You MUST RENAME THEM AS ";

        input = Define_Models(Number_of_Objects, input) + " in the unity hierarchy  MANDATORY at the e but only after you positioned them";


        input = Define_Models_Coordinates(list, Number_of_Objects, input, list_Directions) + " , rename them  " +  Enum_Objects(list, Number_of_Objects, input) + "   add just one collider per gameobject, find the gameobject named Plane   and change its" + "material with the material   called 'Material'THAT MUST BE 23 LOADED inside the" +  Material + " folder which is inside the folder  Resources," +
        "using a method called Start , avoid any type of comments , you must write only  code";


       return input;
  }

//It defines which object Name must be inserted in the input for CHATGPT
public string Enum_Objects(List<string> objects, int Number_of_Objects, string input){
        
        

        for(int i = 0; i < Number_of_Objects; i++)
        {
            input += objects[i] + " ";
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



        for (int ii = 0; ii < Number_of_Objects; ii++)
        {
            input += " Model_" + ii.ToString() + "is an " +  objects[ii]  + " at  Y position equals to -0.47, at X Position equals to  " + Random_PositionX(list_Directions,ii).ToString() + " and Z position equals to " + Random_PositionZ(list_Directions,ii).ToString();

        }

        return input;
    }


    //------------------------------------------------- Numbers Of Models Increase - Decrease --------------------------------------------------
    
    //Functions attached to the Plus and Minus Buttons
    public void Add()
    {

        Number_Models_Text.SetText("Number of models is : " + (Number_of_Objects + 1).ToString());
       Number_of_Objects += 1;
    }

    public void Subtract()
    {
        Number_Models_Text.SetText("Number of models is : " + (Number_of_Objects -1).ToString());
        Number_of_Objects -= 1;
    }

    //---------------------------------------------------------------------------------------------------------


    //----------------------------------------- META LANGUAGE ---------------------------------------------------------------------------------

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


    static bool CheckIfWordContainedTwice(string inputString, string word, int Number_of_obj)
    {
        int count = 0;

        for (int i = 0; i < inputString.Length - word.Length + 1; i++)
        {
            if (inputString.Substring(i, word.Length) == word)
            {
                count++;
            }
        }

        if(count == Number_of_obj || count > Number_of_Objects)
        {
            return true;
        }

        if(count < Number_of_Objects)
        {
            return false;
        }

        return false;
      
    }

    //---------------------------------------------------------------------------------------------------------
}

