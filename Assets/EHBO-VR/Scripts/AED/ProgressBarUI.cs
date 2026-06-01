using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    public static ProgressBarUI Instance;

    [SerializeField] private Image fillImage; // Sleep hier 'ProgressBar_Vul' in

    private Coroutine countdownCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        gameObject.SetActive(false); // Zorg dat hij onzichtbaar start
    }

    public void StartProgressBar(float duration)
    {
        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);
        gameObject.SetActive(true);
        countdownCoroutine = StartCoroutine(AnimateBar(duration));
    }

    public void StopProgressBar()
    {
        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);
        gameObject.SetActive(false);
    }

    private IEnumerator AnimateBar(float duration)
    {
        float elapsed = 0f;

        // We starten de balk nu netjes op leeg (0)
        if (fillImage != null) fillImage.fillAmount = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            
            // Berekening omgedraaid: loopt nu op van 0 naar 1!
            if (fillImage != null)
            {
                fillImage.fillAmount = elapsed / duration;
            }
            yield return null;
        }

        gameObject.SetActive(false); // Verberg de balk weer als hij 100% vol is
    }
}