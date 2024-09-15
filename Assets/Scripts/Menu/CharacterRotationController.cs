using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterRotationController : MonoBehaviour
{
    public Slider angleSlider;
    private Transform characterTransform;

    public void Start()
    {
        characterTransform = transform;
        angleSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float angleValue)
    {
        characterTransform.rotation = Quaternion.Euler(0, angleValue, 0);
    }

    private void OnDestroy()
    {
        angleSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }
    
}
