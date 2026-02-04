
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class Trigger_SwitchToggle : TriggerNode
{
    public override string stateName => "SwitchToggleState";
    public Trigger_SwitchToggle() : base() 
    {
        title = "SwitchToggle";
        
        
        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Single);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

        Port port_isOn = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_isOn.portName = "isOn";
        outputContainer.Add(port_isOn);

        Port port_isOff = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_isOff.portName = "isOff";
        outputContainer.Add(port_isOff);

    }
}
