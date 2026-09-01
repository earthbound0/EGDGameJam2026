using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenTransition : MonoBehaviour
{


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))    
        {
            SceneManager.LoadScene("Start Screen");
        }
    }
}
