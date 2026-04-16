using UnityEngine;
using System.Collections;

public class GhostHandController : MonoBehaviour
{
    [SerializeField] private GameObject ghostHands;
    [SerializeField] private float displayTime = 5.0f; // Nu 5 seconden zoals je vroeg

    private bool isDisplaying = false;

    void Start()
    {
        // Forceer de handen uit bij de start
        if (ghostHands != null) ghostHands.SetActive(false);
    }

    // Wordt aangeroepen door 'When Hover Enter' (als de zone groen wordt)
    public void ShowHands()
    {
        if (isDisplaying) return; // Als ze al aanstaan, doe niets (voorkomt haperen)

        if (ghostHands != null)
        {
            StopAllCoroutines(); // Stop eventuele lopende timers
            StartCoroutine(DisplayHandsRoutine());
        }
    }

    // We halen de logica uit HideHands en regelen alles in deze routine
    private IEnumerator DisplayHandsRoutine()
    {
        isDisplaying = true;
        ghostHands.SetActive(true);

        // Hier blijven de handen exact 5 seconden staan
        yield return new WaitForSeconds(displayTime);

        ghostHands.SetActive(false);
        isDisplaying = false;
    }

    // Deze functie laten we leeg of weghalen uit de Event Wrapper Exit
    public void HideHands() 
    {
        // We doen hier niets, de routine regelt het uitzetten na 5 seconden
    }
}