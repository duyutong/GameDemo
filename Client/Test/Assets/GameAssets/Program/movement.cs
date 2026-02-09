using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public float baseSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 input;

    // 所有当前生效的减速倍率
    private List<float> slowMultipliers = new List<float>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input = input.normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = input * baseSpeed * GetCurrentSpeedMultiplier();
    }

    // ======================
    // 对外接口（重点）
    // ======================

    public void AddSlow(float multiplier)
    {
        if (!slowMultipliers.Contains(multiplier))
        {
            slowMultipliers.Add(multiplier);
        }
    }

    public void RemoveSlow(float multiplier)
    {
        if (slowMultipliers.Contains(multiplier))
        {
            slowMultipliers.Remove(multiplier);
        }
    }

    // 取最慢的那个
    float GetCurrentSpeedMultiplier()
    {
        if (slowMultipliers.Count == 0)
            return 1f;

        float min = 1f;
        foreach (float m in slowMultipliers)
        {
            if (m < min)
                min = m;
        }
        return min;
    }
}
