using Gamekit3D;
using Gamekit3D.Message;
using UnityEngine;

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

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Collider other = contact.otherCollider;

            if (other.CompareTag("target"))
            {
                Damageable damageable = other.GetComponent<Damageable>();
                damageable.OnDeath.Invoke();
                Damageable.DamageMessage data;

                data.amount = 5;
                data.damager = this;
                data.direction = this.transform.position;
                data.damageSource = player.transform.position;
                data.throwing = false;
                data.stopCamera = false;
                damageable.ApplyDamage(data);
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
    }

    void CheckForSwing()
    {
        Vector3 currentVelocity = rb.velocity;

        // Check the difference in velocity
        float velocityChange = Vector3.Distance(currentVelocity, lastVelocity);

        // Check if the change is above the threshold
        if (velocityChange > swingThreshold)
        {
            Debug.Log("Object is swinging!");
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
