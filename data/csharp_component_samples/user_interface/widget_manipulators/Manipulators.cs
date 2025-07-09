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

[Component(PropertyGuid = "7115b15ea4639d29e8e01b9227d75746985e637b")]
public class Manipulators : Component
{
	[ShowInEditor][ParameterMask]
	private int intersectionMask = 1;
	[ShowInEditor]
	private bool transformParent = true;

	private bool xAxisRotation = true;
	public bool XAxisRotation
	{
		get { return xAxisRotation; }

		set
		{
			xAxisRotation = value;
			if (xAxisRotation)
				objectRotator.Mask = objectRotator.Mask | WidgetManipulator.MASK_X;
			else
				objectRotator.Mask = objectRotator.Mask & ~(WidgetManipulator.MASK_X);
		}
	}

	private bool yAxisRotation = true;
	public bool YAxisRotation
	{
		get { return yAxisRotation; }

		set
		{
			yAxisRotation = value;
			if (yAxisRotation)
				objectRotator.Mask = objectRotator.Mask | WidgetManipulator.MASK_Y;
			else
				objectRotator.Mask = objectRotator.Mask & ~(WidgetManipulator.MASK_Y);
		}
	}

	private bool zAxisRotation = true;
	public bool ZAxisRotation
	{
		get { return zAxisRotation; }

		set
		{
			zAxisRotation = value;
			if (zAxisRotation)
				objectRotator.Mask = objectRotator.Mask | WidgetManipulator.MASK_Z;
			else
				objectRotator.Mask = objectRotator.Mask & ~(WidgetManipulator.MASK_Z);
		}
	}

	private bool xAxisTranslation = true;
	public bool XAxisTranslation
	{
		get { return xAxisTranslation; }

		set
		{
			xAxisTranslation = value;
			if (xAxisTranslation)
				objectTranslator.Mask = objectTranslator.Mask | WidgetManipulator.MASK_X;
			else
				objectTranslator.Mask = objectTranslator.Mask & ~(WidgetManipulator.MASK_X);
		}
	}

	private bool yAxisTranslation = true;
	public bool YAxisTranslation
	{
		get { return yAxisTranslation; }

		set
		{
			yAxisTranslation = value;
			if (yAxisTranslation)
				objectTranslator.Mask = objectTranslator.Mask | WidgetManipulator.MASK_Y;
			else
				objectTranslator.Mask = objectTranslator.Mask & ~(WidgetManipulator.MASK_Y);
		}
	}

	private bool zAxisTranslation = true;
	public bool ZAxisTranslation
	{
		get { return zAxisTranslation; }

		set
		{
			zAxisTranslation = value;
			if (zAxisTranslation)
				objectTranslator.Mask = objectTranslator.Mask | WidgetManipulator.MASK_Z;
			else
				objectTranslator.Mask = objectTranslator.Mask & ~(WidgetManipulator.MASK_Z);
		}
	}

	private bool xAxisScale = true;
	public bool XAxisScale
	{
		get { return xAxisScale; }

		set
		{
			xAxisScale = value;
			if (xAxisScale)
				objectScaler.Mask = objectScaler.Mask | WidgetManipulator.MASK_X;
			else
				objectScaler.Mask = objectScaler.Mask & ~(WidgetManipulator.MASK_X);
		}
	}

	private bool yAxisScale = true;
	public bool YAxisScale
	{
		get { return yAxisScale; }

		set
		{
			yAxisScale = value;
			if (yAxisScale)
				objectScaler.Mask = objectScaler.Mask | WidgetManipulator.MASK_Y;
			else
				objectScaler.Mask = objectScaler.Mask & ~(WidgetManipulator.MASK_Y);
		}
	}

	private bool zAxisScale = true;
	public bool ZAxisScale
	{
		get { return zAxisScale; }

		set
		{
			zAxisScale = value;
			if (zAxisScale)
				objectScaler.Mask = objectScaler.Mask | WidgetManipulator.MASK_Z;
			else
				objectScaler.Mask = objectScaler.Mask & ~(WidgetManipulator.MASK_Z);
		}
	}

	private bool localBasis = false;
	public bool LocalBasis
	{
		get { return localBasis; }
		
		set
		{
			localBasis = value;
			SetManipulatorsBasis();
		}
	}

	public bool Active { get { return obj != null; } }

	private WidgetManipulator currentObjectManipulator;
	private WidgetManipulatorTranslator objectTranslator;
	private WidgetManipulatorRotator objectRotator;
	private WidgetManipulatorScaler objectScaler;

	private Unigine.Object obj;

	private Gui gui;

	void Init()
	{
		gui = WindowManager.MainWindow.Gui;

		objectTranslator = new WidgetManipulatorTranslator(gui);
		objectRotator = new WidgetManipulatorRotator(gui);
		objectScaler = new WidgetManipulatorScaler(gui);
		
		gui.AddChild(objectTranslator);
		gui.AddChild(objectRotator);
		gui.AddChild(objectScaler);

		objectTranslator.Hidden = true;
		objectRotator.Hidden = true;
		objectScaler.Hidden = true;

		currentObjectManipulator = objectTranslator;

		objectTranslator.EventChanged.Connect(ApplyTransform);
		objectRotator.EventChanged.Connect(ApplyTransform);
		objectScaler.EventChanged.Connect(ApplyTransform);

		Player player = Game.Player;
		player.Controlled = false;
	}
	
	void Update()
	{
		Player player = Game.Player;

		// if mouse is grabbed, we enable player control making hotkeys for switching manipulators unavailable
		if (Input.MouseGrab)
			player.Controlled = true;

		// reset projection and view matrices for correct rendering of the widget
		if (player)
		{
			objectTranslator.Projection = player.Projection;
			objectRotator.Projection = player.Projection;
			objectScaler.Projection = player.Projection;

			objectTranslator.Modelview = player.Camera.Modelview;
			objectRotator.Modelview = player.Camera.Modelview;
			objectScaler.Modelview = player.Camera.Modelview;
		}

		// trying to get an object to manipulate
		if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.RIGHT) && !Input.MouseGrab)
		{
			obj = GetNodeUnderCursor();
			if (obj)
			{
				SwitchManipulator(currentObjectManipulator);
			}
			else
			{
				Unselect();
			}
		}

		// if some object is selected and it is not a ComponentLogic node
		if (obj)
		{
			// reset manipulator's transform matrix, after moving the object
			if (Input.IsMouseButtonUp(Input.MOUSE_BUTTON.LEFT))
			{
				SwitchManipulator(currentObjectManipulator);
			}

			// if mouse is not grabbed, disable player control and enable hotkeys to switch manipulators
			if (!Input.MouseGrab)
			{
				player.Controlled = false;

				if (Input.IsKeyDown(Input.KEY.W))
					SwitchManipulator(objectTranslator);

				if (Input.IsKeyDown(Input.KEY.E))
					SwitchManipulator(objectRotator);

				if (Input.IsKeyDown(Input.KEY.R))
					SwitchManipulator(objectScaler);
			}

			// hotkey to focus on an object at focus_distance distance
			if (Input.IsKeyDown(Input.KEY.F))
			{
				vec3 inversePlayerViewDirection = -player.ViewDirection;
				WorldBoundSphere bs = new WorldBoundSphere();
				GetMeshBS(obj, ref bs);
				player.WorldPosition = bs.Center + new Vec3(inversePlayerViewDirection * ((float)bs.Radius * 2.0f));
			}

			// hotkeys to unselect an object
			if (Input.IsKeyDown(Input.KEY.U) || Input.IsKeyDown(Input.KEY.ESC)) // || Input::isMouseCursorHide())
			{
				Unselect();
			}
		}

	}

	void Shutdown()
	{
		objectTranslator.DeleteLater();
		objectRotator.DeleteLater();
		objectScaler.DeleteLater();
	}

	private void GetMeshBS(in Node n, ref WorldBoundSphere bs)
	{
		if (!n) return;

		if (n.IsObject)
			bs.Expand(node.WorldBoundSphere);

		if (node.Type == Node.TYPE.NODE_REFERENCE)
			GetMeshBS((n as NodeReference).Reference, ref bs);
		for (int i = 0; i < n.NumChildren; i++)
			GetMeshBS(node.GetChild(i), ref bs);
	}

	private void ApplyTransform()
	{
		// applying transformation of the widget to the object
		if (obj)
		{
			Node manipulateNode = obj;

			if (transformParent && manipulateNode.Parent)
				manipulateNode = manipulateNode.Parent;

			manipulateNode.WorldTransform = currentObjectManipulator.Transform;
		}
	}

	private Unigine.Object GetNodeUnderCursor()
	{
		Player player = Game.Player;
		ivec2 mouse = Input.MousePosition;

		// returns the first object intersected by the ray casted from the player in the forward direction at a distance of 10000 units
		return World.GetIntersection(player.WorldPosition, player.WorldPosition + new Vec3(player.GetDirectionFromMainWindow(mouse.x, mouse.y) * 10000), intersectionMask);
	}

	private void SwitchManipulator(WidgetManipulator currentManipulator)
	{
		if (obj)
		{
			SetManipulatorsBasis();

			currentObjectManipulator = currentManipulator;
			currentObjectManipulator.Hidden = false;

			Node manipulateNode = obj;

			if (transformParent && manipulateNode.Parent)
				manipulateNode = manipulateNode.Parent;

			currentObjectManipulator.Transform = manipulateNode.WorldTransform;

			// show only selected manipulator
			if (objectTranslator != currentObjectManipulator)
				objectTranslator.Hidden = true;
			if (objectRotator != currentObjectManipulator)
				objectRotator.Hidden = true;
			if (objectScaler != currentObjectManipulator)
				objectScaler.Hidden =true;
		}
	}

	private void Unselect()
	{
		// reset selection to the DummyNode(ComponentLogic node in the world)
		obj = null;

		// hide all manipulators
		objectTranslator.Hidden = true;
		objectRotator.Hidden = true;
		objectScaler.Hidden = true;
	}

	private void SetManipulatorsBasis()
	{
		if (obj)
		{
			if (localBasis)
			{
				// reset the basis of manipulators to object's local basis
				objectRotator.Basis = obj.WorldTransform;
				objectTranslator.Basis = obj.WorldTransform;
				objectScaler.Basis = obj.WorldTransform;
			}
			else
			{
				// resets the basis of manipulators to world basis
				objectRotator.Basis = Mat4.IDENTITY;
				objectTranslator.Basis = Mat4.IDENTITY;
				objectScaler.Basis = Mat4.IDENTITY;
			}
		}
	}
}
