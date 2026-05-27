using UnityEngine;
using System.Collections;

public class NPCMovement : MonoBehaviour
{
    public enum MoveState { Idle, Walk, Run }

    [Header("Movement Settings")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private MoveState currentState = MoveState.Walk;
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Animator victimAnimator; 
    [SerializeField] private Transform lookAtTarget;
    
    private AudioSource dogAudio;
    private int currentWaypoint = 0;
    private bool hasArrivedAtFinal = false;

    // --- STATUS LOGICA ---
    private bool isWachtendOp112 = false; 
    private bool isWachtendBijAED = false; 

    void Start()
    {
        dogAudio = GetComponent<AudioSource>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator == null) return;

        // 1. STATUS OVERNEMEN (Kijken of het incident is begonnen)
        if (victimAnimator != null)
        {
            bool isVictimShocked = victimAnimator.GetBool("shocked");
            animator.SetBool("shocked", isVictimShocked);
        }

        // 2. BLOKKADE: Wachten op het ongeluk bij de start van de game
        if (!animator.GetBool("shocked"))
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        // --- HET INCIDENT IS BEGONNEN ---

        // 3. Geluid (Blaffen/Schrikken)
        if (dogAudio != null && !dogAudio.isPlaying && !hasArrivedAtFinal) 
        {
            dogAudio.Play();
        }

        // 4. Aankomst logica (Einde van M6 bereikt)
        if (hasArrivedAtFinal)
        {
            animator.SetFloat("Speed", 0f);
            if (lookAtTarget != null) LookAtTarget();
            return;
        }

        // 5. GEWIJZIGD: Als we wachten bij M4 of M5, stoppen we HIER de Update loop (fysieke beweging).
        // We halen 'animator.SetFloat("Speed", 0f)' hier weg! De Animator regelt dit nu zelf via de overgangen.
        if (isWachtendOp112 || isWachtendBijAED)
        {
            if (lookAtTarget != null && isWachtendOp112) LookAtTarget(); // Blijf naar slachtoffer kijken tijdens bellen
            return;
        }

        // 6. Beweging starten
        if (waypoints.Length > 0)
        {
            MoveTowardsWaypoint();
        }
    }

    private void MoveTowardsWaypoint()
    {
        Vector3 target = waypoints[currentWaypoint].position;
        float distance = Vector3.Distance(transform.position, target);
        Vector3 direction = (target - transform.position).normalized;

        if (distance > 0.15f) 
        {
            float speedValue = (currentState == MoveState.Run) ? runSpeed : walkSpeed;
            transform.position += direction * speedValue * Time.deltaTime;
            
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
            }
            
            float animatorSpeed = 0f;
            if (currentState == MoveState.Walk) animatorSpeed = 1f;
            if (currentState == MoveState.Run) animatorSpeed = 3f;

            animator.SetFloat("Speed", animatorSpeed);
        }
        else
        {
            // Check voor M4 (Index 3): Aankomst bij het slachtoffer vóór het bellen
            if (currentWaypoint == 3 && !isWachtendOp112 && !hasArrivedAtFinal)
            {
                StopBijSlachtofferVoor112();
            }
            // Check voor M5 (Index 4): Aankomst op het parkeerterrein voor de AED
            else if (currentWaypoint == 4 && !isWachtendBijAED)
            {
                StopBijAEDWachtplek();
            }
            // Normale doorloop naar volgende waypoints
            else if (currentWaypoint < waypoints.Length - 1)
            {
                currentWaypoint++;
            }
            else
            {
                StopAtLastWaypoint();
            }
        }
    }

    private void StopBijSlachtofferVoor112()
    {
        isWachtendOp112 = true; 
        animator.SetFloat("Speed", 0f); // Zet eenmalig op 0 om de Blend Tree in de Shocked/Idle stand te zetten
        currentState = MoveState.Idle;
        
        NPCInteraction interactionScript = GetComponent<NPCInteraction>();
        if (interactionScript != null)
        {
            interactionScript.TriggerArrivalText(); 
        }
    }

    public void StartRennenNaarAED()
    {
        if (isWachtendOp112)
        {
            isWachtendOp112 = false; 
            currentWaypoint = 4;     // Richt neus naar M5
            currentState = MoveState.Run; 
            
            // We zetten de speed parameter hier alvast handmatig hoog, zodat de gedupliceerde Blend Tree weet dat hij moet RENNEN!
            animator.SetFloat("Speed", 3f); 
            animator.SetTrigger("GoRun"); 
            Debug.Log("[NPC] Omstander start de sprint naar M5!");
        }
    }

    private void StopBijAEDWachtplek()
    {
        isWachtendBijAED = true;
        animator.SetFloat("Speed", 0f); // Eenmalig stopzetten van de ren-animatie bij M5
        Debug.Log("[NPC] Omstander is bij M5 en wacht...");
    }

    public void RentTerugMetAED()
    {
        if (isWachtendBijAED)
        {
            isWachtendBijAED = false;
            currentWaypoint = 5;         
            currentState = MoveState.Run; 
            animator.SetFloat("Speed", 3f); // Direct weer aanzetten voor de eindsprint
            Debug.Log("[NPC] Omstander sprint nu terug naar M6!");
        }
    }

    private void StopAtLastWaypoint()
    {
        if (hasArrivedAtFinal) return;

        hasArrivedAtFinal = true;
        animator.SetFloat("Speed", 0f);
        animator.SetTrigger("Arrived"); 
    }

    private void LookAtTarget()
    {
        Vector3 targetPos = new Vector3(lookAtTarget.position.x, transform.position.y, lookAtTarget.position.z);
        Vector3 lookDir = targetPos - transform.position;
        
        if (lookDir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 2f);
        }
    }
}