using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour, IUpdateObserver
{
    //BAUARBEITEN
    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;
    [SerializeField] float sprintSpeed = 7f;
    [SerializeField] float smoothTime = 2f;
    bool grounded;
    [SerializeField] float distanceToTheGround = 1f;
    
    //BAUARBEITEN

    private Rigidbody _rigidbody;
    //um zu erfahren, ob Character dem Spieler gehoert oder nicht
    private PhotonView _photonView;
    [SerializeField] private Transform _camera;
    [SerializeField] private float _cameraSensivity = 2f;
    [SerializeField] private float _movementSpeed = 4f;
    [SerializeField] private float _jumpForce = 3f;
    private float _rotationX;

    PlayerManager playerManager;


    private void Awake()
    {
        UpdateManager.Instance.RegisterObserver(this);

        _rigidbody = GetComponent<Rigidbody>();
        _photonView = GetComponent<PhotonView>();

        //um Spieler zu respawnen (falls, bsp, er ausserhalb von Spielfeld ist)
        playerManager = PhotonView.Find((int) _photonView.InstantiationData[0]).GetComponent<PlayerManager>();
    }


    #region UpdateManager connection
    private void OnEnable()
    {
        UpdateManager.Instance.RegisterObserver(this);
        UpdateManager.Instance.RegisterObserverName("PlayerController");
    }
    private void OnDisable()
    {
        UpdateManager.Instance.UnregisterObserver(this);
        UpdateManager.Instance.UnregisterOberverName("PlayerController");
    }
    private void OnDestroy()
    {
        UpdateManager.Instance.UnregisterObserver(this);
        UpdateManager.Instance.UnregisterOberverName("PlayerController");
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if(!_photonView.IsMine)
        {
            //damit wir Blick nur von unseren Camera verwen koennen
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(_rigidbody);
        }
    }

    //Update laeuft jeden Frame und FixedUpdate nur nach bestimmten Zeit
    //PhysicLoop vs GameLoop googeln
    private void FixedUpdate()
    {
        if(_photonView.IsMine)
            _rigidbody.MovePosition(_rigidbody.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : _movementSpeed), ref smoothMoveVelocity, smoothTime);
    }

    public void ObservedUpdate()
    {
            if (!_photonView.IsMine)
            {
                return;
            }

        if(IngameMenuManager.GetCurrentMenu() == MenuType.None || IngameMenuManager.GetCurrentMenu() == MenuType.PlayerlistMenu)
        {


            Move();
            RotatePlayerLeftRight();
            RotateCameraUpDown();

            grounded = Physics.Raycast(transform.position, Vector3.down, distanceToTheGround + 0.1f);
            if(Input.GetButtonDown("Jump") && grounded)
            {
                _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            }
            _rigidbody.drag = 0f;
        }
        else
        {
            _rigidbody.drag = 3f;
            moveAmount = Vector3.Lerp(moveAmount, Vector3.zero, Time.deltaTime * 3f);
        }

        
            if (transform.position.y < -10f)
            {
                Respawn();
            }
    }

    private void RotatePlayerLeftRight()
    {
        transform.Rotate(Vector3.up, Input.GetAxisRaw("Mouse X") * _cameraSensivity);
    }

    private void RotateCameraUpDown()
    {
        _rotationX -= _cameraSensivity * Input.GetAxisRaw("Mouse Y");
        //Camera kann nicht 360 Grad sich drehen, sondern in einem bestimmten Bereich
        _rotationX = Mathf.Clamp(_rotationX, -75, 75);
        _camera.eulerAngles = new Vector3(_rotationX, _camera.eulerAngles.y, _camera.eulerAngles.z);
    }

    void Respawn()
    {
        playerManager.Respawn();
    }
}
