using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class DrawingUIManager : MonoBehaviour
{
    public DrawingBoard drawingBoard;
    public Slider brushSizeSlider;
    public Button redColorButton;
    public Button blueColorButton;
    public Button blackColorButton;
    public Button eraseButton;
    public Button clearButton;

    //BAUARBEITEN wegen Synchronisieren zwischen allen Spielern
    public Button loadImageButton;
    public PhotonView photonView;
    internal byte[] imageData;
    public Texture2D image;
    public RawImage loadedImage;

    public void ShareImage()
    {
        imageData = image.EncodeToPNG();
        photonView.RPC("RPC_ShareImage", RpcTarget.All, imageData);
    }

    [PunRPC]
    private void RPC_ShareImage(byte[] imageData)
    {
        Texture2D texture = new Texture2D(10, 10);
        texture.LoadImage(imageData);
        loadedImage.texture = texture;
    }
    //BAUARBEITEN

    public float brushMaxSize = 40.0f;
    public float brushMinSize = 5.0f;

    [SerializeField] private GameObject container;

    void Start()
    {
        brushSizeSlider.minValue = brushMinSize;
        brushSizeSlider.maxValue = brushMaxSize;
        brushSizeSlider.value = brushMinSize;

        brushSizeSlider.onValueChanged.AddListener(ChangeBrushSize);
        redColorButton.onClick.AddListener(() => drawingBoard.SetBrushColor(Color.red));
        blueColorButton.onClick.AddListener(() => drawingBoard.SetBrushColor(Color.blue));
        blackColorButton.onClick.AddListener(() => drawingBoard.SetBrushColor(Color.black));
        eraseButton.onClick.AddListener(drawingBoard.Erase);
        clearButton.onClick.AddListener(drawingBoard.ClearTexture);
        loadImageButton.onClick.AddListener(ShareImage);
    }

    void ChangeBrushSize(float newSize)
    {
        drawingBoard.brushSize = newSize;
    }
}

