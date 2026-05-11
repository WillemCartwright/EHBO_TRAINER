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

        // 2. BLOKKADE: Wachten op het ongeluk
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

        // 4. Aankomst logica
        if (hasArrivedAtFinal)
        {
            animator.SetFloat("Speed", 0f);
            if (lookAtTarget != null) LookAtTarget();
            return;
        }

        // 5. Beweging starten
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
            if (currentWaypoint < waypoints.Length - 1)
            {
                currentWaypoint++;
            }
            else
            {
                StopAtLastWaypoint();
            }
        }
    }

    private void StopAtLastWaypoint()
    {
        if (hasArrivedAtFinal) return; // Voorkom dubbele aanroep

        hasArrivedAtFinal = true;
        animator.SetFloat("Speed", 0f);
        animator.SetTrigger("Arrived"); 

        // GEEN UIManager aanroep meer. 
        // We roepen nu de interactie-logica aan op de NPC zelf.
        NPCInteraction interactionScript = GetComponent<NPCInteraction>();
        if (interactionScript != null)
        {
            interactionScript.TriggerArrivalText(); 
            // In NPCInteraction hebben we de tekst al verwijderd, 
            // dus dit maakt alleen de outlines/interactie mogelijk.
        }
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