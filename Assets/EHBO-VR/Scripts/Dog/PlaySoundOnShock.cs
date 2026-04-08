using UnityEngine;

public class PlaySoundOnShock : StateMachineBehaviour
{
    // Dit script zoekt automatisch naar de AudioSource op de hond
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // We vragen aan de hond: "Heb jij een AudioSource?"
        AudioSource audio = animator.GetComponent<AudioSource>();

        // Als hij die heeft, zeggen we: "Speel nu het geluid af dat erin zit!"
        if (audio != null)
        {
            audio.Play();
        }
        else
        {
            Debug.LogWarning("Oeps! De hond heeft een Shocked-animatie maar ik zie geen AudioSource op het object.");
        }
    }
}