using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "14dbb4a732390adb693f20c20b2d7b277512f948")]
public class RobotArmConnection : Component
{
	[ShowInEditor]
	private Node connectionPoint;

	[ShowInEditor]
	private PhysicalTrigger connectionTrigger;

	private JointFixed jointFixed;
	private BodyRigid connectionCandidate = null;
	private bool connected = false;

	private void Init()
	{
		connectionTrigger.EventEnter.Connect(OnTriggerEnter);
		connectionTrigger.EventLeave.Connect(OnTriggerLeave);

		int num = node.ObjectBody.FindJoint("connection_joint");
		if (num != -1)
		{
			jointFixed = node.ObjectBody.GetJoint(num) as JointFixed;
		}
	}

	private void Update()
	{
		if(Input.IsKeyDown(Input.KEY.C) && connectionCandidate != null)
		{
			connected = true;

			jointFixed.Body1 = connectionCandidate;

			var itransform = MathLib.Inverse(connectionCandidate.Transform);
			var anchor_1_transform = itransform * connectionPoint.WorldTransform;
			jointFixed.Anchor1 = anchor_1_transform.Translate;
			jointFixed.Rotation1 = anchor_1_transform.GetRotate().Mat3;

			jointFixed.Enabled = true;
		}

		if (Input.IsKeyDown(Input.KEY.V))
		{
			connected = false;
			jointFixed.Enabled = false;
		}
	}

	private void OnTriggerEnter(Body body)
	{
		if (!connectionCandidate)
			connectionCandidate = body as BodyRigid;
	}

	private void OnTriggerLeave(Body body)
	{
		if(connectionCandidate == (body as BodyRigid))
		{
			connectionCandidate = null;
			connected = false;
			jointFixed.Enabled = false;
		}
	}
}
