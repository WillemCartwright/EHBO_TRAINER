using UnityEngine;

public class NPCFilming : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void StartFilming()
    {
        if (animator != null)
        {
            animator.SetBool("shocked", true);
            Debug.Log("De NPC ziet het slachtoffer liggen en begint te filmen.");
        }
    }
}