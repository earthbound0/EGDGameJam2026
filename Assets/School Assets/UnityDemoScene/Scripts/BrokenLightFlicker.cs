using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))] 
public class BrokenLightFlicker : MonoBehaviour
{
    public float minFlickerTime = 0.05f;
    public float maxFlickerTime = 0.3f;
    public float minPause = 0.5f;
    public float maxPause = 2.5f;

    public Renderer lampRenderer; // Посилання на Renderer з матеріалом
    public Color emissionColor = Color.white;

    private Light flickerLight;
    private float originalIntensity;
    private Material instanceMaterial;
    private static readonly string EMISSION_PROPERTY = "_EmissionColor";

    void Start()
    {
        flickerLight = GetComponent<Light>();
        originalIntensity = flickerLight.intensity;

        if (lampRenderer != null)
        {
            // Створюємо інстанс матеріалу
            instanceMaterial = lampRenderer.material;
            instanceMaterial.EnableKeyword("_EMISSION");
        }

        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            int flickerCount = Random.Range(2, 6);
            for (int i = 0; i < flickerCount; i++)
            {
                SetLightState(false);
                yield return new WaitForSeconds(Random.Range(minFlickerTime, maxFlickerTime));

                SetLightState(true, Random.Range(0.3f, 1f));
                yield return new WaitForSeconds(Random.Range(minFlickerTime, maxFlickerTime));
            }

            SetLightState(true, 1f);
            yield return new WaitForSeconds(Random.Range(minPause, maxPause));
        }
    }

    void SetLightState(bool state, float intensityMultiplier = 1f)
    {
        flickerLight.enabled = state;
        flickerLight.intensity = state ? originalIntensity * intensityMultiplier : 0f;

        if (instanceMaterial != null)
        {
            Color targetColor = state ? emissionColor * intensityMultiplier : Color.black;
            instanceMaterial.SetColor(EMISSION_PROPERTY, targetColor);
        }
    }
}
