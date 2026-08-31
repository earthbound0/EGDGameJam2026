using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenTransition : MonoBehaviour
{


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))    
        {
            SceneManager.LoadScene("Tutorial Screen");
        }
    }
}
