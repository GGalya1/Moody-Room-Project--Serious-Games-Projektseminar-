using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float _checkJumpRadius = 0.2f;
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
    private void FixedUpdate()
    {
        //BAUARBEITEN
        if(_photonView.IsMine)
            _rigidbody.MovePosition(_rigidbody.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime * ( (Pause.paused || PhotonChatManager.chatTrigger) ? 0 : 1));
        //BAUARBEITEN

        /*
        if (_photonView.IsMine)
        {
            PlayerMovement();
        }
        */
    }

    //BAUARBEITEN
    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : _movementSpeed), ref smoothMoveVelocity, smoothTime);
    }

    //BAUARBEITEN

    public void ObservedUpdate()
    {
            if (!_photonView.IsMine)
            {
                return;
            }

        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(RoomManager.instance.gameObject);
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel(0);
        }*/

        //BAUARBEITEN

        if (ControlIsNotFrozen())
        {


            Move();
            //Jump();
            //BAUARBEITEN
            RotatePlayerLeftRight();
            RotateCameraUpDown();

            /*if (Input.GetButtonDown("Jump"))
            {
                TryJump();
            }*/
            grounded = Physics.Raycast(transform.position, Vector3.down, distanceToTheGround + 0.1f);
            if(Input.GetButtonDown("Jump") && grounded)
            {
                _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            }
        }

        
            if (transform.position.y < -10f)
            {
                Respawn();
            }
    }
    private bool ControlIsNotFrozen()
    {
        return !Pause.paused && !PhotonChatManager.chatTrigger && !AdminPanelScript.adminPanelIsOn && !DrawingUIManager.whiteboardOn;
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
