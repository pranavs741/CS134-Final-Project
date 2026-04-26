using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPickup : MonoBehaviour
{
    public Transform holdPoint;
    public float pickupRange = 3f;
    public KeyCode pickupKey = KeyCode.E;

    private GameObject heldBall;

    [SerializeField] private float ballSpeed = 10f;

    void Update()
    {
        // picking up ball
        if (Input.GetKeyDown(pickupKey)) {
            if (heldBall == null) {
                Debug.Log("E is pressed");
                TryPickUpBall();
            }
            else {
                DropBall();
            }
        }
        if (heldBall != null) {
            heldBall.transform.position = holdPoint.position;
        }

        // shooting ball
        if (Input.GetKeyDown(KeyCode.G)) {
            if (heldBall != null) {
                ShootBall();
            }
        }

    }

    void TryPickUpBall() 
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange);
        
        foreach (Collider hit in hits) {
            if (hit.CompareTag("Basketball")) {
                heldBall = hit.gameObject;

                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null) {
                    rb.isKinematic = true;
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                heldBall.transform.position = holdPoint.position;
                heldBall.transform.SetParent(holdPoint);

                return;
            }
        }
    }

    void DropBall()
    {
        Rigidbody rb = heldBall.GetComponent<Rigidbody>();

        heldBall.transform.SetParent(null);

        if (rb != null) {
            rb.isKinematic = false;
        }

        heldBall = null;
    }

    void ShootBall() 
    {
        heldBall.transform.SetParent(null);

        Rigidbody rb = heldBall.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = false;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 shootDirection = ray.direction.normalized;

            rb.velocity = shootDirection * ballSpeed;
        }

        heldBall = null; 
    }


}
