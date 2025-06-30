using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;


[Component(PropertyGuid = "4b55a8aba9afa60e0de1f6150101d0705d83290f")]
public class CameraUnitSelection : Component
{
	public AssetLink selectionCircle;
	public vec3 offset = new vec3(0f, 0f, 0.01f);

	private bool selected = false;
	public bool Selected
	{
		get { return selected; }
		set 
		{
			selected = value;
			if (selectionCircleNode != null) 
				selectionCircleNode.Enabled = selected; 
		}
	}

	private Node selectionCircleNode = null;

	private void Init()
	{
		selectionCircleNode = World.LoadNode(selectionCircle.Path);
		if(selectionCircleNode == null)
		{
			Log.Error($"UnitSelectionCircle::init(): cannot load node {selectionCircle.Path}\n");
			return;
		}

		selectionCircleNode.Parent = node;
		selectionCircleNode.Position = offset;
		selectionCircleNode.Enabled = false;
	}
}
