using UnityEngine;

public class BarDoorOpening : MonoBehaviour
{
    public Animator doorAnimator;

    private void OnTriggerEnter(Collider other)
    {
       
        doorAnimator.SetBool("isOpen", true);
        
    }

    private void OnTriggerExit(Collider other)
    {
        
        doorAnimator.SetBool("isOpen", false);
        
    }
}
