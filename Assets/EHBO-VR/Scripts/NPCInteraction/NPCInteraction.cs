using UnityEngine;
using System.Collections;

public class NPCInteraction : MonoBehaviour
{
    private Outline outline;
    private bool hasBeenAddressed = false;
    private bool arrivalShown = false;
    
    // BELANGRIJK: Start op false. De UIManager zet dit op true zodra de tekst fadet.
    private bool canShowOutline = false; 

    void Awake()
    {
        // Zoek de Outline component op de NPC
        outline = GetComponent<Outline>();
        if (outline != null) 
        {
            outline.enabled = false;
        }
    }

    /// <summary>
    /// Wordt aangeroepen door de UIManager zodra de instructietekst verschijnt.
    /// </summary>
    public void EnableOutlineCapability()
    {
        canShowOutline = true;
    }

    /// <summary>
    /// Eventuele backup methode voor NPCMovement.
    /// </summary>
    public void TriggerArrivalText()
    {
        EnableOutlineCapability();

        if (arrivalShown) return;
        arrivalShown = true;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ToonInstructieAankomst();
        }
    }

    // --- HOVER LOGICA (Koppelen aan Interactable Unity Event Wrapper 'When Hover Enter') ---

    public void OnHoverEnter()
    {
        // Check 1: Mogen we überhaupt interacteren (bijv. niet op de hond geklikt)?
        if (UIManager.Instance != null && !UIManager.Instance.CanInteract()) return;

        // De outline gaat PAS aan als canShowOutline door de UIManager op true is gezet
        if (outline != null && canShowOutline && !hasBeenAddressed)
        {
            outline.enabled = true;
        }
    }

    public void OnHoverExit()
    {
        // Altijd de outline uitzetten als de straal de NPC verlaat
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    // --- SELECT LOGICA (Koppelen aan Interactable Unity Event Wrapper 'When Select') ---

    public void AddressNPC()
    {
        // Check 1: Is er een 'Game Over' door de hond?
        if (UIManager.Instance != null && !UIManager.Instance.CanInteract()) return;

        // Check 2: Als de NPC al is aangesproken of de fase is nog niet gestart, stop dan.
        if (hasBeenAddressed || !canShowOutline) return;
        
        hasBeenAddressed = true;

        // Forceer de outline uit omdat de interactie klaar is
        if (outline != null) 
        {
            outline.enabled = false;
        }

        // Start de tekstreeks in de UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowInteractionSequence();
        }
    }
}