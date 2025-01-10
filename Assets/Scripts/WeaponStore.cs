using UnityEngine;
using Valve.VR.InteractionSystem;

public class WeaponStore : MonoBehaviour
{
    // Reason for adjusted scale;
    // setting and object's local scale when attached to a parent
    // multiplies the scale by the parents scale.
    // 1 is about the width of the wrist
    [SerializeField, Range(0, 2)]
    float _weaponScale;

    [SerializeField, Range(0, 360)]
    int rotationInDegrees;

    private Vector3 weaponRotationOffset;

    public Throwable throwing;
    public Transform holster; // Target label to parent to ; renamed from label
    private Transform originalParent; // To store the original parent
    private Vector3 originalScale; // To store the original scale
    public bool weaponInHolster = false; // To ensure parenting happens only once ; renamed from IsParented

    [SerializeField]
    Hand hand;

    public Hand lastHeldByHand;

    [SerializeField]
    SwingDetection swingDetection;

    // done this way to store reference to mesh
    // and assign materials from editor
    private MeshRenderer holsterAppearance;

    [SerializeField]
    Material highlightedHolsterMaterial;
    [SerializeField]
    Material invisibleHolsterMaterial;

    private void Start()
    {
        holsterAppearance = GetComponent<MeshRenderer>();

        if (holster == null)
            holster = transform;

        weaponRotationOffset = new Vector3(rotationInDegrees, 0, 0);

    }

    private void Update()
    {
        //Vector3 test = new Vector3(x, y, z);

        //weapon.position = holster.position + test;

        if (swingDetection.heldInRight)
        {
            lastHeldByHand = swingDetection.rightHand;
        }

        if (swingDetection.heldInLeft)
        {
            lastHeldByHand = swingDetection.leftHand;
        }

    }

    // for reason unknown, attaching the weapon to the holster
    // then pulling it out removes the throwable's gravity

    // this function is called by the Throwable's OnDetachFromHand() Event in the inspector
    public void ReapplyGravity()
    {
        throwing.GetComponent<Rigidbody>().useGravity = true;
    }

    private void OnTriggerStay(Collider other)
    {

/*        if (!swingDetection.hasBeenPickedUp)
            return;*/

        if (hand == lastHeldByHand)
        {
            holsterAppearance.material = invisibleHolsterMaterial;
            return;
        }
            

        if (other.CompareTag("weapon") && !weaponInHolster)
        {
            holsterAppearance.material = highlightedHolsterMaterial;
            Debug.Log("Holster is highlighting");
        }
        else
        {
            Debug.Log("Holster is invisible");
            holsterAppearance.material = invisibleHolsterMaterial;
        }

        // grabbing weapon in holster sometimes wouldn't unholster weapon,
        // unholsters the weapon in case this happens
        if (throwing.attached)
        {
            UnholsterWeapon(other);
        }

        Debug.Log("Trigger Stay Detected");

        // Check if the object is not attached (using the Throwable component) and has the "weapon" tag
        if (!weaponInHolster && !throwing.attached && other.CompareTag("weapon"))
        {
            HolsterWeapon(other);
        }
        else if (throwing.attached) // If the object is attached via Throwable, scale it back
        {
            other.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    // same logic as before, just moved into its own function.
    // minor change to scale
    private void HolsterWeapon(Collider other)
    {
        Debug.Log("Parenting Weapon to Label");

        // Store original parent and scale
        originalParent = other.transform.parent;
        originalScale = other.transform.localScale;

        // Parent to the label and set scale
        other.transform.SetParent(holster);
        other.transform.position = holster.position; // Snap to the label's position
        other.transform.localScale = new Vector3(_weaponScale, _weaponScale, _weaponScale);
        other.transform.rotation = holster.rotation * Quaternion.Euler(weaponRotationOffset);

        Rigidbody weaponRigidbody = other.gameObject.GetComponent<Rigidbody>();
        weaponRigidbody.useGravity = false;
        weaponRigidbody.constraints = RigidbodyConstraints.FreezeAll;

        weaponInHolster = true; // Ensure this happens only once
    }

    private void OnTriggerExit(Collider other)
    {

        //if (!swingDetection.hasBeenPickedUp)
        //    return;

        if (hand == lastHeldByHand)
            return;

        if (!weaponInHolster && other.CompareTag("weapon"))
        {
            holsterAppearance.material = invisibleHolsterMaterial;
        }

        Debug.Log("Trigger Exit Detected");

        // Check if the object is parented and has the "weapon" tag
        if (weaponInHolster && throwing.attached && other.CompareTag("weapon"))
        {
            UnholsterWeapon(other);
        }
    }

    // same logic as before, just in its own function
    private void UnholsterWeapon(Collider other)
    {
        Debug.Log("Unparenting Weapon and Restoring Scale");

        // Restore original parent and scale
        other.transform.SetParent(originalParent);
        other.transform.localScale = originalScale;

        Rigidbody weaponRigidbody = other.gameObject.GetComponent<Rigidbody>();
        weaponRigidbody.useGravity = true;
        weaponRigidbody.constraints = RigidbodyConstraints.None;

        weaponInHolster = false; // Reset the parenting flag for future triggers
    }
}