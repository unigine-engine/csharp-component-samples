#region Math Variables
#if UNIGINE_DOUBLE
using Scalar = System.Double;
using Vec2 = Unigine.dvec2;
using Vec3 = Unigine.dvec3;
using Vec4 = Unigine.dvec4;
using Mat4 = Unigine.dmat4;
#else
using Scalar = System.Single;
using Vec2 = Unigine.vec2;
using Vec3 = Unigine.vec3;
using Vec4 = Unigine.vec4;
using Mat4 = Unigine.mat4;
using WorldBoundBox = Unigine.BoundBox;
using WorldBoundSphere = Unigine.BoundSphere;
using WorldBoundFrustum = Unigine.BoundFrustum;
#endif
#endregion

using Unigine;

[Component(PropertyGuid = "b9a76f513f32e07ca391c1ab87b2eb0e5e8a7715")]
public class MouseRayIntersection : Component
{
	public float distance = 100.0f;

	// use mask to separate objects for intersection
	[ParameterMask(MaskType = ParameterMaskAttribute.TYPE.INTERSECTION)]
	public int mask = 1;

	private WidgetLabel label = null;
	Input.MOUSE_HANDLE initHandle;

	private void Init()
	{
		// show cursor when player is not rotate
		initHandle = Input.MouseHandle;
		Input.MouseHandle = Input.MOUSE_HANDLE.SOFT;

		// create label for target object name
		label = new WidgetLabel(Gui.GetCurrent());
		label.FontSize = 30;
		label.FontOutline = 1;
		Gui.GetCurrent().AddChild(label, Gui.ALIGN_OVERLAP);
	}

	private void Update()
	{
		// get points to detect intersection based on mouse position and player direction
		Vec3 firstPoint = Game.Player.WorldPosition;
		ivec2 mouse_coord = Input.MousePosition;
		Vec3 secondPoint = firstPoint + Game.Player.GetDirectionFromMainWindow(mouse_coord.x, mouse_coord.y) * distance;

		// try to get intersection object
		Unigine.Object hitObject = World.GetIntersection(firstPoint, secondPoint, mask);
		if (hitObject)
		{
			// change object name
			label.Text = hitObject.Name;
		}
		else
			label.Text = "empty hit object";

		// update cursor position
		label.SetPosition(WindowManager.MainWindow.Gui.MouseX + 25, WindowManager.MainWindow.Gui.MouseY + 25);
	}

	private void Shutdown()
	{
		Gui.GetCurrent().RemoveChild(label);

		Input.MouseHandle = initHandle;
	}
}
