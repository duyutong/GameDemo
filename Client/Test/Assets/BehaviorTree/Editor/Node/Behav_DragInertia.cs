
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class Behav_DragInertia : BehaviorNode
{
    public override string stateName => "DragInertiaState";
    public Behav_DragInertia() : base() 
    {
        title = "DragInertia";
        
        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(System.Boolean), Port.Capacity.Single);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        
        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Single);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

    }
}
