using Gamekit3D;
using UnityEngine;
using Valve.VR;

public class Locomotion : MonoBehaviour
{
    public SteamVR_Action_Vector2 leftJoystick = SteamVR_Input.GetVector2Action("Locomotion");
    public SteamVR_Action_Boolean jumpButton = SteamVR_Input.GetBooleanAction("Jump");

    [SerializeField]
    float speedScale = 0.05f;

    public bool isJumping = false;
    public bool isGrounded = false;

    [SerializeField]
    private float jumpHeight;
    private float jumpForce = 0f;

    [SerializeField]
    private float gravity;
    [SerializeField]
    private float fallVelocityDrag;

    [SerializeField]
    private Rigidbody playerRigidbody;

    [SerializeField]
    private Transform camera;

    [SerializeField]
    LayerMask jumpLayers;

    public RandomAudioPlayer footstepPlayer;
    public RandomAudioPlayer jumpPlayer;
    public RandomAudioPlayer landedPlayer;

    private bool soundCanPlay = false;
    public float soundInterval = 2.0f; // Time between sound plays (in seconds)

    private float timer = 0f; // Timer to track time between sounds

    private Vector2 lateralMovement = Vector2.zero;

    public PlayerController controller;
    private void Start()
    {
        //controller = GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {

        MoveFromJoystick();

        if (!isJumping)
        {
            if (isGrounded)
            {
                CheckForJumpInput();
            }
            else
            {
                FallMovement();
            }
        }
        else if (isJumping)
        {
            AirMovement();
        }

        if (isGrounded)
        {

            //playerRigidbody.useGravity = true;
            playerRigidbody.AddForce(new Vector3(0, -1.0f, 0) * playerRigidbody.mass * 6.67f);
        }
        else
        {
            //playerRigidbody.useGravity = false;
            playerRigidbody.AddForce(new Vector3(0, 1.0f, 0) * playerRigidbody.mass * 6.67f);
        }

        SoundTimer();

        


    }

    private void SoundTimer()
    {
        soundCanPlay = false;
        timer += Time.deltaTime;

        // Check if the timer has reached the interval
        if (timer >= soundInterval)
        {
            // Play the sound
            soundCanPlay = true;

            // Reset the timer
            timer = 0f;
        }
        
    }

    private void CheckForJumpInput()
    {
        bool jumpPressed = jumpButton.GetChanged(SteamVR_Input_Sources.RightHand);

        if (jumpPressed)
        {
            StartJump();
        }
    }

    private void StartJump()
    {
        jumpPlayer.PlayRandomClip();
        isJumping = true;
        jumpForce = jumpHeight;
    }


    private void AirMovement()
    {

        Vector3 velocityMinusDrag = Vector3.zero;

        if (jumpForce > 0)
        {
            jumpForce -= (gravity * Time.fixedDeltaTime);
        }
        else
        {
            jumpForce -= (gravity / fallVelocityDrag * Time.fixedDeltaTime);
        }

        velocityMinusDrag.y = jumpForce;

        playerRigidbody.velocity += velocityMinusDrag;
    }

    private void FallMovement()
    {
        Vector3 velocityMinusDrag = Vector3.zero;

        jumpForce -= (gravity / fallVelocityDrag * Time.fixedDeltaTime);

        velocityMinusDrag.y = jumpForce;

        playerRigidbody.velocity += velocityMinusDrag;
    }

    private Vector3 MoveFromJoystick()
    {

        Vector2 joystickMovement = leftJoystick.GetAxis(SteamVR_Input_Sources.LeftHand);

        //print($"Joystick movement: {joystickMovement}");

        float forwardMagnitude = joystickMovement.y;
        float sidewaysMagnitude = joystickMovement.x;


        Quaternion cameraYRotation = Quaternion.Euler(0f, camera.localEulerAngles.y, 0f);

        Vector3 forwardMovement = cameraYRotation * playerRigidbody.transform.forward * forwardMagnitude;
        Vector3 horizontalMovement = cameraYRotation * playerRigidbody.transform.right * sidewaysMagnitude;

        // camera has 3 rotations, x y z , ignore x and z. multiply movement by y rotation 

        Vector3 movement = forwardMovement + horizontalMovement;

        lateralMovement = movement;

        // print($"movement magnitude: {movement.magnitude}");

        if (soundCanPlay)
        {
            PlayAudio();
        }



        //player.transform.position += movement;
        playerRigidbody.velocity += movement * Time.fixedDeltaTime * speedScale;

        return movement * Time.fixedDeltaTime * speedScale;
    }

    private void PlayAudio()
    {
        if (lateralMovement.magnitude > 0.01f && !footstepPlayer.playing && footstepPlayer.canPlay)
        {
            footstepPlayer.playing = true;
            footstepPlayer.canPlay = false;
            footstepPlayer.PlayRandomClip(controller.m_CurrentWalkingSurface, lateralMovement.magnitude < 1 ? 0 : 1);
        }
        else if (footstepPlayer.playing)
        {
            footstepPlayer.playing = false;
        }
        else if (lateralMovement.magnitude < 0.01f && !footstepPlayer.canPlay)
        {
            footstepPlayer.canPlay = true;
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if ((jumpLayers & (1 << other.gameObject.layer)) != 0)
        {
            // print($"Hit: {other.name}");

            if (isJumping)
            {
                landedPlayer.PlayRandomClip();
            }

            isJumping = false;
            jumpForce = 0f;

            isGrounded = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if ((jumpLayers & (1 << other.gameObject.layer)) != 0)
        {
            // print($"In: {other.name}");

            isGrounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((jumpLayers & (1 << other.gameObject.layer)) != 0)
        {
            print($"Left: {other.name}");
            isGrounded = false;
        }

    }
}
