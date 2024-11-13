using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Reset : MonoBehaviour
{
    public void ResetScene()
    {
        // Start is called before the first frame update
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    

}
