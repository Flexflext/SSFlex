using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    // This script is responsible for:
    // - Change of Speed
    // - Walking
    // - Running
    // - Sneaking
    // - Jumping
    // - Gravity
    // - Movement Animations
    // - Movement Audio

    [Header("References")]

    // Layers
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask stoneGroundedMask;
    [SerializeField] private LayerMask gravelGroundedMask;

    // Photon
    [SerializeField] private GameObject cameraHolder;
    [SerializeField] private GameObject firstPersonShotgun;
    [SerializeField] private GameObject firstPersonAR;
    [SerializeField] private GameObject firstPersonSniper;
    [SerializeField] private GameObject firstPersonPistol;
    [SerializeField] private GameObject firstPersonMesh;
    [SerializeField] private GameObject thirdPersonShotgun;
    [SerializeField] private GameObject thirdPersonAR;
    [SerializeField] private GameObject thirdPersonSniper;
    [SerializeField] private GameObject thirdPersonPistol;
    [SerializeField] private GameObject thirdPersonMesh;
    [SerializeField] private GameObject thirdPersonPlayer;

    [SerializeField] private Animator thirdPersonAnimator;

    [Header("Testing Stats")]
    [SerializeField] private Vector3 moveDir;

    [SerializeField] private float currentPlayerSpeed, playerWalkingSpeed, playerRunSpeed, playerSneakingSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float groundDrag;
    [SerializeField] private float airDrag;
    [SerializeField] private bool isOnStone, isOnGravel;

    // Non-visible References
    private PlayerShooting shooting;
    private Rigidbody rb;
    private Animator fpsAnimator;
    private PhotonView photonView;
    private PlayerShooting playerShooting;

    // Non-visible Stats
    // Movement
    private float movementMultiplier = 1;
    public float MovementMultiplier { get { return movementMultiplier; } set { movementMultiplier = Mathf.Clamp01(value); } }

   

    private bool isMoving, isRunning, isSneaking;

    private Vector2 input;

    // Jump
    private bool isGrounded, isStoneGrounded, isGravelGrounded;
    private bool useGravity = true;

    private bool isStoneWalking, isStoneRunning, isGravelWalking, isGravelRunning;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        shooting = GetComponent<PlayerShooting>();
        fpsAnimator = GetComponentInChildren<Animator>();
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        // Visualization First Person View: Oneself
        // Own player body - Only arms and weapon.
        // Other player bodies - Whole 3rdPerson body with animations.
        if (photonView.IsMine)
        {
            Destroy(thirdPersonMesh.gameObject);
            Destroy(thirdPersonShotgun);
            Destroy(thirdPersonAR);
            Destroy(thirdPersonSniper);
            Destroy(thirdPersonPistol);
            thirdPersonPlayer.SetActive(false);
        }

        // To seperate the camera control and the rigidbody of multiple players.
        // Visualization First Person View: The other clients.
        // + FPS Body and shotgun on other players are not visible for own player.
        if (!photonView.IsMine)
        {
            GetComponent<Collider>().enabled = false;
            Destroy(cameraHolder.gameObject);
            Destroy(firstPersonMesh.gameObject);
            Destroy(firstPersonShotgun);
            Destroy(firstPersonAR);
            Destroy(firstPersonSniper);
            Destroy(firstPersonPistol);
        }

        currentPlayerSpeed = playerWalkingSpeed;
    }

    private void Update()
    {
        // To seperate the controls of multiple players.
        if (!photonView.IsMine)
        {
            Debug.Log("Player kinda works");
            return;
        }

        // Movement
        PlayerMoving();
        PlayerMovingAnimator();
        PlayerRun();
        PlayerSneak();
        PlayerAimMovement();
        PlayerJump();


        // Audio
        AudioMixing();
        
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            Debug.Log("Player kinda works");
            return;
        }


        rb.velocity = new Vector3(moveDir.normalized.x * currentPlayerSpeed * Time.fixedDeltaTime, rb.velocity.y, moveDir.normalized.z * currentPlayerSpeed * Time.fixedDeltaTime);

        rb.useGravity = false;
        if (useGravity)
        {
            Debug.Log("using grav");
            rb.AddForce(Physics.gravity * (rb.mass * rb.mass));
        }

    }

    #region Movement
    private void PlayerMoving()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveDir = transform.forward * vertical + transform.right * horizontal;
        


        if (moveDir.magnitude >= 0.1f)
        {
            isMoving = true;

            if (isRunning)
            {
                fpsAnimator.SetFloat("Velocity", 1f);
            }
            else
            {
                fpsAnimator.SetFloat("Velocity", 0.5f);
            }

        }
        else
        {
            isMoving = false;
            moveDir = new Vector3(0, rb.velocity.y, 0);

            fpsAnimator.SetFloat("Velocity", 0);
        }
    }

    private void PlayerMovingAnimator()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        thirdPersonAnimator.SetFloat("InputX", input.x);
        thirdPersonAnimator.SetFloat("InputY", input.y);
    }

    private void PlayerRun()
    {
        if (Input.GetKey(KeyCode.LeftShift) && isMoving == true && !shooting.ImAiming)
        {
            isRunning = true;
            thirdPersonAnimator.SetBool("isRunning", true);
            shooting.InterruptReload();
            currentPlayerSpeed = playerRunSpeed;
        }
        else
        {
            currentPlayerSpeed = playerWalkingSpeed * movementMultiplier;
            isRunning = false;
            thirdPersonAnimator.SetBool("isRunning", false);
        }
    }


    private void PlayerSneak()
    {
        if (Input.GetKey(KeyCode.LeftControl) && isMoving == true)
        {
            currentPlayerSpeed = playerSneakingSpeed;
            thirdPersonAnimator.SetBool("isSneaking", true);
            isSneaking = true;
        }
        else
        {
            thirdPersonAnimator.SetBool("isSneaking", false);
            isSneaking = false;
        }
    }

    private void PlayerAimMovement()
    {
        if (isMoving && Input.GetKey(KeyCode.Mouse1))
        {
            thirdPersonAnimator.SetBool("isSneaking", true);
        }
        else
        {
            thirdPersonAnimator.SetBool("isSneaking", false);
        }

        if (!isMoving && Input.GetKey(KeyCode.Mouse1))
        {
            thirdPersonAnimator.SetBool("isAiming", true);
        }
        else
        {
            thirdPersonAnimator.SetBool("isAiming", false);
        }
    }

    private void PlayerJump()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            StartCoroutine(JumpAnimatorTimer());
            rb.AddForce(new Vector3(0, jumpForce));
        }
    }

   

    public void SetIsGroundedState(bool _isGrounded)
    {
        isGrounded = _isGrounded;
    }

    public void SetIsStoneGroundedState(bool _isStoneGrounded)
    {
        isStoneGrounded = _isStoneGrounded;
    }
    public void SetIsGravelGroundedState(bool _isGravelGrounded)
    {
        isGravelGrounded = _isGravelGrounded;
    }
    #endregion

    #region Audio
    //public void DisplayAudioMixing()
    //{
    //    photonView.RPC("DisplayTeam", RpcTarget.AllBufferedViaServer);
    //}


    
    private void AudioMixing()
    {
        


        if (isStoneGrounded && !isGravelGrounded)
        {
            Debug.Log("isonstone");
            isOnStone = true;

            if (isMoving && !isStoneRunning)
            {
                isStoneWalking = true;
                AudioManager.Instance.PlayRandom("StoneWalk", 3);
            }

            if (isRunning && !isStoneWalking)
            {
                isStoneRunning = true;
                AudioManager.Instance.PlayRandom("StoneRun", 4);
                AudioManager.Instance.Play("BreathingRun");
            }
        }
        else
        {
            isOnStone = false;
        }
        
        if (isGravelGrounded && !isStoneGrounded)
        {
            isOnGravel = true;

            if (isMoving && !isGravelRunning)
            {
                isGravelWalking = true;
                AudioManager.Instance.PlayRandom("GravelWalk", 1);
            }

            if (isRunning && !isGravelWalking)
            {
                isGravelRunning = true;
                AudioManager.Instance.PlayRandom("GravelRun", 2);
                AudioManager.Instance.Play("BreathingRun");
            }
        }
        else
        {
            isOnGravel = false;
        }


        if (!isOnStone)
        {
            AudioManager.Instance.Stop("StoneWalk");
            AudioManager.Instance.Stop("StoneRun");
            AudioManager.Instance.Stop("BreathingRun");
        }
        if (!isOnGravel)
        {
            AudioManager.Instance.Stop("GravelWalk");
            AudioManager.Instance.Stop("GravelRun");
            AudioManager.Instance.Stop("BreathingRun");
        }
        if (!isMoving || isSneaking)
        {
            AudioManager.Instance.Stop("GravelWalk");
            AudioManager.Instance.Stop("GravelRun");
            AudioManager.Instance.Stop("StoneWalk");
            AudioManager.Instance.Stop("StoneRun");
            AudioManager.Instance.Stop("BreathingRun");
        }

    }

    //[PunRPC]
    //public void SyncAudio(string _audioName)
    //{
    //    AudioManager.Instance.Play(_audioName);
    //}
    #endregion 

    IEnumerator JumpAnimatorTimer()
    {
        thirdPersonAnimator.SetBool("isJumping", true);
        yield return new WaitForSeconds(0.8f);
        thirdPersonAnimator.SetBool("isJumping", false);
    }


    
}
