using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Lives")]
    [SerializeField] private int maxLives = 3;

    private int currentLives;

    public int CurrentLives => currentLives;
    public int MaxLives => maxLives;
    public bool IsDead => currentLives <= 0;

    void Awake()
    {
        currentLives = maxLives;
    }

    void Start()
    {
        ReportLivesToManager();
    }

    public void LoseLife(int amount = 1)
    {
        if (IsDead) return;

        currentLives = Mathf.Max(0, currentLives - amount);
        Debug.Log($"[Health] -{amount} life. Remaining: {currentLives}/{maxLives}");
        ReportLivesToManager();

        if (IsDead && GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    public void ResetHealth()
    {
        currentLives = maxLives;
        ReportLivesToManager();
    }

    private void ReportLivesToManager()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateLivesUI(currentLives, maxLives);
        }
    }
}
