using System.Collections;
using UnityEngine;

public class ProgressBarUI : MonoBehaviour
{
    public static ProgressBarUI Instance;

    private string shaderProgressName = "_FillAmount"; 
    
    // Deze bewaren we tijdelijk voor de stap die NU actief is
    private Material currentLeftHandMat;
    private Material currentRightHandMat;
    
    private Coroutine countdownCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    /// <summary>
    /// Start de voortgang en vertel het script DIRECT welke handen er gevuld moeten worden.
    /// </summary>
    public void StartProgressBar(float duration, Renderer leftHand, Renderer rightHand)
    {
        if (countdownCoroutine != null) 
        {
            StopCoroutine(countdownCoroutine);
        }

        // Pak de materialen van de specifieke handen die nu in de zone zitten
        if (leftHand != null) currentLeftHandMat = leftHand.material;
        if (rightHand != null) currentRightHandMat = rightHand.material;
        
        countdownCoroutine = StartCoroutine(AnimateBar(duration));
    }

    public void StopProgressBar()
    {
        if (countdownCoroutine != null) 
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null; 
        }

        ResetCurrentHandMaterials();
    }

    private IEnumerator AnimateBar(float duration)
    {
        float elapsed = 0f;
        UpdateHandMaterials(0f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            
            UpdateHandMaterials(progress);

            yield return null;
        }

        countdownCoroutine = null;
        ResetCurrentHandMaterials(); 
    }

    private void UpdateHandMaterials(float value)
    {
        if (currentLeftHandMat != null) currentLeftHandMat.SetFloat(shaderProgressName, value);
        if (currentRightHandMat != null) currentRightHandMat.SetFloat(shaderProgressName, value);
    }

    private void ResetCurrentHandMaterials()
    {
        if (currentLeftHandMat != null) currentLeftHandMat.SetFloat(shaderProgressName, 0f);
        if (currentRightHandMat != null) currentRightHandMat.SetFloat(shaderProgressName, 0f);
        
        // Maak de referenties weer leeg voor de volgende stap
        currentLeftHandMat = null;
        currentRightHandMat = null;
    }
}