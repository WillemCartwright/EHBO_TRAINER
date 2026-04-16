using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SimplePanicAudio : MonoBehaviour
{
    [Header("Audio Instellingen")]
    [SerializeField] private AudioClip panicClip;
    [SerializeField] private float initialDelay = 10f; // De vertraging voor de enige schreeuw
    [SerializeField] private string animatorParameter = "shocked";

    private AudioSource audioSource;
    private Animator animator;
    private float timer;
    private bool hasScreamed = false; // Houdt bij of er al geschreeuwd is

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        timer = 0; 
    }

    void Update()
    {
        // Check of de NPC in paniek is
        if (animator != null && animator.GetBool(animatorParameter))
        {
            // Alleen de timer laten lopen als er nog niet geschreeuwd is
            if (!hasScreamed)
            {
                timer += Time.deltaTime;

                if (timer >= initialDelay)
                {
                    PlaySound();
                    hasScreamed = true; // Zorg dat we hierna niet meer schreeuwen
                }
            }
        }
        else
        {
            // Reset de status als de paniek stopt
            // Zo schreeuwt ze opnieuw 1 keer als de paniek later weer getriggerd wordt
            hasScreamed = false;
            timer = 0; 
        }
    }

    private void PlaySound()
    {
        if (panicClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(panicClip);
        }
    }
}