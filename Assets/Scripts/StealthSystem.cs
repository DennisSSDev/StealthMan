using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class StealthSystem : MonoBehaviour
{
    // Start is called before the first frame update
    public PostProcessVolume ppVolume;

    private Vignette vignetteLayer = null;
    private ColorGrading colorGradingLayer = null;

    private bool isInShadow = true;
    private bool hasTriggeredDarknessEffect = false;
    private bool hasTriggeredLightEffect = false;

    private Light[] sceneLights;

    private IEnumerator darknessTimer;
    private IEnumerator lightTimer;
    
    public bool GetIsInShadow()
    {
        return isInShadow;
    }

    void Start()
    {
        sceneLights = GameObject.FindObjectsOfType<Light>();
        ppVolume.profile.TryGetSettings<Vignette>(out vignetteLayer);
        ppVolume.profile.TryGetSettings<ColorGrading>(out colorGradingLayer);
    }

    // Update is called once per frame
    void Update()
    {
        bool localIsInShadow = true;
        foreach(var light in sceneLights)
        {
            float disBetweenLightAndPlayer = Vector3.SqrMagnitude(transform.position - light.transform.position);
            float rangeSqrd = light.range * light.range;
            // in light
            if(disBetweenLightAndPlayer < rangeSqrd)
            {
                localIsInShadow = false;
                if(!hasTriggeredLightEffect)
                {
                    hasTriggeredDarknessEffect = false;
                    hasTriggeredLightEffect = true;
                    if(darknessTimer != null)
                    {
                        StopCoroutine(darknessTimer);
                    }
                    lightTimer = TurnToLight(.02f);
                    StartCoroutine(lightTimer);
                }
            }
        }
        isInShadow = localIsInShadow;
        if(!hasTriggeredDarknessEffect && hasTriggeredLightEffect && isInShadow)
        {
            // deactivate darkness and activate lightness
            hasTriggeredLightEffect = false;
            hasTriggeredDarknessEffect = true;
            if(lightTimer != null)
            {
                StopCoroutine(lightTimer);
            }
            darknessTimer = TurnToShadow(.02f);
            StartCoroutine(darknessTimer);
        }
    }

    private IEnumerator TurnToShadow(float delay)
    {
        while(true)
        {
            // make the vignette dark
            vignetteLayer.intensity.value += 0.05f;
            vignetteLayer.intensity.value = Mathf.Clamp(vignetteLayer.intensity.value, .0f, .3f);
            Debug.Log("Darkness");
            //make the color dark
            colorGradingLayer.colorFilter.value.r -= 0.05f;
            colorGradingLayer.colorFilter.value.g -= 0.05f;
            colorGradingLayer.colorFilter.value.b -= 0.05f;

            colorGradingLayer.colorFilter.value.r = Mathf.Clamp(colorGradingLayer.colorFilter.value.r, .3f, 0.68f);
            colorGradingLayer.colorFilter.value.g = Mathf.Clamp(colorGradingLayer.colorFilter.value.g, .3f, 0.68f);
            colorGradingLayer.colorFilter.value.b = Mathf.Clamp(colorGradingLayer.colorFilter.value.b, .3f, 0.68f);

            yield return new WaitForSeconds(delay);
        }
    }
    private IEnumerator TurnToLight(float delay)
    {
        // make the vignette dark
        while(true)
        {
            vignetteLayer.intensity.value -= 0.05f;
            vignetteLayer.intensity.value = Mathf.Clamp(vignetteLayer.intensity.value, .0f, .3f);
            Debug.Log("Light");
            //make the color dark
            colorGradingLayer.colorFilter.value.r += 0.05f;
            colorGradingLayer.colorFilter.value.g += 0.05f;
            colorGradingLayer.colorFilter.value.b += 0.05f;

            colorGradingLayer.colorFilter.value.r = Mathf.Clamp(colorGradingLayer.colorFilter.value.r, .3f, 0.68f);
            colorGradingLayer.colorFilter.value.g = Mathf.Clamp(colorGradingLayer.colorFilter.value.g, .3f, 0.68f);
            colorGradingLayer.colorFilter.value.b = Mathf.Clamp(colorGradingLayer.colorFilter.value.b, .3f, 0.68f);

            yield return new WaitForSeconds(delay);
        }
    }
}
