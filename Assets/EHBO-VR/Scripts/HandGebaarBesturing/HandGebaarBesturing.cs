using UnityEngine;

public class HandGebaarBesturing : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform vrEyes; // Sleep hier de 'CenterEyeAnchor' in
    [SerializeField] private Transform playerOrigin; // Sleep hier '[BuildingBlock] Camera Rig' in
    
    [Header("Instellingen")]
    [SerializeField] private float loopSnelheid = 1.5f;

    private bool isWijsGebaarActief = false;

    // Deze functie roep je dadelijk aan via je OVR Hand Gesture Block (On Gesture Started)
    public void StartLopen()
    {
        isWijsGebaarActief = true;
        Debug.Log("[GEBAAR] Speler start met lopen via handgebaar.");
    }

    // Deze functie roep je aan via OVR Hand Gesture Block (On Gesture Ended)
    public void StopLopen()
    {
        isWijsGebaarActief = false;
        Debug.Log("[GEBAAR] Speler stopt met lopen.");
    }

    void Update()
    {
        if (isWijsGebaarActief && playerOrigin != null && vrEyes != null)
        {
            // Bereken de kijkrichting van de speler (plat op de grond, dus Y = 0)
            Vector3 richting = vrEyes.forward;
            richting.y = 0;
            richting.Normalize();

            // Verplaats de speler direct via zijn Transform, passend bij de Locomotor
            playerOrigin.position += richting * loopSnelheid * Time.deltaTime;
        }
    }
}