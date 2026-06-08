using UnityEngine;
using UnityEngine.AI;

public class CharacterSwarmer : MonoBehaviour
{
    public Transform victim; 
    public float stopDistance = 3.0f; 
    public float rotationSpeed = 5f;
    
    private Animator animator;
    private NavMeshAgent agent;
    private bool shouldSurround = false;
    private bool hasArrived = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        
        if (agent != null)
        {
            agent.stoppingDistance = stopDistance;
            agent.avoidancePriority = Random.Range(30, 60); 
        }
    }

void Update()
    {
        if (hasArrived)
        {
            RotateTowardsVictim();
            return; 
        }

        if (agent == null || !agent.isOnNavMesh || !agent.isActiveAndEnabled)
        {
            return; 
        }

        if (shouldSurround && victim != null)
        {
            agent.SetDestination(victim.position);

            float currentSpeed = agent.velocity.magnitude;
            animator.SetFloat("Speed", currentSpeed);

            // Bereken de fysieke afstand over de grond
            Vector3 npcPos = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 victimPos = new Vector3(victim.position.x, 0, victim.position.z);
            float actualDistance = Vector3.Distance(npcPos, victimPos);

            // Als hij binnen de cirkel komt
            if (actualDistance <= (stopDistance + 1.5f))
            {
                StopNPCAndLockDefinitively();
                return;
            }

            // BACKUP: Als hij plotseling stilvalt
            if (agent.velocity.sqrMagnitude < 0.1f && actualDistance < (stopDistance * 2f))
            {
                StopNPCAndLockDefinitively();
                return;
            }
        }
    }

void StopNPCAndLockDefinitively()
{
    if (hasArrived) return; 

    hasArrived = true; 

    if (agent != null)
    {
        agent.isStopped = true;       // Vertel de agent dat hij NU moet stoppen met navigeren
        agent.velocity = Vector3.zero; // Haal direct alle resterende snelheid uit het lichaam
        agent.ResetPath();             // Wis de huidige route naar het slachtoffer, zodat hij niet blijft drukken
        
        // agent.enabled = false;     // <--- DEZE REGEL IS NU WEG. De agent blijft dus LIVE!
    }

    if (animator != null)
    {
        animator.SetFloat("Speed", 0f); 
        animator.Update(0f); 
    }
}

    void RotateTowardsVictim()
    {
        Vector3 direction = (victim.position - transform.position).normalized;
        direction.y = 0; 

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

// Dit is de functie die je manager of countdown script aanroept
    public void StartSurrounding()
    {
        // In plaats van direct te starten, starten we een Coroutine (tijdlijn)
        StartCoroutine(StartWithRandomDelay());
    }

    // Dit is de onzichtbare tijdlijn die de vertraging regelt
    private System.Collections.IEnumerator StartWithRandomDelay()
    {
        // Kies een willekeurige tijd tussen 0.0 en 2.0 seconden
        float randomDelay = Random.Range(0f, 2f);
        
        // Wacht het aantal gekozen seconden netjes af
        yield return new WaitForSeconds(randomDelay);

        // --- VANAF HIER START DE BEWEGING PAS ---
        shouldSurround = true;
        hasArrived = false;
        
        if (agent != null) 
        {
            // Zorg dat de agent weer leeft (als hij uit stond) en mag gaan lopen
            agent.enabled = true; 
            agent.isStopped = false;
            agent.speed = 3.5f; 
        }

        // De animator schiet nu pas aan, dus de ren-animatie start ook pas na de delay!
        if (animator != null)
        {
            animator.SetFloat("Speed", 3.5f);
        }
    }
}