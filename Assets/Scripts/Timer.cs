using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    
    [SerializeField] TextMeshProUGUI TimerText;
    public float ElapsedTime = 720;
    public bool Check = false;

    void Start()
    {

    }

    void Update()
    {
        if(ElapsedTime >= 780)
        {
            ElapsedTime = 60;
            Check = true;
        }

        if(ElapsedTime >= 360 && Check == true)
        {
            SceneManager.LoadScene("Lose Screen");
        }

        ElapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(ElapsedTime / 60);
        int seconds = Mathf.FloorToInt(ElapsedTime % 60);

        TimerText.text = string.Format("{0:00}:{1:00} AM", minutes, seconds);
    }
}
