using UnityEngine;

public class SlowArea : MonoBehaviour
{
    [Range(0f, 1f)]
    public float slowMultiplier = 0.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger ENTER by: " + other.name);

        if (!other.CompareTag("Player"))
        {
            Debug.Log("Not player, ignored");
            return;
        }

        PlayerMovement movement = other.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            Debug.Log("Add slow: " + slowMultiplier);
            movement.AddSlow(slowMultiplier);
        }
        else
        {
            Debug.LogWarning("PlayerMovement not found on Player");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Trigger EXIT by: " + other.name);

        if (!other.CompareTag("Player"))
            return;

        PlayerMovement movement = other.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            Debug.Log("Remove slow: " + slowMultiplier);
            movement.RemoveSlow(slowMultiplier);
        }
    }
}
