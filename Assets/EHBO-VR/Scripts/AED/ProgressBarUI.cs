using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    public static ProgressBarUI Instance;

    [SerializeField] private Image fillImage; // De groene vulbalk
    private Image achtergrondImage;           // De achtergrondbalk zelf

    private Coroutine countdownCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        achtergrondImage = GetComponent<Image>();
    }

    void Start()
    {
        ZetBalkenZichtbaar(false);
    }

    public void StartProgressBar(float duration)
    {
        // Als er nog een oude timer liep (omdat je er net in/uit ging), zetten we die hard stop
        if (countdownCoroutine != null) 
        {
            StopCoroutine(countdownCoroutine);
        }
        
        ZetBalkenZichtbaar(true);
        countdownCoroutine = StartCoroutine(AnimateBar(duration));
    }

    // --- HIER ZIT DE ENORME VERBETERING ---
    public void StopProgressBar()
    {
        // 1. Stop de lopende vulling-animatie direct op de achtergrond!
        if (countdownCoroutine != null) 
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null; // Maak de referentie weer leeg
        }

        // 2. Reset de voortgang direct hard terug naar 0 (leeg)
        if (fillImage != null) 
        {
            fillImage.fillAmount = 0f;
        }

        // 3. Gooi de balken direct uit beeld
        ZetBalkenZichtbaar(false);
    }

    private IEnumerator AnimateBar(float duration)
    {
        float elapsed = 0f;
        if (fillImage != null) fillImage.fillAmount = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (fillImage != null)
            {
                fillImage.fillAmount = elapsed / duration;
            }
            yield return null;
        }

        // Als de tijd succesvol volgemaakt is, sluiten we netjes af
        countdownCoroutine = null;
        ZetBalkenZichtbaar(false); 
    }

    private void ZetBalkenZichtbaar(bool zichtbaar)
    {
        if (achtergrondImage != null) achtergrondImage.enabled = zichtbaar;
        if (fillImage != null) fillImage.enabled = zichtbaar;
    }
}