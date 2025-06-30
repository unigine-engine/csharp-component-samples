using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "5e20f9d3c8d23de58480e7bd3ce7084517bdcc7a")]
public class RobotArmController : Component
{
	public struct ArmJoint
	{
		public Node armNode;
		public float speed;

		private JointHinge armJointHinge;

		public ArmJoint()
		{
			armNode = null;
			speed = 100.0f;
			armJointHinge = null;
		}

		public void init()
		{
			armJointHinge = armNode.ObjectBody.GetJoint(0) as JointHinge;
		}

		public void update(float ifps, Input.KEY positiveAxis, Input.KEY negativeAxis)
		{
			if (Input.IsKeyPressed(positiveAxis))
				rotate(speed * ifps);

			if (Input.IsKeyPressed(negativeAxis))
				rotate(-speed * ifps);
		}

		private void rotate(float angle)
		{
			if (armJointHinge)
			{
				float currentAngle = armJointHinge.AngularAngle;
				currentAngle += angle;
				if (currentAngle < -180)
					currentAngle += 360.0f;
				if (currentAngle > 180)
					currentAngle -= 360.0f;

				if (currentAngle < armJointHinge.AngularLimitFrom)
					currentAngle = armJointHinge.AngularLimitFrom;

				if (currentAngle > armJointHinge.AngularLimitTo)
					currentAngle = armJointHinge.AngularLimitTo;

				armJointHinge.AngularAngle = currentAngle;
			}
		}
	}

	public ArmJoint armJoint0 = new ArmJoint();
	public ArmJoint armJoint1 = new ArmJoint();
	public ArmJoint armJoint2 = new ArmJoint();
	public ArmJoint armJoint3 = new ArmJoint();
	public ArmJoint armJoint4 = new ArmJoint();
	public ArmJoint armJoint5 = new ArmJoint();

	void Init()
	{
		armJoint0.init();
		armJoint1.init();
		armJoint2.init();
		armJoint3.init();
		armJoint4.init();
		armJoint5.init();
	}
	
	void Update()
	{
		armJoint0.update(Game.IFps, Input.KEY.H, Input.KEY.F);
		armJoint1.update(Game.IFps, Input.KEY.T, Input.KEY.G);
		armJoint2.update(Game.IFps, Input.KEY.I, Input.KEY.K);
		armJoint3.update(Game.IFps, Input.KEY.J, Input.KEY.L);
		armJoint4.update(Game.IFps, Input.KEY.U, Input.KEY.O);
		armJoint5.update(Game.IFps, Input.KEY.R, Input.KEY.Y);
	}
}
