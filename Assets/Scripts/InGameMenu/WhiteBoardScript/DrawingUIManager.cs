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

    //BAUARBEITEN wegen Synchronisieren zwischen allen Spielern
    public Button loadImageButton;
    public PhotonView photonView;
    internal byte[] imageData;
    public Texture2D image;
    public RawImage loadedImage;

    //BAUARBEITEN wegen Zerlegen von Image in mehreren Dateien
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
            Texture2D texture = new Texture2D(10, 10);
            texture.LoadImage(fullImageData.ToArray());
            loadedImage.texture = texture;

            // Очистка
            imagePackets.Clear();
        }
    }

    //BAUARBEITEN 

    /*public void ShareImage()
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
    }*/
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
        eraseButton.onClick.AddListener(() => drawingBoard.SetBrushColor(Color.white));
        clearButton.onClick.AddListener(drawingBoard.ClearTexture);
        loadImageButton.onClick.AddListener(ShareImage);
    }

    void ChangeBrushSize(float newSize)
    {
        drawingBoard.brushSize = newSize;
    }
}

