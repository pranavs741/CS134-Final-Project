using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scoring")]
    [SerializeField] private int winScore = 10;

    [Header("Score / Lives UI")]
    [SerializeField] private TMP_Text scoreLabel;
    [SerializeField] private TMP_Text livesLabel;
    [SerializeField] private HeartLivesUI heartLivesUI;

    [Header("Menus")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text winLabel;
    [SerializeField] private TMP_Text gameOverLabel;

    [Header("Behavior")]
    [Tooltip("If true, the game starts paused with the start menu showing.")]
    [SerializeField] private bool showStartMenuOnLoad = true;
    [SerializeField] private KeyCode resetKey = KeyCode.R;

    private int score;
    private bool hasWon;
    private bool hasLost;
    private bool gameStarted;

    public int Score => score;
    public bool HasWon => hasWon;
    public bool HasLost => hasLost;
    public bool IsGameOver => hasWon || hasLost;
    public bool IsMenuOpen =>
        IsGameOver ||
        (startPanel != null && startPanel.activeSelf);

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        UpdateScoreUI();
        if (winPanel != null) winPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (showStartMenuOnLoad && startPanel != null)
        {
            startPanel.SetActive(true);
            Time.timeScale = 0f;
            gameStarted = false;
        }
        else
        {
            if (startPanel != null) startPanel.SetActive(false);
            Time.timeScale = 1f;
            gameStarted = true;
        }

        ApplyCursorState();
    }

    void Update()
    {
        if (Input.GetKeyDown(resetKey)) RestartScene();
        ApplyCursorState();
    }

    public void AddScore(int amount, string source = "")
    {
        if (IsGameOver || !gameStarted) return;

        score += amount;
        Debug.Log($"[Score] +{amount} ({source}). Total: {score}");
        UpdateScoreUI();

        if (score >= winScore) Win();
    }

    public void UpdateLivesUI(int current, int max)
    {
        if (livesLabel != null) livesLabel.text = $"Lives: {current}/{max}";
        if (heartLivesUI != null) heartLivesUI.SetLives(current, max);
    }

    public void GameOver()
    {
        if (IsGameOver) return;
        hasLost = true;
        Debug.Log("[Score] GAME OVER");
        if (gameOverLabel != null) gameOverLabel.text = "GAME OVER";
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void StartGame()
    {
        gameStarted = true;
        if (startPanel != null) startPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void UpdateScoreUI()
    {
        if (scoreLabel != null) scoreLabel.text = $"Score: {score}";
    }

    private void Win()
    {
        hasWon = true;
        Debug.Log("[Score] YOU WIN!");
        if (winLabel != null) winLabel.text = "YOU WIN!";
        if (winPanel != null) winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void ApplyCursorState()
    {
        bool unlock = IsMenuOpen || !gameStarted;
        Cursor.lockState = unlock ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = unlock;
    }
}
