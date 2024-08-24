using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using System.Collections;
using Photon.Realtime;

public class DrawingBoard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Color brushColor = Color.black;
    public float brushSize = 5.0f;

    private Texture2D texture;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RawImage rawImage;
    private bool isDrawing = false;

    PhotonView _photonView;

    void Start()
    {
        _photonView = GetComponent<PhotonView>();
        texture = new Texture2D((int)rectTransform.rect.width, (int)rectTransform.rect.height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;

        rawImage.texture = texture;

        // Starte Coroutine, um auf die vollstaendige Verbindung zu warten und dann die RPC-Anfrage zu senden
        //diese Anfrage verlangt von MasterClient den aktuellen Stand von Whiteboard
        StartCoroutine(WaitForConnectionAndRequestData());
    }
    IEnumerator WaitForConnectionAndRequestData()
    {
        // Warte, bis der Spieler mit dem Raum verbunden ist

        while (!PhotonNetwork.InRoom || !PhotonNetwork.IsConnectedAndReady)
        {
            yield return null;  // Wartet einen Frame
        }
        //zur Sichercheit. Spaeter kann geloescht werden
        while (PhotonNetwork.NetworkClientState != ClientState.Joined)
        {
            yield return null;
        }

        // Sobald der Spieler verbunden ist und nicht der MasterClient, fordere die aktuelle Textur vom MasterClient an
        if (!PhotonNetwork.IsMasterClient)
        {
            _photonView.RPC("RequestTextureData", RpcTarget.MasterClient);
        }
        //jetzt ursprunglich ist leider Texture nicht weiss. Mit dieser Zeile faerben wir das
        else
        {
            _photonView.RPC("ClearTextureForAll", RpcTarget.All);
        }
    }

    public void ClearTexture()
    {
        _photonView.RPC("ClearTextureForAll", RpcTarget.All);
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

        _photonView.RPC("DrawForEveryone", RpcTarget.All, x, y, brushSize, brushColor.r, brushColor.g, brushColor.b);
    }

    public void SetBrushColor(Color color)
    {
        //"Erase" wird realisiert, indem Color.white gesetzt wird
        brushColor = color;

    }

    #region PunRPC section

    [PunRPC]
    void DrawForEveryone(int x, int y, float brushSize, float r, float g, float b)
    {
        Color _brushColor = new Color(r, g, b);
        for (int i = -Mathf.CeilToInt(brushSize / 2); i < Mathf.CeilToInt(brushSize / 2); i++)
        {
            for (int j = -Mathf.CeilToInt(brushSize / 2); j < Mathf.CeilToInt(brushSize / 2); j++)
            {
                texture.SetPixel(x + i, y + j, _brushColor);
            }
        }
        texture.Apply();
    }

    [PunRPC]
    public void ClearTextureForAll()
    {
        Color32[] colors = new Color32[texture.width * texture.height];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = Color.white;
        texture.SetPixels32(colors);
        texture.Apply();
    }

    [PunRPC]
    void RequestTextureData()
    {
        byte[] textureData = texture.EncodeToPNG();
        _photonView.RPC("ReceiveTextureData", RpcTarget.Others, textureData);
    }

    [PunRPC]
    void ReceiveTextureData(byte[] data)
    {
        Texture2D receivedTexture = new Texture2D((int)rectTransform.rect.width, (int)rectTransform.rect.height, TextureFormat.RGBA32, false);
        receivedTexture.LoadImage(data);
        receivedTexture.filterMode = FilterMode.Point;
        rawImage.texture = receivedTexture;
        texture = receivedTexture;
    }
    #endregion
}

