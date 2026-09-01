using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FMODUnity;

public class PageCollect : MonoBehaviour
{
    public GameObject collectTextObj, intText;
    public bool interactable;
    public static int pagesCollected;
    public Text collectText;
    public AudioSource PickupSound;

    public GameObject page;
    [SerializeField] EventReference PageCollectSound;

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

    void Start()
    {
        pagesCollected = 0;
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

                CollectSound();
            }
        }

        if (pagesCollected == 10)
        {
            SceneManager.LoadScene("Win Screen");
        }

    }

    public void CollectSound()
    {
        RuntimeManager.PlayOneShotAttached(PageCollectSound, page);
    }
}
