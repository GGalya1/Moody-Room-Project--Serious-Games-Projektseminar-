using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    public bool grounded;

    private void OnCollisionEnter(Collision collision)
    {
        grounded = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        grounded = false;
    }
}
