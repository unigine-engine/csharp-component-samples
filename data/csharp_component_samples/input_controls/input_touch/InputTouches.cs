using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "587166faf7a2ca1167defc3cfd1daa6b8d4a3cce")]
public class InputTouches : Component
{
	public ivec2[] TouchesPositions { get; private set; } = new ivec2[Input.NUM_TOUCHES];

	private void Init()
	{
		for (int i = 0; i < Input.NUM_TOUCHES; i++)
			TouchesPositions[i] = -ivec2.ONE;
	}

	private void Update()
	{
		for (int i = 0; i < Input.NUM_TOUCHES; i++)
		{
			if (Input.IsTouchPressed(i))
				TouchesPositions[i] = Input.GetTouchPosition(i);

			if (Input.IsTouchUp(i))
				TouchesPositions[i] = -ivec2.ONE;
		}
	}
}
