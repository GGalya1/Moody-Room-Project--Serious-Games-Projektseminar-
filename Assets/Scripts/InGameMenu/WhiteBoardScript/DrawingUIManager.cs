using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;

public class DrawingUIManager : MonoBehaviour
{
    public DrawingBoard drawingBoard;
    public Slider brushSizeSlider;
    public Button redColorButton;
    public Button blueColorButton;
    public Button blackColorButton;
    public Button eraseButton;
    public Button clearButton;

    //BAUARBEITEN wegen custom Farbe
    [SerializeField] private Slider redSlider;
    [SerializeField] private Slider greenSlider;
    [SerializeField] private Slider blueSlider;

    [SerializeField] private Button setCustomColor;
    //BAUARBEITEN wegen custom Farbe

    public float brushMaxSize = 40.0f;
    public float brushMinSize = 5.0f;

    [SerializeField] private GameObject container;

    //BAUARBEITEN wegen Synchronisieren zwischen allen Spielern
    public Button loadImageButton;
    public PhotonView photonView;
    internal byte[] imageData;
    public Texture2D image;
    public RawImage loadedImage;
    [SerializeField] private RectTransform rectTransform;

    //BAUARBEITEN wegen Zerlegen von Image in mehreren Dateien
    [SerializeField] private FileManager fileManager;
    private const int PacketSize = 300 * 1024; // 300 KB для каждого пакета
    private List<byte[]> imagePackets = new List<byte[]>();
    public void ShareImage()
    {
        imageData = image.EncodeToPNG();

        // Разбиваем изображение на пакеты по 300 KB
        int totalPackets = Mathf.CeilToInt((float)imageData.Length / PacketSize);
        for (int i = 0; i < totalPackets; i++)
        {
            int offset = i * PacketSize;
            int size = Mathf.Min(PacketSize, imageData.Length - offset);

            byte[] packet = new byte[size];
            System.Array.Copy(imageData, offset, packet, 0, size);

            photonView.RPC("RPC_ReceiveImagePacket", RpcTarget.All, packet, i, totalPackets);
        }
    }

    [PunRPC]
    private void RPC_ReceiveImagePacket(byte[] packet, int packetIndex, int totalPackets)
    {
        if (imagePackets.Count == 0)
        {
            // Инициализируем список для сборки всех пакетов
            imagePackets = new List<byte[]>(new byte[totalPackets][]);
        }

        // Сохраняем полученный пакет
        imagePackets[packetIndex] = packet;

        // Если все пакеты получены
        if (imagePackets.Count == totalPackets && imagePackets.All(p => p != null))
        {
            // Объединяем все пакеты в один массив байтов
            List<byte> fullImageData = new List<byte>();
            foreach (byte[] part in imagePackets)
            {
                fullImageData.AddRange(part);
            }

            // Преобразуем обратно в текстуру
            Texture2D texture = new Texture2D((int)rectTransform.rect.width, (int)rectTransform.rect.height, TextureFormat.RGBA32, false);
            texture.LoadImage(fullImageData.ToArray());
            
            //скалируем текстуру
            texture = ResizeTexture(texture, rectTransform.rect.width, rectTransform.rect.height);

            // Очистка
            imagePackets.Clear();

            //проверяем, загрузилась ли текстура в скрипт для рисования
            drawingBoard.SetImage(texture);

            //синхронизируем с большим бордом
            drawingBoard.SyncLargeBoardWithLittleBoard();
        }
    }

    public Texture2D ResizeTexture(Texture2D sourceTexture, float targetWidth, float targetHeight)
    {
        Texture2D resizedTexture = new Texture2D((int)targetWidth, (int)targetHeight, sourceTexture.format, false);

        // Масштабируем изображение с билинейной интерполяцией
        for (int x = 0; x < targetWidth; x++)
        {
            for (int y = 0; y < targetHeight; y++)
            {
                float u = (float)x / targetWidth;
                float v = (float)y / targetHeight;
                Color color = sourceTexture.GetPixelBilinear(u, v);
                resizedTexture.SetPixel(x, y, color);
            }
        }
        resizedTexture.Apply();
        return resizedTexture;
    }

    void Start()
    {
        brushSizeSlider.minValue = brushMinSize;
        brushSizeSlider.maxValue = brushMaxSize;
        brushSizeSlider.value = drawingBoard.brushSize;

        redSlider.maxValue = 255;
        redSlider.minValue = 0;
        redSlider.value = 64;
        greenSlider.maxValue = 255;
        greenSlider.minValue = 0;
        greenSlider.value = 224;
        blueSlider.maxValue = 255;
        blueSlider.minValue = 0;
        blueSlider.value = 208;

        brushSizeSlider.onValueChanged.AddListener(ChangeBrushSize);
        setCustomColor.onClick.AddListener(SetCustomColor);

        redColorButton.onClick.AddListener(() => drawingBoard.SetBrushColor(Color.red));
        blueColorButton.onClick.AddListener(() => drawingBoard.SetBrushColor(Color.blue));
        blackColorButton.onClick.AddListener(() => drawingBoard.SetBrushColor(Color.black));
        eraseButton.onClick.AddListener(() => drawingBoard.SetBrushColor(Color.white));
        clearButton.onClick.AddListener(drawingBoard.ClearTexture);
        loadImageButton.onClick.AddListener(fileManager.OpenFileBrowserForImagesSearch);
    }

    void ChangeBrushSize(float newSize)
    {
        drawingBoard.brushSize = newSize;
    }

    //passiert, wenn wir auf dem Knopf drucken
    public void SetCustomColor()
    {
        // Werte der Schieberegler holen und in einen Bereich von 0 bis 1 umrechnen
        float red = redSlider.value / 255f;
        float green = greenSlider.value / 255f;
        float blue = blueSlider.value / 255f;

        drawingBoard.brushColor = new Color(red, green, blue);
    }

    public RectTransform GetRectTransform()
    {
        return rectTransform;
    }
}

