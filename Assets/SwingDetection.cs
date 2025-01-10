using Gamekit3D;
using Gamekit3D.Message;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class SwingDetection : MonoBehaviour
{
    public Rigidbody rb; // Reference to the Rigidbody component
    public float swingThreshold = 0.5f; // Threshold for detecting swing
    public float checkInterval = 0.1f; // Interval to check the velocity
    private Vector3 lastVelocity;
    private float timer;
    public bool swinging;
    public GameObject player;
    public PlayerController playerController;

    public AudioSource thwack;

    public Hand leftHand;
    public Hand rightHand;

    public bool heldInLeft;
    public bool heldInRight;

    public bool hasBeenPickedUp;

    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        lastVelocity = rb.velocity;
        timer = checkInterval;
    }

    public void SetWeaponHasBeenPickedUpToTrue()
    {
        //hasBeenPickedUp = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Collider other = contact.otherCollider;

            if (other.CompareTag("target") && swinging)
            {
                Damageable damageable = other.GetComponent<Damageable>();
                
                Damageable.DamageMessage data;

                data.amount = 1;
                data.damager = this;
                data.direction = this.transform.position;
                data.damageSource = player.transform.position;
                data.throwing = false;
                data.stopCamera = false;

                damageable.ApplyDamage(data);

                if (damageable.gameObject.name == "DestructibleBox")
                    break;

                float pitchRandomRange = 0.2f;
                thwack.pitch = Random.Range(1.0f - pitchRandomRange, 1.0f + pitchRandomRange);
                thwack.Play();
                Debug.Log("Collision detected with Target!");
                // Handle the collision logic here
                break; // Exit after handling the first target hit
            }
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            CheckForSwing();
            timer = checkInterval;
        }

        if (leftHand.currentAttachedObject != null)
        {
            heldInLeft = true;
        }
        else
        {
            heldInLeft = false;
        }

        if (rightHand.currentAttachedObject != null)
        {
            heldInRight = true;
        }
        else
        {
            heldInRight = false;
        }
    }

    void CheckForSwing()
    {
        Vector3 currentVelocity = rb.velocity;

        // Check the difference in velocity
        float velocityChange = Vector3.Distance(currentVelocity, lastVelocity);

        // Check if the change is above the threshold
        if (velocityChange > swingThreshold)
        {
            // Debug.Log("Object is swinging!");
            swinging = true;
        }
        else
        {
            swinging = false;
        }

        // Update the last velocity
        lastVelocity = currentVelocity;
    }
}
