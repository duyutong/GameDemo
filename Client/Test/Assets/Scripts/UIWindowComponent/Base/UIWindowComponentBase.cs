using UnityEngine;

public class UIWindowComponentBase : MonoBehaviour
{
    public virtual void Open() => OnOpen();
    public virtual void Close() => OnClose();
    protected virtual void OnOpen()  { }
    protected virtual void OnClose()  { }
    protected virtual void OnInit() { }
    protected virtual void OnUninit() { }

}
