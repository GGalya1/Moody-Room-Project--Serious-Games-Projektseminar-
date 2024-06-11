using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetCustomization : MonoBehaviour
{
    public Slider angleSlider;

    public void ResetAngleOfView()
    {
        angleSlider.value = 190;
    }
}
