using UnityEngine;

public class VictimInteraction : MonoBehaviour
{
    private Outline outline;
    private bool canInteract = false;
    private bool hasBeenClicked = false;

    void Awake()
    {
        outline = GetComponent<Outline>();
        if (outline != null) outline.enabled = false;
    }

    // Wordt nu direct aangeroepen door de EHBOStappenChecker 
    // zodra de stap 'Omstanders Aangesproken' is voltooid.
    public void EnableVictimInteraction()
    {
        canInteract = true;
    }

    public void OnHoverEnter()
    {
        if (outline != null && canInteract && !hasBeenClicked)
        {
            outline.enabled = true;
        }
    }

    public void OnHoverExit()
    {
        if (outline != null) outline.enabled = false;
    }

    public void OnVictimSelect()
    {
        // Alleen actie ondernemen als interactie is vrijgegeven en nog niet is uitgevoerd
        if (!canInteract || hasBeenClicked) return;
        
        hasBeenClicked = true;

        if (outline != null) outline.enabled = false;

        // Geef direct het seintje aan de checker.
        // De checker zal nu direct de Schouder-Master-Zone aanzetten.
        if (EHBOStappenChecker.Instance != null)
        {
            EHBOStappenChecker.Instance.RegisterStep("Bewustzijn Check");
        }
        
        Debug.Log("Slachtoffer geselecteerd: Stap 'Bewustzijn Check' direct doorgegeven.");
    }
}