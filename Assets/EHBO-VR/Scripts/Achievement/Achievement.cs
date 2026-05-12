using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Achievement : MonoBehaviour
{
    [Header("Feedback Settings")]
    [SerializeField] private TMP_Text achievementText;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource achievementSound;

    // Houd bij welke achievements al getoond zijn
    private HashSet<int> unlockedAchievementsList = new HashSet<int>();

    public enum TutorialType
    {
        Game,
        VR
    }

    public void UnlockAchievement(TutorialType type, int completedStep)
    {
        if (achievementText == null || animator == null)
            return;

        // Unieke key voor VR vs Game
        int uniqueStep = ((int)type) * 100 + completedStep;

        if (unlockedAchievementsList.Contains(uniqueStep))
            return;

        unlockedAchievementsList.Add(uniqueStep);

        // Kies tekst op basis van tutorial type
        switch (type)
        {
            case TutorialType.Game:
                switch (completedStep)
                {
                    case 1:
                        //achievementText.text = "<size=130%><b>De tijd tikt</b></size>\n<size=90%>Prestatie behaald</size>\nTIjdsbalk onder controle";
                        break;
                    case 2:
                        //achievementText.text = "<size=130%><b>Aan de slag</b></size>\n<size=90%>Prestatie behaald</size>\nVR-handen onder controle";
                        break;
                    case 3:
                        //achievementText.text = "<size=130%><b>EHBO in de praktijk</b></size>\n<size=90%>Prestatie behaald</size>\nHandgebaren onder controle";
                        break;
                }
                break;

            case TutorialType.VR:
                switch (completedStep)
                {
                    case 1:
                        //achievementText.text = "<size=130%><b>VR Task 1</b></size>\n<size=90%>Prestatie behaald</size>\nKubus is opgepakt";
                        break;
                    case 2:
                        //achievementText.text = "<size=130%><b>VR Task 2</b></size>\n<size=90%>Prestatie behaald</size>\nKnop ingedrukt";
                        break;
                    case 3:
                        //achievementText.text = "<size=130%><b>VR Task 3</b></size>\n<size=90%>Prestatie behaald</size>\nStop teken gegeven";
                        break;
                }
                break;
        }

        // Speel de animatie
        animator.Play("Achievement", 0, 0f);

        if (achievementSound != null)
            achievementSound.Play();
    }

}
