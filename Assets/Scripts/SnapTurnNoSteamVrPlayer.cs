// Copyright (c) Valve Corporation, All rights reserved. ======================================================================================================



using UnityEngine;
using System.Collections;
using Valve.VR;

public class SnapTurnNoSteamVrPlayer : MonoBehaviour
{
    [SerializeField]
    Transform player;

    public float snapAngle = 90.0f;

    public bool showTurnAnimation = true;

    public AudioSource snapTurnSource;
    public AudioClip rotateSound;

    public GameObject rotateRightFX;
    public GameObject rotateLeftFX;

    public SteamVR_Action_Boolean snapLeftAction = SteamVR_Input.GetBooleanAction("SnapTurnLeft");
    public SteamVR_Action_Boolean snapRightAction = SteamVR_Input.GetBooleanAction("SnapTurnRight");

    public bool fadeScreen = true;
    public float fadeTime = 0.1f;
    public Color screenFadeColor = Color.black;

    public float distanceFromFace = 1.3f;
    public Vector3 additionalOffset = new Vector3(0, -0.3f, 0);

    public static float teleportLastActiveTime;

    private bool canRotate = true;

    public float canTurnEverySeconds = 0.4f;


    private void Start()
    {
        AllOff();
    }

    private void AllOff()
    {
        if (rotateLeftFX != null)
            rotateLeftFX.SetActive(false);

        if (rotateRightFX != null)
            rotateRightFX.SetActive(false);
    }


    private void Update()
    {


        if (canRotate && snapLeftAction != null && snapRightAction != null && snapLeftAction.activeBinding && snapRightAction.activeBinding)
        {
            //only allow snap turning after a quarter second after the last teleport
            if (Time.time < (teleportLastActiveTime + canTurnEverySeconds))
                return;


            //bool leftHandTurnLeft = snapLeftAction.GetStateDown(SteamVR_Input_Sources.LeftHand);
            bool rightHandTurnLeft = snapLeftAction.GetStateDown(SteamVR_Input_Sources.RightHand);

            //bool leftHandTurnRight = snapRightAction.GetStateDown(SteamVR_Input_Sources.LeftHand);
            bool rightHandTurnRight = snapRightAction.GetStateDown(SteamVR_Input_Sources.RightHand);

            if (rightHandTurnLeft)
            {
                RotatePlayer(-snapAngle);
            }
            else if (rightHandTurnRight)
            {
                RotatePlayer(snapAngle);
            }
        }
    }


    private Coroutine rotateCoroutine;
    public void RotatePlayer(float angle)
    {
        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
            AllOff();
        }

        rotateCoroutine = StartCoroutine(DoRotatePlayer(angle));
    }

    //-----------------------------------------------------
    private IEnumerator DoRotatePlayer(float angle)
    {
        canRotate = false;

        snapTurnSource.panStereo = angle / 90;
        snapTurnSource.PlayOneShot(rotateSound);

        if (fadeScreen)
        {
            SteamVR_Fade.Start(Color.clear, 0);

            Color tColor = screenFadeColor;
            tColor = tColor.linear * 0.6f;
            SteamVR_Fade.Start(tColor, fadeTime);
        }

        yield return new WaitForSeconds(fadeTime);


        player.transform.Rotate(Vector3.up, angle);


        GameObject fx = angle > 0 ? rotateRightFX : rotateLeftFX;

        if (showTurnAnimation)
            ShowRotateFX(fx);

        if (fadeScreen)
        {
            SteamVR_Fade.Start(Color.clear, fadeTime);
        }

        float startTime = Time.time;
        float endTime = startTime + canTurnEverySeconds;

        while (Time.time <= endTime)
        {
            yield return null;
            UpdateOrientation(fx);
        };

        fx.SetActive(false);
        canRotate = true;
    }

    void ShowRotateFX(GameObject fx)
    {
        if (fx == null)
            return;

        fx.SetActive(false);

        UpdateOrientation(fx);

        fx.SetActive(true);

        UpdateOrientation(fx);
    }

    private void UpdateOrientation(GameObject fx)
    {

        //position fx in front of face
        this.transform.position = player.position + (player.forward * distanceFromFace);
        this.transform.rotation = Quaternion.LookRotation(player.position - this.transform.position, Vector3.up);
        this.transform.Translate(additionalOffset, Space.Self);
        this.transform.rotation = Quaternion.LookRotation(player.position - this.transform.position, Vector3.up);
    }
}