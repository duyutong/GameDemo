using System.Collections.Generic;
using UnityEngine;

public class ClueSystem : MonoBehaviour
{
    [Header("Library")]
    public int librarySize = 30;          // X 条线索
    public int requiredToSubmit = 10;     // 收集10条提交

    [Header("Random")]
    public bool useFixedSeed = true;
    public int seed = 12345;

    // 线索库（静态内容）
    private List<ClueData> library = new List<ClueData>();

    // 每局随机真假：clueId -> isTrue
    private Dictionary<string, bool> truthMap = new Dictionary<string, bool>();

    // 玩家已收集：存 clueId
    private List<string> collected = new List<string>();

    void Start()
    {
        InitLibrary();
        StartNewRound();
    }

    // 初始化线索库（Demo阶段先自动生成；以后你可以换成ScriptableObject/表格导入）
    void InitLibrary()
    {
        library.Clear();
        for (int i = 1; i <= librarySize; i++)
        {
            library.Add(new ClueData(
                id: $"CLUE_{i:000}",
                title: $"线索 {i}",
                description: $"这是第 {i} 条线索的描述（Demo占位）。"
            ));
        }
    }

    public void StartNewRound()
    {
        collected.Clear();
        truthMap.Clear();

        if (useFixedSeed) Random.InitState(seed);
        else Random.InitState(System.Environment.TickCount);

        // 每条线索随机真假
        foreach (var clue in library)
        {
            bool isTrue = Random.value < 0.5f;
            truthMap[clue.id] = isTrue;
        }

        Debug.Log($"[ClueSystem] New round started. Library={library.Count}");
    }

    // Demo：随机收集一条未收集的线索
    public void CollectRandomClue()
    {
        if (collected.Count >= requiredToSubmit)
        {
            Debug.Log("[ClueSystem] 已收集满10条，不能继续收集。");
            return;
        }

        // 找未收集的
        List<ClueData> candidates = new List<ClueData>();
        foreach (var c in library)
            if (!collected.Contains(c.id))
                candidates.Add(c);

        if (candidates.Count == 0)
        {
            Debug.LogWarning("[ClueSystem] 没有可收集的线索了。");
            return;
        }

        var pick = candidates[Random.Range(0, candidates.Count)];
        collected.Add(pick.id);

        Debug.Log($"[ClueSystem] 收集：{pick.id} - {pick.title}（当前 {collected.Count}/{requiredToSubmit}）");
    }

    // 提交并评分：返回一个结果对象
    public SubmissionResult Submit()
    {
        if (collected.Count < requiredToSubmit)
        {
            Debug.LogWarning($"[ClueSystem] 未收集到 {requiredToSubmit} 条，无法提交。");
            return null;
        }

        // 评分规则（可改）：真=+10，假=-5
        int score = 0;
        var entries = new List<ClueJudgement>();

        foreach (string id in collected)
        {
            bool isTrue = truthMap.TryGetValue(id, out var t) ? t : false;
            int delta = isTrue ? 10 : -5;
            score += delta;

            entries.Add(new ClueJudgement
            {
                clueId = id,
                isTrue = isTrue,
                scoreDelta = delta
            });
        }

        var result = new SubmissionResult
        {
            totalScore = score,
            judgements = entries
        };

        // 输出到Console（之后可接UI）
        Debug.Log($"[ClueSystem] 提交完成，总分：{result.totalScore}");
        foreach (var e in result.judgements)
            Debug.Log($"  - {e.clueId}: {(e.isTrue ? "真" : "假")} ({(e.scoreDelta>=0?"+":"")}{e.scoreDelta})");

        return result;
    }

    public IReadOnlyList<string> GetCollectedIds()
    {
    return collected;
    }

    // 供UI显示用（可选）
    public int CollectedCount => collected.Count;
    public int RequiredCount => requiredToSubmit;
}

// 每条线索的判定结果
[System.Serializable]
public class ClueJudgement
{
    public string clueId;
    public bool isTrue;
    public int scoreDelta;
}

// 一次提交的总体结果
[System.Serializable]
public class SubmissionResult
{
    public int totalScore;
    public List<ClueJudgement> judgements;
}

