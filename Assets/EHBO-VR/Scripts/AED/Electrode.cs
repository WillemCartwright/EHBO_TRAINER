using UnityEngine;

public class HandElectrode : MonoBehaviour
{
    // Vink dit aan bij de linker, en UIT bij de rechter plakker in de Inspector!
    [SerializeField] private bool isLinkerHand; 

    private void OnTriggerEnter(Collider other)
    {
        // Check of we de borstkas raken (met de juiste tag)
        if (other.CompareTag("Chest")) 
        {
            AEDInteraction aed = FindObjectOfType<AEDInteraction>();
            if (aed != null)
            {
                aed.PlakElektrode(isLinkerHand); 
            }
        }
    }
}