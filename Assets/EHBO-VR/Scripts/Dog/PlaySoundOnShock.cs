using UnityEngine;
using Oculus.Interaction; // Nodig om de RayInteractable aan te sturen

public class PlaySoundOnShock : StateMachineBehaviour
{
    // Dit script wordt uitgevoerd zodra de "Shocked" (blaf) animatie start
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 1. Zoek de AudioSource op de hond
        AudioSource audio = animator.GetComponent<AudioSource>();

        // 2. Zoek de RayInteractable op de hond (voor de klik-logica)
        RayInteractable rayInteractable = animator.GetComponent<RayInteractable>();

        // --- GELUID AFSPELEN ---
        if (audio != null)
        {
            audio.Play();
        }
        else
        {
            Debug.LogWarning("De hond heeft geen AudioSource op het object.");
        }

        // --- INTERACTIE ACTIVEREN ---
        if (rayInteractable != null)
        {
            // De hond wordt nu pas klikbaar, precies wanneer het geblaf begint!
            rayInteractable.enabled = true;
            Debug.Log("Hond blaft: RayInteractable geactiveerd.");
        }
        else
        {
            Debug.LogWarning("Ik kan de RayInteractable niet vinden op de hond.");
        }
    }
}