using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private int currentIndex = 0;
    private int currentPrefabIndex = 0;
    private GameObject currentInstance;
    public TMP_Text Macro_Text;
    public TMP_Text Objects_Text;
    public float rotationSpeed = 30f;

    List<string> Macrocategory = new List<string>() { "Nature", "City", "Industry", "Cars", "Furniture" };
    List<string> Nature = new List<string>() {"Oak", "Pine" , "Bush","Flower","Mushroom","Stone","Wood"};
    List<string> City = new List<string>() {"Bench", "Stoplight", "Barrel", "Bin", "Dumpster", "Hydrant", "Mailbox" };
    List<string> Furniture = new List<string>() { "Bed", "Chair", "Desk", "Drawer", "Shower", "Sink", "Table" };
    List<string> Industry = new List<string>() {"Cable","Car","Garbage","Pallet","Plank","Tank","Tubes"};
    List<string> Cars = new List<string>() {"Cops","Sedan","Sport","Suv","Taxi"};
 
    public List<GameObject> NaturePrefabs;
    public List<GameObject> CityPrefabs;
    public List<GameObject> FurniturePrefabs;
    public List<GameObject> CarsPrefabs;
    public List<GameObject> IndustryPrefabs;

    public Transform previewPosition;       // Posizione nella scena dove istanziare il prefab di anteprima
  

    // Start is called before the first frame update
    void Start()
    {
        Macro_Text.text = Macrocategory[0];
        Objects_Text.text = Nature[0];
        ShowPrefab(currentPrefabIndex);
    }

    void Update()
    {
        // Ruota il prefab corrente se esiste
        if (currentInstance != null)
        {
            currentInstance.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
    }


    public void IncreaseMacro() { 
 
        currentIndex = (currentIndex + 1) % Macrocategory.Count; // 0 resto 1 quando index è 1, stessa cosa per 2 3 4 , quando è 5 si riparte
        Macro_Text.text = Macrocategory[currentIndex];
        if(Macro_Text.text == "Nature")
        {
            Objects_Text.text = Nature[0];
        }

        if (Macro_Text.text == "City")
        {
            Objects_Text.text = City[0];
        }

        if (Macro_Text.text == "Furniture")
        {
            Objects_Text.text = Furniture[0];
        }


        if (Macro_Text.text == "Industry")
        {
            Objects_Text.text = Industry[0];
        }


        if (Macro_Text.text == "Cars")
        {
            Objects_Text.text = Cars[0];
        }

        ShowPrefab(currentPrefabIndex);
    }

    public void DecreaseMacro()
    {
        currentIndex = (currentIndex - 1 + Macrocategory.Count) % Macrocategory.Count;
        Macro_Text.text = Macrocategory[currentIndex];
        if (Macro_Text.text == "Nature")
        {
            Objects_Text.text = Nature[0];
        }

        if (Macro_Text.text == "City")
        {
            Objects_Text.text = City[0];
        }

        if (Macro_Text.text == "Furniture")
        {
            Objects_Text.text = Furniture[0];
        }


        if (Macro_Text.text == "Industry")
        {
            Objects_Text.text = Industry[0];
        }


        if (Macro_Text.text == "Cars")
        {
            Objects_Text.text = Cars[0];
        }

        ShowPrefab(currentPrefabIndex);
    }

    public void IncreaseObjects()
    {
        if(Macro_Text.text == "Nature")
        {
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

    void ShowPrefab(int index)
    {
        // Rimuovi il prefab corrente se esiste
        if (currentInstance != null)
        {
            Destroy(currentInstance);
        }
        if(Macro_Text.text == "Nature")
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
