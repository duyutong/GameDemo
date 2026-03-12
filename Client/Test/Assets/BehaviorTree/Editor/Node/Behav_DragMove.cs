
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class Behav_DragMove : BehaviorNode
{
    public override string stateName => "DragMoveState";
    public Behav_DragMove() : base() 
    {
        title = "DragMove";
        
        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(System.Boolean), Port.Capacity.Single);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        Port port_dragOffset = CreatePortForNode(this, Direction.Input, typeof(UnityEngine.Vector2), Port.Capacity.Single);
        port_dragOffset.portName = "dragOffset";
        inputContainer.Add(port_dragOffset);

        
        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

    }
}
