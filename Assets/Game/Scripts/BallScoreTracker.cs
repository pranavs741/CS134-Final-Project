using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallScoreTracker : MonoBehaviour
{
    [Header("Points")]
    [SerializeField] private int backboardPoints = 1;
    [SerializeField] private int rimPoints = 3;
    [SerializeField] private int hoopPoints = 10;

    [Header("Tags")]
    [SerializeField] private string backboardTag = "Backboard";
    [SerializeField] private string rimTag = "Rim";
    [SerializeField] private string hoopTag = "Hoop";

    [Header("Hoop Make Detection")]
    [Tooltip("Ball must be falling at least this fast (m/s downward) to count as a make.")]
    [SerializeField] private float minDownwardSpeed = 1.0f;
    [Tooltip("Vertical speed must be at least this multiple of horizontal speed. " +
             "1.0 = 'mostly downward'; lower = more lenient.")]
    [SerializeField] private float verticalDominanceRatio = 1.0f;

    private Rigidbody rb;
    private bool isLive;
    private bool scoredBackboard;
    private bool scoredRim;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void BeginShot()
    {
        isLive = true;
        scoredBackboard = false;
        scoredRim = false;
    }

    public void EndShot()
    {
        isLive = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isLive || GameManager.Instance == null) return;

        Collider other = collision.collider;

        if (!scoredBackboard && other.CompareTag(backboardTag))
        {
            scoredBackboard = true;
            GameManager.Instance.AddScore(backboardPoints, "Backboard");
        }
        else if (!scoredRim && other.CompareTag(rimTag))
        {
            scoredRim = true;
            GameManager.Instance.AddScore(rimPoints, "Rim");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isLive || GameManager.Instance == null) return;
        if (!other.CompareTag(hoopTag)) return;

        if (!IsFallingThroughHoop()) return;

        GameManager.Instance.AddScore(hoopPoints, "Hoop");
        EndShot();
    }

    private bool IsFallingThroughHoop()
    {
        Vector3 v = rb.velocity;

        // Must be moving downward fast enough.
        if (v.y > -minDownwardSpeed) return false;

        // Vertical drop speed must dominate horizontal speed.
        float downSpeed = -v.y;
        float horizontalSpeed = new Vector2(v.x, v.z).magnitude;
        if (downSpeed < horizontalSpeed * verticalDominanceRatio) return false;

        return true;
    }
}
