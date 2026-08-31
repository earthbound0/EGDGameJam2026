using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PageCollect : MonoBehaviour
{
    public GameObject collectTextObj, intText;
    public bool interactable;
    public static int pagesCollected;
    public Text collectText;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(true);
            interactable = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(false);
            interactable = false;
        }
    }

    void Update()
    {
        if(interactable == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                pagesCollected = pagesCollected + 1;
                collectText.text = pagesCollected + "/10 pages";
                collectTextObj.SetActive(true);

                intText.SetActive(false);
                this.gameObject.SetActive(false);
                interactable = false;
            }
        }

        if (pagesCollected == 10)
        {
            SceneManager.LoadScene("Win Screen");
        }

    }
}
