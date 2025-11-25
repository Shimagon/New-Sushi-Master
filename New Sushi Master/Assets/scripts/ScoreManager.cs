using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スコア管理システム（シングルトン）
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score Settings")]
    [Tooltip("現在のスコア")]
    public int currentScore = 0;

    [Tooltip("ハイスコア")]
    public int highScore = 0;

    [Header("UI References")]
    [Tooltip("スコア表示用のテキスト")]
    public Text scoreText;

    [Tooltip("ハイスコア表示用のテキスト")]
    public Text highScoreText;

    [Header("Audio")]
    [Tooltip("スコア獲得時の効果音")]
    public AudioClip scoreSound;

    [Tooltip("ハイスコア更新時の効果音")]
    public AudioClip highScoreSound;

    private AudioSource audioSource;

    void Awake()
    {
        // シングルトンの設定
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // AudioSourceを追加
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // ハイスコアを読み込み
        LoadHighScore();
    }

    void Start()
    {
        UpdateScoreUI();
    }

    /// <summary>
    /// スコアを追加
    /// </summary>
    public void AddScore(int points)
    {
        currentScore += points;

        Debug.Log($"スコア追加: +{points}点 (合計: {currentScore}点)");

        // ハイスコア更新チェック
        bool isNewHighScore = false;
        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveHighScore();
            isNewHighScore = true;
            Debug.Log($"新しいハイスコア: {highScore}点！");
        }

        // UIを更新
        UpdateScoreUI();

        // 効果音を再生
        PlayScoreSound(isNewHighScore);
    }

    /// <summary>
    /// スコアをリセット
    /// </summary>
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
        Debug.Log("スコアをリセットしました");
    }

    /// <summary>
    /// UIを更新
    /// </summary>
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }

        if (highScoreText != null)
        {
            highScoreText.text = $"High Score: {highScore}";
        }
    }

    /// <summary>
    /// スコア獲得音を再生
    /// </summary>
    private void PlayScoreSound(bool isHighScore)
    {
        if (audioSource == null) return;

        if (isHighScore && highScoreSound != null)
        {
            audioSource.PlayOneShot(highScoreSound);
        }
        else if (scoreSound != null)
        {
            audioSource.PlayOneShot(scoreSound);
        }
    }

    /// <summary>
    /// ハイスコアを保存
    /// </summary>
    private void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// ハイスコアを読み込み
    /// </summary>
    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    /// <summary>
    /// 現在のスコアを取得
    /// </summary>
    public int GetCurrentScore()
    {
        return currentScore;
    }

    /// <summary>
    /// ハイスコアを取得
    /// </summary>
    public int GetHighScore()
    {
        return highScore;
    }
}
