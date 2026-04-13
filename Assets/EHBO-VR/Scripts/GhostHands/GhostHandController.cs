using UnityEngine;
using System.Collections;

public class GhostHandController : MonoBehaviour
{
    [SerializeField] private GameObject ghostHands;
    [SerializeField] private float displayExtraTime = 3.0f; // De 3 seconden die je wilde

    private Coroutine hideCoroutine;

    void Start()
    {
        if (ghostHands != null) ghostHands.SetActive(false);
    }

    // Wordt aangeroepen door 'When Hover Enter'
    public void ShowHands()
    {
        // Als we nog bezig waren met de handen verbergen, stop dat proces dan
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        if (ghostHands != null)
        {
            ghostHands.SetActive(true);
        }
    }

    // Wordt aangeroepen door 'When Hover Exit'
    public void HideHands()
    {
        // Start het aftellen om de handen te verbergen
        hideCoroutine = StartCoroutine(WaitAndHide());
    }

    private IEnumerator WaitAndHide()
    {
        // Wacht het aantal opgegeven seconden
        yield return new WaitForSeconds(displayExtraTime);
        
        if (ghostHands != null)
        {
            ghostHands.SetActive(false);
        }
        
        hideCoroutine = null;
    }
}