using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "725d757e1636d8f8c2c1da6f63cf62b3ae34b5a4")]
public class Toggler : Component
{
	[ParameterMask(MaskType = ParameterMaskAttribute.TYPE.INTERSECTION)]
	public int interaction_intersection_mask = 1 << 16;

	private void Update()
	{
		if (!Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT))
			return;

		ivec2 mouse = Input.MousePosition;
		Player player = (Player) node;

		if (player == null)
			return;

		vec3 direction = player.GetDirectionFromMainWindow(mouse.x, mouse.y);

		vec3 p0 = new vec3(player.WorldPosition);
		vec3 p1 = p0 + direction * player.ZFar;
		
		Object obj = World.GetIntersection(p0, p1, interaction_intersection_mask);

		if (obj)
		{
			var toggleable = obj.GetComponent<Toggleable>();
			if (toggleable)
			{
				toggleable.Toggle();
			}
		}
	}
	private void Init()
	{
		consoleOnScreen = Console.Onscreen;
		Console.Onscreen = true;
	}

	private void Shutdown()
	{
		Console.Onscreen = consoleOnScreen;
	}

	private bool consoleOnScreen = false;
}
