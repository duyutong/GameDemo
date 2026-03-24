using UnityEngine;
using TMPro;

public class ExplorationTimer : MonoBehaviour
{
    [Header("Time Settings")]
    public float roundTime = 120f;

    [Header("References")]
    public Transform player;
    public Transform spawnPoint;
    public TextMeshProUGUI timeText;

    private float timeLeft;
    private bool isRunning = false;

    void Start()
    {
        StartRound();
    }

    void Update()
    {
        if (!isRunning) return;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0)
        {
            EndRound();
        }

        UpdateUI();
    }

    void StartRound()
    {
        timeLeft = roundTime;
        isRunning = true;
        UpdateUI();
    }

    void EndRound()
    {
        isRunning = false;
        timeLeft = 0;

        // 传送玩家回出生点
        player.position = spawnPoint.position;

        // 如果你之后想自动开始下一回合
        StartRound();
    }

    void UpdateUI()
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60f);
        int seconds = Mathf.FloorToInt(timeLeft % 60f);

        timeText.text = $"{minutes:00}:{seconds:00}";
    }
}
