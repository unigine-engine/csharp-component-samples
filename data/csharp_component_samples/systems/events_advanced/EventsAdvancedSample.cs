using Unigine;

[Component(PropertyGuid = "eb20e44b63e9ec504c235d116762b533005661fd")]
public class EventsAdvancedSample : Component
{
	public Event<float> EventRotateX { get { return rotate_x_event; } }
	public Event<float> EventRotateY { get { return rotate_y_event; } }
	public Event<float> EventRotateZ { get { return rotate_z_event; } }
	public Event<float, float, float, EventsAdvancedSample> EventRotate { get { return rotate_event; } }

	private readonly EventInvoker<float> rotate_x_event = new();
	private readonly EventInvoker<float> rotate_y_event = new();
	private readonly EventInvoker<float> rotate_z_event = new();
	private readonly EventInvoker<float, float, float, EventsAdvancedSample> rotate_event = new();

	public vec3 rotation_speed = new(3.0f, 3.0f, 3.0f);

	private void Update()
	{
		if (Console.Active)
			return;

		if (Input.IsKeyPressed(Input.KEY.T))
			rotate_x_event.Run(rotation_speed.x);
		if (Input.IsKeyPressed(Input.KEY.Y))
			rotate_y_event.Run(rotation_speed.y);
		if (Input.IsKeyPressed(Input.KEY.U))
			rotate_z_event.Run(rotation_speed.z);
		if (Input.IsKeyPressed(Input.KEY.I))
			rotate_event.Run(rotation_speed.x, rotation_speed.y, rotation_speed.z, this);
	}
}
