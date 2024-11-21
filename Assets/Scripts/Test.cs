using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    private int currentIndex = 0;
    private int currentPrefabIndex = 0;
    private GameObject currentInstance;
    public TMP_Text text;
    public Button button;

  List<string> Macrocategory = new List<string>() { "Nature", "City", "Industrial", "Cars", "Furniture" };

    public List<GameObject> prefabs;        // Lista dei prefab da mostrare
    public Transform previewPosition;       // Posizione nella scena dove istanziare il prefab di anteprima
    public Camera previewCamera;            // Camera che renderizza l'anteprima
    

    // Start is called before the first frame update
    void Start()
    {
        text.text = Macrocategory[0];
        ShowPrefab(currentPrefabIndex);
    }



    public void Increase() { 
 
        currentIndex = (currentIndex + 1) % Macrocategory.Count; // 0 resto 1 quando index è 1, stessa cosa per 2 3 4 , quando è 5 si riparte
        currentPrefabIndex = (currentPrefabIndex +1 ) % prefabs.Count;
        text.text = Macrocategory[currentIndex];
        ShowPrefab(currentPrefabIndex);
    }

    public void Decrease()
    {
        currentIndex = (currentIndex - 1 + Macrocategory.Count) % Macrocategory.Count;
        currentPrefabIndex = (currentPrefabIndex - 1 + prefabs.Count) % prefabs.Count;
        text.text = Macrocategory[currentIndex];
        ShowPrefab(currentPrefabIndex);
    }

    void ShowPrefab(int index)
    {
        // Rimuovi il prefab corrente se esiste
        if (currentInstance != null)
        {
            Destroy(currentInstance);
        }

        // Istanzia il nuovo prefab alla posizione di anteprima
        currentInstance = Instantiate(prefabs[index], previewPosition.position, previewPosition.rotation);
        currentInstance.transform.SetParent(previewPosition, true);

   
    }

}
