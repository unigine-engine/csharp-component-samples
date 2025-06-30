using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "a315de6ff40279026df7e479e6dec384cf897913")]
public class Movement : Component
{
	[ShowInEditor]
	private Node movementPointNode = null;

	private BodyRigid bodyRigid = null;

	private vec3 direction = vec3.ZERO;
	private float forceMultiplier = 6.0f;

	private void Init()
	{
		bodyRigid = node.ObjectBodyRigid;
	}
	
	private void Update()
	{
		direction = vec3.ZERO;

		if (Input.IsKeyPressed(Input.KEY.W))
			direction += vec3.FORWARD;

		if (Input.IsKeyPressed(Input.KEY.S))
			direction += vec3.BACK;

		if (Input.IsKeyPressed(Input.KEY.D))
			direction += vec3.LEFT * 0.15f;

		if (Input.IsKeyPressed(Input.KEY.A))
			direction += vec3.RIGHT * 0.15f;
	}

	private void UpdatePhysics()
	{
		direction = bodyRigid.Transform.GetRotate() * direction;

		bodyRigid.AddWorldForce(movementPointNode.WorldPosition, direction * bodyRigid.Mass * forceMultiplier);
	}
}
