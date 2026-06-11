using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Zorg dat deze erbij staat voor de Image component

public class ProgressBarUI : MonoBehaviour
{
    public static ProgressBarUI Instance;

    [Header("UI Elementen")]
    [SerializeField] private GameObject visualCanvasGroup; // Het paneel/canvas van de balk
    [SerializeField] private Image progressFillImage;       // De daadwerkelijke vullende balk (UI Image)

    private Coroutine countdownCoroutine;

    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Zorg dat de balk onzichtbaar is bij de start van de game
        if (visualCanvasGroup != null) visualCanvasGroup.SetActive(false);
        if (progressFillImage != null) progressFillImage.fillAmount = 0f;
    }

    /// <summary>
    /// Start de voortgangsbalk op het scherm voor de opggeven tijd.
    /// (De extra mesh-parameters vangen we op zodat je zone-script niet crashed!)
    /// </summary>
    public void StartProgressBar(float duration, Renderer leftHand = null, Renderer rightHand = null)
    {
        if (countdownCoroutine != null) 
        {
            StopCoroutine(countdownCoroutine);
        }

        if (visualCanvasGroup != null) visualCanvasGroup.SetActive(true);
        
        countdownCoroutine = StartCoroutine(AnimateBar(duration));
    }

    public void StopProgressBar()
    {
        if (countdownCoroutine != null) 
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null; 
        }

        if (progressFillImage != null) progressFillImage.fillAmount = 0f;
        if (visualCanvasGroup != null) visualCanvasGroup.SetActive(false);
    }

    private IEnumerator AnimateBar(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            
            if (progressFillImage != null)
            {
                progressFillImage.fillAmount = progress;
            }

            yield return null;
        }

        countdownCoroutine = null;
        if (visualCanvasGroup != null) visualCanvasGroup.SetActive(false);
    }
}