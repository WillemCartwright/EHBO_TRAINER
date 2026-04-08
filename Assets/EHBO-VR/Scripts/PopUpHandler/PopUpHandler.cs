using UnityEngine;

public class PopUpHandler : MonoBehaviour
{
    public void SluitPopUp()
    {
        gameObject.SetActive(false); // Verberg het scherm
        
        // Optioneel: Lock de muis weer als je een First Person controller gebruikt
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}