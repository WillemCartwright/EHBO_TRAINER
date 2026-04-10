using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SimplePanicAudio : MonoBehaviour
{
    [Header("Audio Instellingen")]
    [SerializeField] private AudioClip panicClip;
    [SerializeField] private float interval = 20f;
    [SerializeField] private float initialDelay = 10f; // De vertraging voor de eerste schreeuw
    [SerializeField] private string animatorParameter = "shocked";

    private AudioSource audioSource;
    private Animator animator;
    private float timer;
    private bool hasStarted = false;
    private bool initialDelayFinished = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        
        // We beginnen de timer op 0
        timer = 0; 
    }

    void Update()
    {
        // Check of de NPC in paniek is (animator parameter is true)
        if (animator != null && animator.GetBool(animatorParameter))
        {
            timer += Time.deltaTime;

            // Stap 1: Wacht op de eerste vertraging
            if (!initialDelayFinished)
            {
                if (timer >= initialDelay)
                {
                    PlaySound();
                    timer = 0; // Reset timer voor het normale interval
                    initialDelayFinished = true;
                    hasStarted = true;
                }
            }
            // Stap 2: Gebruik het normale interval voor de rest van de tijd
            else
            {
                if (timer >= interval)
                {
                    PlaySound();
                    timer = 0;
                }
            }
        }
        else
        {
            // Reset alles als de paniek stopt (zodat het opnieuw begint als ze later weer schrikt)
            hasStarted = false;
            initialDelayFinished = false;
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