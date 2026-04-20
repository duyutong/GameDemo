
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class Behav_bTTest : BehaviorNode
{
    public override string stateName => "bTTestState";
    public Behav_bTTest() : base() 
    {
        title = "bTTest";
        
        Port port_port1 = CreatePortForNode(this, Direction.Input, typeof(System.Boolean), Port.Capacity.Single);
        port_port1.portName = "port1";
        inputContainer.Add(port_port1);

        Port port_port3 = CreatePortForNode(this, Direction.Input, typeof(System.Boolean), Port.Capacity.Single);
        port_port3.portName = "port3";
        inputContainer.Add(port_port3);

        
        Port port_port2 = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Single);
        port_port2.portName = "port2";
        outputContainer.Add(port_port2);

    }
}
