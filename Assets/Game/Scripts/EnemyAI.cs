using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float turnSpeed = 360f;
    [SerializeField] private float stopDistance = 0.6f;

    [Header("Targeting")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private Transform target;

    [Header("Damage")]
    [SerializeField] private int damageAmount = 1;
    [Tooltip("How close (in meters) the player has to be to take damage. Roughly the size of the enemy's body.")]
    [SerializeField] private float contactRadius = 1.0f;

    [Header("Stun")]
    [SerializeField] private float stunDuration = 3f;
    [SerializeField] private string ballTag = "Basketball";

    [Header("Animation")]
    [Tooltip("Animator that drives the enemy's movement/attack clips. Leave empty to auto-find on this object or its children.")]
    [SerializeField] private Animator animator;
    [Tooltip("Bool parameter set to true while the enemy is actively chasing the player.")]
    [SerializeField] private string runningBoolParam = "IsRunning";
    [Tooltip("Trigger parameter fired the moment the enemy hits the player.")]
    [SerializeField] private string attackTriggerParam = "Attack";

    private Rigidbody rb;
    private float stunTimer;
    private static readonly Collider[] overlapBuffer = new Collider[8];

    public bool IsStunned => stunTimer > 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.sleepThreshold = 0f;

        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        if (target == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag(playerTag);
            if (go != null) target = go.transform;
        }
    }

    void FixedUpdate()
    {
        bool isRunning = false;

        if (stunTimer > 0f)
        {
            stunTimer -= Time.fixedDeltaTime;
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }
        else if (TryDamageOverlappingPlayer())
        {
            // Damage applied; we're now stunned, so we're not running.
        }
        else
        {
            isRunning = MoveTowardTarget();
        }

        SetRunningAnim(isRunning);
    }

    private bool TryDamageOverlappingPlayer()
    {
        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            contactRadius,
            overlapBuffer,
            ~0,
            QueryTriggerInteraction.Ignore);

        for (int i = 0; i < count; i++)
        {
            Collider c = overlapBuffer[i];
            if (c == null) continue;
            if (!c.CompareTag(playerTag)) continue;

            PlayerHealth health = c.GetComponentInParent<PlayerHealth>();
            if (health != null) health.LoseLife(damageAmount);

            FireAttackAnim();
            Stun();
            return true;
        }

        return false;
    }

    private bool MoveTowardTarget()
    {
        if (target == null) return false;

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;
        float dist = toTarget.magnitude;

        if (dist > 0.001f)
        {
            Quaternion want = Quaternion.LookRotation(toTarget);
            rb.MoveRotation(Quaternion.RotateTowards(
                rb.rotation, want, turnSpeed * Time.fixedDeltaTime));
        }

        bool moving = dist > stopDistance;
        Vector3 desired = moving ? toTarget.normalized * moveSpeed : Vector3.zero;
        rb.velocity = new Vector3(desired.x, rb.velocity.y, desired.z);
        return moving;
    }

    private void SetRunningAnim(bool running)
    {
        if (animator == null) return;
        if (string.IsNullOrEmpty(runningBoolParam)) return;
        animator.SetBool(runningBoolParam, running);
    }

    private void FireAttackAnim()
    {
        if (animator == null) return;
        if (string.IsNullOrEmpty(attackTriggerParam)) return;
        animator.SetTrigger(attackTriggerParam);
    }

    public void Stun() => Stun(stunDuration);

    public void Stun(float duration)
    {
        if (duration > stunTimer) stunTimer = duration;
        rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(ballTag))
        {
            Stun();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, contactRadius);
    }
}
