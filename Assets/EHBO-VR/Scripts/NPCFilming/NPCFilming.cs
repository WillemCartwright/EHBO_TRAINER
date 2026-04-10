using UnityEngine;

public class NPCFilming : MonoBehaviour
{
    private Animator animator;

    [Header("Telefoon Instellingen")]
    [SerializeField] private GameObject phoneObject; // Sleep hier de telefoon uit de hand-bone in

    void Awake()
    {
        animator = GetComponent<Animator>();

        // Zorg dat de telefoon bij het opstarten onzichtbaar is
        if (phoneObject != null)
        {
            phoneObject.SetActive(false);
        }
    }

    /// <summary>
    /// Wordt aangeroepen door het StateMachineBehaviour op het slachtoffer
    /// </summary>
    public void StartFilming()
    {
        if (animator != null)
        {
            // 1. Start de film-animatie (die op Loop Time staat)
            animator.SetBool("shocked", true);
            
            // 2. Maak de telefoon in de hand zichtbaar
            if (phoneObject != null)
            {
                phoneObject.SetActive(true);
                Debug.Log("De NPC pakt de telefoon en begint te filmen.");
            }
        }
        else
        {
            Debug.LogError("Geen Animator gevonden op de NPCFilming!");
        }
    }
}