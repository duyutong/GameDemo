using UnityEngine;
using UnityEngine.UI;

public class DoorButtonTest : MonoBehaviour
{
    [Header("UI")]
    public Image doorImage;

    [Header("Sprites")]
    public Sprite closedSprite;
    public Sprite openSprite;

    [Header("State")]
    public bool isOpen = false;

    public void OnClickDoor()
    {
        Debug.Log("Door button clicked");

        isOpen = !isOpen;

        if (isOpen)
        {
            doorImage.sprite = openSprite;
            Debug.Log("Door opened");
        }
        else
        {
            doorImage.sprite = closedSprite;
            Debug.Log("Door closed");
        }
    }
}