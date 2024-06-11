using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    //BAUARBEITEN
    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;
    [SerializeField] float sprintSpeed = 7f;
    [SerializeField] float smoothTime = 2f;
    bool grounded;
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
        _rigidbody = GetComponent<Rigidbody>();
        _photonView = GetComponent<PhotonView>();

        //um Spieler zu respawnen (falls, bsp, er ausserhalb von Spielfeld ist)
        playerManager = PhotonView.Find((int) _photonView.InstantiationData[0]).GetComponent<PlayerManager>();
    }
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
    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }
    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : _movementSpeed), ref smoothMoveVelocity, smoothTime);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            _rigidbody.AddForce(transform.up * _jumpForce);
        }
    }

    //BAUARBEITEN

    private void Update()
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

        if (!Pause.paused && !PhotonChatManager.chatTrigger)
        {


            Move();
            //Jump();
            //BAUARBEITEN
            RotatePlayerLeftRight();
            RotateCameraUpDown();

            if (Input.GetButtonDown("Jump"))
            {
                TryJump();
            }
        }

        
            if (transform.position.y < -10f)
            {
                Respawn();
            }
    }

    private void PlayerMovement()
    {
        float h = Input.GetAxis("Horizontal"); //a, d
        float v = Input.GetAxis("Vertical"); //w (+1), s (-1)

        Vector3 movementDir = transform.forward * v + transform.right * h;
        movementDir = Vector3.ClampMagnitude(movementDir, 1f);
        _rigidbody.velocity = new Vector3(movementDir.x * _movementSpeed, _rigidbody.velocity.y, movementDir.z * _movementSpeed);
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

    private void TryJump()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position - Vector3.down * 0.5f, _checkJumpRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != gameObject)
                return;
        }
        _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }

    void Respawn()
    {
        playerManager.Respawn();
    }
}
