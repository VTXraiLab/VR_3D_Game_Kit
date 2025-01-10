using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpaceshipMovement : MonoBehaviour
{
    public float upwardDistance = 10f; // Distance to move upwards
    public float forwardDistance = 20f; // Distance to move forward
    public float speed = 5f; // Movement speed
    public int timeBeforeLaunch = 20;

    private Vector3 targetPosition; // Target position for the spaceship
    private bool movingUp = true; // Flag for upward movement
    private bool movingForward = false; // Flag for forward movement
    public GameObject dropship;
    public GameObject player; // The VR rig or player object
    public Transform marker; // Marker for additional movement or positioning logic
    public bool countdownExpired = false;
    private bool opened = false;

    public EndGameScreenFade screenFaderCanvas;

    public Text legacyTextBox;  // Reference to the legacy text box
    public float typingSpeed = 0.05f;  // Speed of typing (time between each letter)
    public float messageDelay = 2f;   // Delay before clearing the text
    private Coroutine typingCoroutine; // To handle coroutine instance

    public IEnumerator count(Action function, int seconds, bool displayDebugText = true)
    {
        for (int i = seconds; i >= 0; i--)
        {
            if (displayDebugText)
                Debug.Log("Count: " + i); // Print the current number to the console
            yield return new WaitForSeconds(1); // Wait for 1 second
        }
        dropship.transform.rotation = Quaternion.Euler(0, 245, 0);
        function.Invoke();
    }

    public IEnumerator StartScreenFader()
    {
        screenFaderCanvas.fadeOut = true;
        yield return new WaitForSeconds(3);
        QuitGame();
    }
    public void QuitGame()
    {
        // save any game data here
        #if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }


    public IEnumerator setShip()
    {
        screenFaderCanvas.fadeOut = true;
        yield return new WaitForSeconds(3);
        targetPosition = dropship.transform.position + new Vector3(0, upwardDistance, 0);
        player.transform.position = marker.position;
        player.transform.rotation = marker.rotation;
        // Parent the player to the spaceship
        player.transform.SetParent(dropship.transform);
        Rigidbody rb = player.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePosition;
        yield return new WaitForSeconds(1);
        screenFaderCanvas.FadeFromBlack();
        yield return new WaitForSeconds(1);
        string[] sentences = {
            "Congrats You Made It to the End",
            "Or As Some Might Say, The Beginning",
            "This Port Was Created By the XRAI Lab at Virginia Tech",
            "Credit to Dr. Ryan McMahan, Christian Cassell, Christopher Lee, and Nayan Chawla"
        };
        StartTyping(sentences);
        countdownExpired = true;
    }

    

    public void StartTyping(string[] sentences)
    {
        // If another typing coroutine is running, stop it
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeMultipleMessages(sentences));
    }

    private IEnumerator TypeMultipleMessages(string[] sentences)
    {
        foreach (string sentence in sentences)
        {
            yield return StartCoroutine(TypeMessage(sentence));
            yield return new WaitForSeconds(messageDelay); // Wait after typing each sentence
        }

        // Optional: Clear the text box after finishing all sentences
        legacyTextBox.text = "";
    }

    private IEnumerator TypeMessage(string message)
    {
        legacyTextBox.text = ""; // Clear the text box
        foreach (char letter in message.ToCharArray())
        {
            legacyTextBox.text += letter; // Add one letter at a time
            yield return new WaitForSeconds(typingSpeed); // Wait before typing the next letter
        }
    }

    [System.Obsolete]
    void Update()
    {
        if (dropship.active && !opened)
        {
            opened = true;
            StartCoroutine(count(() => StartCoroutine(setShip()), timeBeforeLaunch));
            StartCoroutine(count(() => StartCoroutine(StartScreenFader()), timeBeforeLaunch + 25, false));

        }
        if (countdownExpired) { 
            // Move the spaceship towards the target position
            dropship.transform.position = Vector3.MoveTowards(dropship.transform.position, targetPosition, speed * Time.deltaTime);

            // Check if the spaceship has reached the upward target
            if (movingUp && Vector3.Distance(dropship.transform.position, targetPosition) < 0.1f)
            {
                movingUp = false;
                movingForward = true;
                // Set the target position to move forward
                targetPosition = dropship.transform.position + new Vector3(-forwardDistance, 0, 0);
            }

            // Check if the spaceship has reached the forward target
            if (movingForward && Vector3.Distance(dropship.transform.position, targetPosition) < 0.1f)
            {
                movingForward = false;
                // Movement sequence complete
                Debug.Log("Spaceship movement complete!");
            }

        }
    }
}
