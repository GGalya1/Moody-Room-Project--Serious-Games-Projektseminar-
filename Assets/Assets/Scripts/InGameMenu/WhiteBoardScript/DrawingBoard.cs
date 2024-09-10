using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DrawingBoard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Color brushColor = Color.black;
    public float brushSize = 5.0f;

    private Texture2D texture;
    private RectTransform rectTransform;
    private bool isDrawing = false;

    Vector2 previousPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        texture = new Texture2D((int)rectTransform.rect.width, (int)rectTransform.rect.height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;

        ClearTexture();
        GetComponent<RawImage>().texture = texture;
    }

    public void ClearTexture()
    {
        Color32[] colors = new Color32[texture.width * texture.height];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = Color.white;
        texture.SetPixels32(colors);
        texture.Apply();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDrawing = true;
        Draw(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDrawing = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDrawing)
            Draw(eventData.position);
    }

    void Draw(Vector2 screenPosition)
    {
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPosition, null, out localPosition);

        int x = (int)(localPosition.x + rectTransform.rect.width / 2);
        int y = (int)(localPosition.y + rectTransform.rect.height / 2);

        for (int i = -Mathf.CeilToInt(brushSize / 2); i < Mathf.CeilToInt(brushSize / 2); i++)
        {
            for (int j = -Mathf.CeilToInt(brushSize / 2); j < Mathf.CeilToInt(brushSize / 2); j++)
            {
                texture.SetPixel(x + i, y + j, brushColor);
            }
        }

        texture.Apply();
    }
    /*void Draw(Vector2 screenPosition)
    {
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPosition, null, out localPosition);

        int x = (int)(localPosition.x + rectTransform.rect.width / 2);
        int y = (int)(localPosition.y + rectTransform.rect.height / 2);

        DrawBrush(x, y);

        if (previousPosition != null)
        {
            Interpolate(previousPosition, new Vector2(x, y));
        }

        previousPosition = new Vector2(x, y);
    }

    void Interpolate(Vector2 start, Vector2 end)
    {
        float distance = Vector2.Distance(start, end);
        Vector2 direction = (end - start).normalized;

        for (float i = 0; i < distance; i += 0.5f)
        {
            Vector2 interpolatedPosition = start + direction * i;
            DrawBrush((int)interpolatedPosition.x, (int)interpolatedPosition.y);
        }
    }

    void DrawBrush(int x, int y)
    {
        for (int i = -Mathf.CeilToInt(brushSize / 2); i < Mathf.CeilToInt(brushSize / 2); i++)
        {
            for (int j = -Mathf.CeilToInt(brushSize / 2); j < Mathf.CeilToInt(brushSize / 2); j++)
            {
                texture.SetPixel(x + i, y + j, brushColor);
            }
        }

        texture.Apply();
    }*/


    public void SetBrushColor(Color color)
    {
        brushColor = color;
    }

    public void Erase()
    {
        SetBrushColor(Color.white);
    }
}

