using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour



{

    public GameObject plane;
    public GameObject objectToPosition;
    public float offset = 0.1f;

    private bool isPositioned = false;


    private int currentIndex = 0; //Index for the correct displayment of the name in the Text
    private int currentPrefabIndex = 0; //Index for the correct displayment of the 3D Object preview
    private GameObject currentInstance;
    public TMP_Text Macro_Text;
    public TMP_Text Objects_Text;
    public float rotationSpeed = 30f;

    List<string> Macrocategory = new List<string>() { "Nature", "City", "Industry", "Cars", "Furniture" };

    //---------------------------------- Objects' name for every Macro-Category ------------------------------------------

    List<string> Nature = new List<string>() {"Pine", "Oak" , "Bush","Flower","Mushroom","Stone","Wood"};
    List<string> City = new List<string>() {"Bench", "Stoplight", "Barrel", "Bin", "Dumpster", "Hydrant", "Mailbox" };

    List<string> Furniture = new List<string>() { "Bed", "Chair", "Desk", "Drawer", "Shower", "Sink", "Table","Bookshelf"
    ,"Cabinet", "Can" , "Chest", "Column", "Couch","Dresser","ElegantDesk","Globe","KingBed","Lamp","Library","PCDesk","Satellite"
    ,"Shelf","SinkCabinet","TableCoffee","Tub","WC","Whitebin" };

    List<string> Industry = new List<string>() {"Cable","Car","Garbage","Pallet","Plank","Tank","Tubes"};
    List<string> Cars = new List<string>() {"Cops","Sedan","Sport","Suv","Taxi"};
    
    //---------------------------------------------------------------------------------------------------------------------
 
    //-------------------------------- Lists of the respective Macro Category Prefabs -----------------------------------

    public List<GameObject> NaturePrefabs;
    public List<GameObject> CityPrefabs;
    public List<GameObject> FurniturePrefabs;
    public List<GameObject> CarsPrefabs;
    public List<GameObject> IndustryPrefabs;

    //---------------------------------------------------------------------------------------------------------------------

    public Transform previewPosition; //3D Object Position for the Preview
  


    void Start()
    {
        Macro_Text.text = Macrocategory[0];
        Objects_Text.text = Nature[0];
        ShowPrefab(currentPrefabIndex);
    }

    void Update()
    {
        // Preview Prefab rotation
        if (currentInstance != null)
        {
            currentInstance.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
    }

    //Menu for the Macro Category when '>' button is clicked. The process is the same for all the Macro Categories
    public void IncreaseMacro() { 
 
        currentIndex = (currentIndex + 1) % Macrocategory.Count; 
        Macro_Text.text = Macrocategory[currentIndex]; //Update of the text with the current index

        //Check which Macro Category is on
        if(Macro_Text.text == "Nature")
        {
           //Start with the first object of the category, show it and reset the prefabindex
            Objects_Text.text = Nature[0];
            ShowPrefab(0);
            currentPrefabIndex = 0;
        }

        if (Macro_Text.text == "City")
        {
            
            Objects_Text.text = City[0];
            ShowPrefab(0);
            currentPrefabIndex = 0;
        }

        if (Macro_Text.text == "Furniture")
        {
           
            Objects_Text.text = Furniture[0];
            ShowPrefab(0);
            currentPrefabIndex = 0;
        }


        if (Macro_Text.text == "Industry")
        {
            
            Objects_Text.text = Industry[0];
            ShowPrefab(0);
            currentPrefabIndex = 0;
        }


        if (Macro_Text.text == "Cars")
        {
            
            Objects_Text.text = Cars[0];
            ShowPrefab(0);
            currentPrefabIndex = 0;
        }
    }

    //Same as before but in this case the button clicked is '<'
    public void DecreaseMacro()
    {
        currentIndex = (currentIndex - 1 + Macrocategory.Count) % Macrocategory.Count;
        Macro_Text.text = Macrocategory[currentIndex];
        if (Macro_Text.text == "Nature")
        {
            
            Objects_Text.text = Nature[0];
            ShowPrefab(0);
            currentPrefabIndex = 0;
        }

        if (Macro_Text.text == "City")
        {
           
            Objects_Text.text = City[0];
            ShowPrefab(0); 
            currentPrefabIndex = 0;
        }

        if (Macro_Text.text == "Furniture")
        {
            
            Objects_Text.text = Furniture[0];
            ShowPrefab(0);
            currentPrefabIndex = 0;

        }


        if (Macro_Text.text == "Industry")
        {
           
            Objects_Text.text = Industry[0];
            ShowPrefab(0);
            currentPrefabIndex = 0;

        }


        if (Macro_Text.text == "Cars")
        {
           
            Objects_Text.text = Cars[0];
            ShowPrefab(0);
            currentPrefabIndex = 0;
        }

        
    }

    //Method attached to the button '>'  for the Gameobjects that scroll through the list of objects belonging to the same macro category
    //Same process for every macro category
    public void IncreaseObjects()
    {
        if(Macro_Text.text == "Nature")
        {
            //Compute the prefab index, show the name of the object and show the 3D object prefab.
            currentPrefabIndex = (currentPrefabIndex + 1) % NaturePrefabs.Count;
            Objects_Text.text = Nature[currentPrefabIndex];
            ShowPrefab(currentPrefabIndex);
        }

        if (Macro_Text.text == "City")
        {
            currentPrefabIndex = (currentPrefabIndex + 1) % CityPrefabs.Count;
            Objects_Text.text = City[currentPrefabIndex];
            ShowPrefab(currentPrefabIndex);
        }

        if (Macro_Text.text == "Furniture")
        {
            currentPrefabIndex = (currentPrefabIndex + 1) % FurniturePrefabs.Count;
            Objects_Text.text = Furniture[currentPrefabIndex];
            ShowPrefab(currentPrefabIndex);
        }

        if (Macro_Text.text == "Industry")
        {
            currentPrefabIndex = (currentPrefabIndex + 1) % IndustryPrefabs.Count;
            Objects_Text.text = Industry[currentPrefabIndex];
            ShowPrefab(currentPrefabIndex);
        }

        if (Macro_Text.text == "Cars")
        {
            currentPrefabIndex = (currentPrefabIndex + 1) % CarsPrefabs.Count;
            Objects_Text.text = Cars[currentPrefabIndex];
            ShowPrefab(currentPrefabIndex);
        }

    }

    //Method attached to the button '<'  for the Gameobjects that scroll through the list of objects belonging to the same macro category
    //Same process for every macro category
    public void DecreaseObjects()
    {
        if(Macro_Text.text == "Nature")
        {
            currentPrefabIndex = (currentPrefabIndex - 1 + NaturePrefabs.Count) % NaturePrefabs.Count;
            Objects_Text.text = Nature[currentPrefabIndex];
            ShowPrefab(currentPrefabIndex);
        }

        if(Macro_Text.text == "City")
        {
            currentPrefabIndex = (currentPrefabIndex - 1 + CityPrefabs.Count) % CityPrefabs.Count;
            Objects_Text.text = City[currentPrefabIndex];
            ShowPrefab(currentPrefabIndex);

        }

        if (Macro_Text.text == "Furniture")
        {
            currentPrefabIndex = (currentPrefabIndex - 1 + FurniturePrefabs.Count) % FurniturePrefabs.Count;
            Objects_Text.text = Furniture[currentPrefabIndex];
            ShowPrefab(currentPrefabIndex);

        }

        if (Macro_Text.text == "Industry")
        {
            currentPrefabIndex = (currentPrefabIndex - 1 + IndustryPrefabs.Count) % IndustryPrefabs.Count;
            Objects_Text.text = Industry[currentPrefabIndex];
            ShowPrefab(currentPrefabIndex);

        }

        if (Macro_Text.text == "Cars")
        {
            currentPrefabIndex = (currentPrefabIndex - 1 + Cars.Count) % CarsPrefabs.Count;
            Objects_Text.text = Cars[currentPrefabIndex];
            ShowPrefab(currentPrefabIndex);

        }

    }

    //Auxiliary method that instantiate the selected gameobject depending on the Macro Category selected
    void ShowPrefab(int index)
    {
        // Rimuovi il prefab corrente se esiste
        if (currentInstance != null)
        {
            Destroy(currentInstance);
        }
        if (Macro_Text.text == "Nature")
        {
            currentInstance = Instantiate(NaturePrefabs[index], previewPosition.position, previewPosition.rotation);
            currentInstance.transform.SetParent(previewPosition, true);


        }

        if (Macro_Text.text == "City")
        {
            currentInstance = Instantiate(CityPrefabs[index], previewPosition.position, previewPosition.rotation);
            currentInstance.transform.SetParent(previewPosition, true);

        }

        if (Macro_Text.text == "Furniture")
        {
            currentInstance = Instantiate(FurniturePrefabs[index], previewPosition.position, previewPosition.rotation);
            currentInstance.transform.SetParent(previewPosition, true);



        }

        if (Macro_Text.text == "Industry")
        {
            currentInstance = Instantiate(IndustryPrefabs[index], previewPosition.position, previewPosition.rotation);
            currentInstance.transform.SetParent(previewPosition, true);

        }

        if (Macro_Text.text == "Cars")
        {
            currentInstance = Instantiate(CarsPrefabs[index], previewPosition.position, previewPosition.rotation);
            currentInstance.transform.SetParent(previewPosition, true);

        }
    }
}
