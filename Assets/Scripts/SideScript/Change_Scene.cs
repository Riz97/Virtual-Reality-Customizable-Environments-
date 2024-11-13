using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Change_Scene : MonoBehaviour
{
    // Start is called before the first frame update
   public void  Developer_Scene()
    {
        SceneManager.LoadScene("Developer_Scene");
    }

    public void User_Scene()
    {
        SceneManager.LoadScene("User_Scene");
    }

    public void Useful_Info_Scene()
    {
        SceneManager.LoadScene("Useful_Scene");
    }

    public void Opening_Scene()
    {
        SceneManager.LoadScene("Opening_Scene");
    }

    public void Quit()
    {
        Application.Quit();
    }
    public void VR_Developer_Scene()
    {
        SceneManager.LoadScene(5);
    }

    public void VR_User_Scene()
    {
        SceneManager.LoadScene(6);
    }

    public void VR_Useful_Info_Scene()
    {
        SceneManager.LoadScene(7);
    }

    public void VR_Opening_Scene()
    {
        SceneManager.LoadScene(4);
    }
}
