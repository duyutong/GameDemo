
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class Trigger_ToggleGroup : TriggerNode
{
    public override string stateName => "ToggleGroupState";
    public Trigger_ToggleGroup() : base() 
    {
        title = "ToggleGroup";
        
        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(System.Boolean), Port.Capacity.Single);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        
        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

        Port port_isAnyTogglesOn = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_isAnyTogglesOn.portName = "isAnyTogglesOn";
        outputContainer.Add(port_isAnyTogglesOn);

        Port port_isAllTogglesOff = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_isAllTogglesOff.portName = "isAllTogglesOff";
        outputContainer.Add(port_isAllTogglesOff);

    }
}
