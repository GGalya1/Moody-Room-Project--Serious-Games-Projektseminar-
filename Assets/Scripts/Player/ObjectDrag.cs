using UnityEngine;

public class ObjectDrag : MonoBehaviour
{
    private Transform _target;

    public LayerMask targetMask;
    public LayerMask ignoreTargetMask;
    public float offsetFactor;

    private float _originalScale;
    private float _originalDistance;
    Vector3 targetScale;

    private void Update()
    {
        HandleInput();
        ResizeTarget();
    }

    public void HandleInput()
    {
        //falls wir linke Mouse druecken
        if (Input.GetMouseButtonDown(0))
        {
            //wir halten noch kein Objekt => wir nehmen ein
            if (_target == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, targetMask))
                {
                    _target = hit.transform;
                    _target.GetComponent<Rigidbody>().isKinematic = true;
                    //Distanz zwischen Kamera und Objekt
                    _originalDistance = Vector3.Distance(transform.position, _target.position);
                    _originalScale = _target.localScale.x;
                    targetScale = _target.localScale;
                }
            }
            else
            {
                _target.GetComponent<Rigidbody>().isKinematic = false;
                _target = null;
            }
        }
    }
    public void ResizeTarget()
    {
        //falls wir kein Objekt halten
        if (_target == null)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, ignoreTargetMask))
        {
            //damit Objekt nicht so stark in die Wand oder ein anderes Objekt geht (offset)
            _target.position = hit.point - transform.forward * offsetFactor * targetScale.x;

            float currentDistance = Vector3.Distance(transform.position, _target.position);
            //scaleFactor
            float s = currentDistance / _originalDistance;
            targetScale.x = targetScale.y = targetScale.z = s;

            _target.localScale = targetScale * _originalScale;
        }
    }
}