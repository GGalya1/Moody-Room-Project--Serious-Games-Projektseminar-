using UnityEngine;

public class BarDoorOpening : MonoBehaviour
{
    public Animator doorAnimator;

    private void Start()
    {
        doorAnimator.SetBool("isOpen", false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))  
        {
            doorAnimator.SetBool("isOpen", true);  
        }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  
        {
            doorAnimator.SetBool("isOpen", false);  
        }
    }
}
