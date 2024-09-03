using UnityEngine;
using UnityEngine.UI;

public class DrawingUIManager : MonoBehaviour, IUpdateObserver
{
    public DrawingBoard drawingBoard;
    public Slider brushSizeSlider;
    public Button redColorButton;
    public Button blueColorButton;
    public Button blackColorButton;
    public Button eraseButton;
    public Button clearButton;

    public float brushMaxSize = 40.0f;
    public float brushMinSize = 5.0f;

    public static bool whiteboardOn = false;
    [SerializeField] private GameObject container;

    #region UpdateManager connection
    private void Awake()
    {
        UpdateManager.Instance.RegisterObserver(this);
    }
    private void OnEnable()
    {
        UpdateManager.Instance.RegisterObserver(this);
    }
    private void OnDisable()
    {
        UpdateManager.Instance.UnregisterObserver(this);
    }
    private void OnDestroy()
    {
        UpdateManager.Instance.UnregisterObserver(this);
    }
    #endregion

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
    }

    public void ObservedUpdate()
    {
        if (whiteboardCanBeOpened())
        {
            whiteboardOn = !whiteboardOn;
        }
        if (whiteboardOn)
        {
            container.SetActive(true);
        }
        else
        {
            container.SetActive(false);
        }
        //hier gibt es kein "else"-Block, weil das ganze Umgehen mit Cursor und "visible"/ "not visible" 
        //in PhotonChatManager realisiert ist
    }
    private bool whiteboardCanBeOpened()
    {
        return !Pause.paused &&
            !AdminPanelScript.adminPanelIsOn &&
            Input.GetKeyDown(KeyCode.RightAlt);
    }

    void ChangeBrushSize(float newSize)
    {
        drawingBoard.brushSize = newSize;
    }
}

