using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialScreenTransition : MonoBehaviour
{


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))    
        {
            SceneManager.LoadScene("Gameplay");
        }
    }
}
