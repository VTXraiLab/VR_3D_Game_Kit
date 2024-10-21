using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

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
    LayerMask jumpableLayers;


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

        //player.transform.position += movement;
        playerRigidbody.velocity += movement * Time.fixedDeltaTime * speedScale;

        return movement * Time.fixedDeltaTime * speedScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((jumpableLayers & (1 << other.gameObject.layer)) != 0)
        {
            print($"Hit: {other.name}");
            isJumping = false;
            jumpForce = 0f;

            isGrounded = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if ((jumpableLayers & (1 << other.gameObject.layer)) != 0)
        {
            // print($"In: {other.name}");

            isGrounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((jumpableLayers & (1 << other.gameObject.layer)) != 0)
        {
            print($"Left: {other.name}");
            isGrounded = false;
        }
        
    }
}
