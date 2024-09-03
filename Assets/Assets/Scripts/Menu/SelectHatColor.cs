/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectHatColor : MonoBehaviour
{
    public float redAmount;
    public float greenAmount;
    public float blueAmount;

    private Color currentHatColor;

    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;

    //bekommen Zugrif zu Material von Hat und danach wechseln die Farbe
    public List<SkinnedMeshRenderer> renderList = new List<SkinnedMeshRenderer>();

    public void UpdateSlieders()
    {
        redAmount = redSlider.value;
        greenAmount = greenSlider.value;
        blueAmount = blueSlider.value;
        SetHairColor();
    }

    public void SetHairColor()
    {
        currentHatColor = new Color(redAmount, greenAmount, blueAmount);

        for (int i = 0; i < renderList.Count; i++) {
            renderList[i].material.SetColor("_Color", currentHatColor);
        }
    }
}*/


using UnityEngine;
using UnityEngine.UI;

public class SelectHatColor : MonoBehaviour
{
    [SerializeField] private Slider redSlider;
    [SerializeField] private Slider greenSlider;
    [SerializeField] private Slider blueSlider;

    private Renderer[] renderers;

    void Start()
    {
        // Sammelt alle MeshRenderer im GameObject und dessen Kindern
        renderers = GetComponentsInChildren<MeshRenderer>();

        // Initialisiert die Schieberegler mit den aktuellen Farbwerten
        if (renderers.Length > 0 && renderers[0].material.HasProperty("_Color"))
        {
            Color initialColor = renderers[0].material.color;
            redSlider.value = initialColor.r;
            greenSlider.value = initialColor.g;
            blueSlider.value = initialColor.b;
        }

        // Fügt Listener zu den Schiebereglern hinzu
        redSlider.onValueChanged.AddListener(UpdateColor);
        greenSlider.onValueChanged.AddListener(UpdateColor);
        blueSlider.onValueChanged.AddListener(UpdateColor);
    }

    public void UpdateColor(float value)
    {
        // Holt die aktuellen Werte der Schieberegler
        float red = redSlider.value;
        float green = greenSlider.value;
        float blue = blueSlider.value;

        // Erstellt eine neue Farbe basierend auf den Schiebereglerwerten
        Color newColor = new Color(red, green, blue);

        // Aktualisiert die Farbe aller Materialien
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null && renderer.material.HasProperty("_Color"))
            {
                // Erstellt eine neue Material-Instanz, um das Originalmaterial nicht zu ändern
                Material newMaterial = new Material(renderer.material);
                newMaterial.color = newColor;

                // Setzt das neue Material auf den Renderer
                renderer.material = newMaterial;
            }
        }
    }
}