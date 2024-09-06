using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.OnButtonClick?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.OnButtonHover?.Invoke();
    }
}
