using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Unigine;

[Component(PropertyGuid = "46b8b84b2dc018a78753ab7dd80b07824cfdbe4f")]
public class BodyFractureShootingGalleryGun : Component
{
	public float Force = 50.0f;

	[ParameterFile(Filter = ".node")]
	public string InstanceNode = "";

	[ParameterFile]
	public string CrosshairImage = "";
	public ivec2 CrosshairSize = new(25, 25);

	private WidgetSprite crosshair = null;

	void Init()
	{
		if (CrosshairImage != "")
		{
			crosshair = new(CrosshairImage) { Width = CrosshairSize.x, Height = CrosshairSize.y };
			Gui.GetCurrent().AddChild(crosshair, Gui.ALIGN_OVERLAP | Gui.ALIGN_CENTER);
		}
	}
	void Update()
	{
		if (Console.Active && InstanceNode != "")
			return;

		if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT))
		{
			var inst = World.LoadNode(InstanceNode);
			inst.WorldPosition = node.WorldPosition;

			var body = inst.ObjectBodyRigid;
			if (!body)
				return;

			body.AddLinearImpulse(node.GetWorldDirection() * Force);
		}
	}

	void Shutdown()
	{
		crosshair?.DeleteLater();
	}
}
