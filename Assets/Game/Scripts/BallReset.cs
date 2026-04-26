using UnityEngine;

public class BallReset : MonoBehaviour
{
    [SerializeField] private float maxDistance = 25f;

    private Vector3 homePosition;
    private Rigidbody rb;
    private BallEffects effects;

    void Start()
    {
        homePosition = transform.position;
        rb = GetComponent<Rigidbody>();
        effects = GetComponent<BallEffects>();
    }

    void Update()
    {
        // Skip while the ball is being held (parented to the player's hold point).
        if (transform.parent != null) return;

        if (Vector3.Distance(transform.position, homePosition) > maxDistance)
        {
            ResetToHome();
        }
    }

    void ResetToHome()
    {
        transform.position = homePosition;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (effects != null) effects.PlayResetEffect();
    }
}
