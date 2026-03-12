
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class Behav_GetDragOffset : BehaviorNode
{
    public override string stateName => "GetDragOffsetState";
    public Behav_GetDragOffset() : base() 
    {
        title = "GetDragOffset";
        
        Port port_onDragBegin = CreatePortForNode(this, Direction.Input, typeof(System.Boolean), Port.Capacity.Single);
        port_onDragBegin.portName = "onDragBegin";
        inputContainer.Add(port_onDragBegin);

        Port port_onDrag = CreatePortForNode(this, Direction.Input, typeof(System.Boolean), Port.Capacity.Single);
        port_onDrag.portName = "onDrag";
        inputContainer.Add(port_onDrag);

        
        Port port_dragOffset = CreatePortForNode(this, Direction.Output, typeof(UnityEngine.Vector2), Port.Capacity.Multi);
        port_dragOffset.portName = "dragOffset";
        outputContainer.Add(port_dragOffset);

        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

    }
}
