
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class Behav_SpriteRendererAlpha : BehaviorNode
{
    public override string stateName => "SpriteRendererAlphaState";
    public Behav_SpriteRendererAlpha() : base() 
    {
        title = "SpriteRendererAlpha";
        
        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(System.Boolean), Port.Capacity.Multi);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        
        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

    }
}
