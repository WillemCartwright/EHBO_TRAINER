using UnityEngine;

public class TriggerFilmingOnLand : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Zoek de filmende NPC in de scene
        NPCFilming filmingNPC = Object.FindAnyObjectByType<NPCFilming>();

        if (filmingNPC != null)
        {
            filmingNPC.StartFilming();
        }
        else
        {
            Debug.LogWarning("NPCFilming script niet gevonden in de scene!");
        }
    }
}