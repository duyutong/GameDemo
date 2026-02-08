using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public abstract class EventVisualElement:VisualElement
{
    public bool isAdd;
    public abstract void Init();
    public abstract NetworkProtocolEventSaveData GetEventSaveData();
    public abstract void SetProtocolDataToWindow(NetworkProtocolEventSaveData eventSaveData);
}
