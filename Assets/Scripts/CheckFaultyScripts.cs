using UnityEngine;

public class ModelManager : MonoBehaviour
{
    public GameObject[] models;

    void Start()
    {
        // STEP ONE - Find with the GameObject.Find method, not FindObjectsByTag,one time per gameobjects, and they are called Model_0 Model_1 Model_2 Model_3 and destroy them
        foreach (GameObject model in models)
        {
            Destroy(model);
        }

        // STEP TWO - Substitute them with the objects loaded from the Resources/Nature the gameobjects to be uploaded are : 'Oak' 'Oak' 'Pine' 'Pine' in the same way is written here Resources.Load<GameObject>("Nature/nameoftheobject")
        GameObject[] newModels = new GameObject[4];
        newModels[0] = (GameObject)Resources.Load("Nature/Oak");
        newModels[1] = (GameObject)Resources.Load("Nature/Oak");
        newModels[2] = (GameObject)Resources.Load("Nature/Pine");
        newModels[3] = (GameObject)Resources.Load("Nature/Pine");

        // STEP THREE - In the third step, it is mandatory to rename the new models with .name in the following way Model_0 Model_1 Model_2 Model_3
        for (int i = 0; i < 4; i++)
        {
            newModels[i].name = "Model_" + i.ToString();
        }

        // STEP FOUR - The positions for every objects are the following and are ALL mandatory to be inserted in the script as float, do not truncate the code
        Vector3 model0Position = new Vector3(13.79024f, -0.47f, 10.46344f);
        Vector3 model1Position = new Vector3(-12.48934f, -0.47f, 13.51912f);
        Vector3 model2Position = new Vector3(-8.065386f, -0.47f, 16.05688f);
        Vector3 model3Position = new Vector3(10.09068f, -0.47f, 5.89573f);

        // STEP FIVE - Find with the Find() method the gameobject 'Plane' and change its material with the following code Resources.Load<Material>(Nature/Material)
        GameObject plane = GameObject.Find("Plane");
        if (plane != null)
        {
            Material newMaterial = (Material)Resources.Load("Nature/Material");
            plane.GetComponent<Renderer>().material = newMaterial;
        }

        // STEP SIX - add a boxcollider per gameobject
        foreach (GameObject model in models)
        {
            BoxCollider collider = model.AddComponent<BoxCollider>();
            collider.size = new Vector3(1f, 1f, 1f);
        }
    }
}