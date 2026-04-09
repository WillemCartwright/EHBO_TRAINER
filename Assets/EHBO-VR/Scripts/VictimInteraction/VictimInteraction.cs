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

    // Wordt aangeroepen door UIManager zodra de tekst 'Klik het slachtoffer aan' verschijnt
    public void EnableVictimInteraction()
    {
        canInteract = true;
    }

    public void OnHoverEnter()
    {
        // Check of interactie mag (niet op hond geklikt)
        if (UIManager.Instance != null && !UIManager.Instance.CanInteract()) return;

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
        // Check of interactie mag (niet op hond geklikt)
        if (UIManager.Instance != null && !UIManager.Instance.CanInteract()) return;

        if (!canInteract || hasBeenClicked) return;
        hasBeenClicked = true;

        if (outline != null) outline.enabled = false;

        // Start de nieuwe reeks in de UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowVictimReactionSequence();
        }
        
        Debug.Log("Slachtoffer aangesproken, sequence gestart.");
    }
}