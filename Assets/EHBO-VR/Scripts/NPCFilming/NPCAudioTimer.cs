using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NPCAudioTimer : MonoBehaviour
{
    [Header("Instellingen")]
    [SerializeField] private AudioClip panicClip;
    [SerializeField] private float interval = 20f;
    
    private AudioSource audioSource;
    private float timer;
    private bool canPlay = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        // Zorg dat de audio source goed staat ingesteld voor VR (3D geluid)
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f; // Volledig 3D geluid
    }

    void Update()
    {
        if (!canPlay) return;

        timer += Time.deltaTime;

        if (timer >= interval)
        {
            PlayPanicSound();
            timer = 0f; // Reset de timer
        }
    }

    // Deze functie roep je aan op het moment dat ze begint te rennen
    public void StartPanicAudio()
    {
        canPlay = true;
        timer = interval; // Zet op 'interval' zodat ze direct 1 keer schreeuwt bij het starten
    }

    public void StopPanicAudio()
    {
        canPlay = false;
    }

    private void PlayPanicSound()
    {
        if (panicClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(panicClip);
        }
    }
}