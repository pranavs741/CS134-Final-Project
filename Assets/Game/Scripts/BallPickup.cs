using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPickup : MonoBehaviour
{
    public Transform holdPoint;
    public float pickupRange = 3f;
    public KeyCode pickupKey = KeyCode.E;

    private GameObject heldBall;

    [Header("Shot Power")]
    [SerializeField] private float minShotSpeed = 8f;
    [SerializeField] private float maxShotSpeed = 25f;
    [SerializeField] private float chargeTime = 1.0f;
    [SerializeField] private KeyCode shootKey = KeyCode.Mouse1;

    [Header("Trajectory Preview")]
    [SerializeField] private LineRenderer trajectoryLine;
    [SerializeField] private int trajectorySteps = 40;
    [SerializeField] private float trajectoryStepTime = 0.08f;
    [SerializeField] private float lineWidth = 0.03f;

    private bool isCharging;
    private float chargeStartTime;

    void Awake()
    {
        if (trajectoryLine == null) trajectoryLine = GetComponent<LineRenderer>();
        if (trajectoryLine != null) {
            trajectoryLine.positionCount = 0;
            trajectoryLine.startWidth = lineWidth;
            trajectoryLine.endWidth = lineWidth;
        }
    }

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
            DrawTrajectory();
        }
        else if (trajectoryLine != null) {
            trajectoryLine.positionCount = 0;
        }

        // shooting ball - hold to charge, release to fire
        if (Input.GetKeyDown(shootKey) && heldBall != null) {
            isCharging = true;
            chargeStartTime = Time.time;
        }
        if (Input.GetKeyUp(shootKey)) {
            if (isCharging && heldBall != null) {
                ShootBall();
            }
            isCharging = false;
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
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = true;
                }

                BallScoreTracker tracker = heldBall.GetComponent<BallScoreTracker>();
                if (tracker != null) tracker.EndShot();

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
            rb.velocity = ComputeShootVelocity();
        }

        BallScoreTracker tracker = heldBall.GetComponent<BallScoreTracker>();
        if (tracker != null) tracker.BeginShot();

        if (trajectoryLine != null) trajectoryLine.positionCount = 0;
        heldBall = null;
        isCharging = false;
    }

    Vector3 ComputeShootVelocity()
    {
        Vector3 forward = Camera.main != null
            ? Camera.main.transform.forward
            : transform.forward;
        return forward.normalized * GetCurrentShotSpeed();
    }

    float GetCurrentShotSpeed()
    {
        if (!isCharging) return minShotSpeed;
        float elapsed = Time.time - chargeStartTime;
        // Ping-pong 0..1..0..1... so power ramps up, back down, and loops.
        float t = Mathf.PingPong(elapsed, chargeTime) / chargeTime;
        return Mathf.Lerp(minShotSpeed, maxShotSpeed, t);
    }

    void DrawTrajectory()
    {
        if (trajectoryLine == null) return;

        Vector3 pos = holdPoint.position;
        Vector3 vel = ComputeShootVelocity();
        Vector3 g = Physics.gravity;

        trajectoryLine.positionCount = trajectorySteps;
        for (int i = 0; i < trajectorySteps; i++)
        {
            trajectoryLine.SetPosition(i, pos);
            Vector3 nextVel = vel + g * trajectoryStepTime;
            pos += (vel + nextVel) * 0.5f * trajectoryStepTime;
            vel = nextVel;
        }
    }
}
