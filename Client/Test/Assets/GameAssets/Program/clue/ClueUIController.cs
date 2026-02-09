using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClueUIController : MonoBehaviour
{
    [Header("Refs")]
    public ClueSystem clueSystem;

    [Header("UI")]
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI collectedText;
    public TextMeshProUGUI resultText;

    public Button collectButton;
    public Button submitButton;
    public Button resetButton;

    void Start()
    {
        // 按钮绑定
        collectButton.onClick.AddListener(OnCollectClicked);
        submitButton.onClick.AddListener(OnSubmitClicked);
        resetButton.onClick.AddListener(OnResetClicked);

        RefreshAll();
    }

    void OnCollectClicked()
    {
        clueSystem.CollectRandomClue();
        RefreshAll();
    }

    void OnSubmitClicked()
    {
        var result = clueSystem.Submit();
        RefreshAll(result);
    }

    void OnResetClicked()
    {
        clueSystem.StartNewRound();
        RefreshAll();
    }

    void RefreshAll(SubmissionResult result = null)
    {
        // 顶部状态
        statusText.text = $"Collected: {clueSystem.CollectedCount}/{clueSystem.RequiredCount}";

        // 已收集列表（需要 ClueSystem 提供可读数据：下面第4步会让它支持）
        collectedText.text = BuildCollectedText();

        // 结果区
        if (result == null)
        {
            resultText.text = "(Submit to see results)";
        }
        else
        {
            resultText.text = BuildResultText(result);
        }

        // 按钮可用性
        collectButton.interactable = clueSystem.CollectedCount < clueSystem.RequiredCount;
        submitButton.interactable = clueSystem.CollectedCount >= clueSystem.RequiredCount;
    }

    string BuildCollectedText()
    {
        var ids = clueSystem.GetCollectedIds();
        var sb = new StringBuilder();
        sb.AppendLine("Collected Clues:");
        sb.AppendLine("----------------");

        for (int i = 0; i < ids.Count; i++)
        {
            sb.AppendLine($"{i + 1}. {ids[i]}");
        }

        return sb.ToString();
    }

    string BuildResultText(SubmissionResult result)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Total Score: {result.totalScore}");
        sb.AppendLine("----------------");

        for (int i = 0; i < result.judgements.Count; i++)
        {
            var j = result.judgements[i];
            string tf = j.isTrue ? "TRUE" : "FALSE";
            string delta = j.scoreDelta >= 0 ? $"+{j.scoreDelta}" : $"{j.scoreDelta}";
            sb.AppendLine($"{i + 1}. {j.clueId}  →  {tf}  ({delta})");
        }

        return sb.ToString();
    }
}
