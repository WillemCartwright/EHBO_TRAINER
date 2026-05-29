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
    [SerializeField] private GameObject aedInHand;   
    [SerializeField] private GameObject aedOpGrond;
    
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
    { // <--- DEZE MISTE HIER! Nu staat hij er netjes in.
        if (animator == null) return;

        // 1. STATUS OVERNEMEN (Kijken of het incident is begonnen)
        if (victimAnimator != null)
        {
            bool isVictimShocked = victimAnimator.GetBool("shocked");
            
            if (!animator.GetBool("shocked") && isVictimShocked)
            {
                animator.SetBool("shocked", true);
                currentState = MoveState.Run; 
                animator.SetFloat("Speed", 3f);
                
                // --- DE DOODSTEEK VOOR HET GLIJDEN ---
                animator.SetTrigger("startRunning"); 
                
                Debug.Log("[START] Incident begonnen! 'startRunning' trigger afgevuurd.");
            }
            else if (!isVictimShocked)
            {
                animator.SetBool("shocked", false);
            }
        }

        // 2. BLOKKADE: Wachten op het ongeluk bij de start van de game
        if (!animator.GetBool("shocked"))
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        // --- HET INCIDENT IS VANAF HIER GEACTIVEERD ---

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

        // 5. Wachten bij M4 of M5
        if (isWachtendOp112 || isWachtendBijAED)
        {
            if (lookAtTarget != null && isWachtendOp112) LookAtTarget();
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
        animator.SetFloat("Speed", 0f); 
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
            currentWaypoint = 4;     
            currentState = MoveState.Run; 
            
            animator.SetFloat("Speed", 3f); 
            animator.SetTrigger("GoRun"); 
            Debug.Log("[NPC] Omstander start the sprint naar M5!");
        }
    }

    private void StopBijAEDWachtplek()
    {
        isWachtendBijAED = true;
        animator.SetFloat("Speed", 0f); 
        Debug.Log("[NPC] Omstander is bij M5 and wacht...");
    }

    public void RentTerugMetAED()
    {
        if (isWachtendBijAED)
        {
            isWachtendBijAED = false;
            currentWaypoint = 5;         
            currentState = MoveState.Run; 
            animator.SetFloat("Speed", 3f); 
            if (aedInHand != null) aedInHand.SetActive(true);
            Debug.Log("[NPC] Omstander sprint nu terug naar M6 met de AED!");
        }
    }

    private void StopAtLastWaypoint()
    {
        if (hasArrivedAtFinal) return;

        hasArrivedAtFinal = true;
        animator.SetFloat("Speed", 0f);
        animator.SetTrigger("Arrived"); 

        // NIEUW: Start het proces om de AED netjes op de grond te leggen
        StartCoroutine(WisselAEDOmNaAnimatie());
    }

    private IEnumerator WisselAEDOmNaAnimatie()
    {
        // Wacht een aantal seconden totdat de NPC op zijn knieën zit 
        // Pas deze 2.0 seconden aan naar de lengte van jouw kniel-animatie!
        yield return new WaitForSeconds(2.0f);

        // Schakel de AED in de hand uit
        if (aedInHand != null) aedInHand.SetActive(false); 

        // Laat de AED op de grond verschijnen!
        if (aedOpGrond != null) aedOpGrond.SetActive(true);

        Debug.Log("[AED] AED ligt nu succesvol aangesloten op de grond!");
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