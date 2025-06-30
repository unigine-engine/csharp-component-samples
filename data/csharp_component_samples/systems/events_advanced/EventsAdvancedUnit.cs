using Unigine;

[Component(PropertyGuid = "fbbbeabda92b13cabac199b32f43a802e6e6cbbf")]
public class EventsAdvancedUnit : Component
{
	public EventsAdvancedSample input_manager;

	private EventConnection rotate_x_connection;
	private EventConnection rotate_y_connection;
	private EventConnection rotate_z_connection;
	private EventConnection rotate_connection;

	private readonly EventConnections connections = new();

	private EventDelegate<float, float, float> rotate_delegate;

	private void Init()
	{
		if (input_manager is null)
			return;

		rotate_delegate += rotate;
		
		// Connect to method with additional arguments
		rotate_x_connection = input_manager.EventRotateX.Connect(rotateNode, 0.0f, 0.0f, node);
		rotate_y_connection = input_manager.EventRotateY.Connect(connections, rotateNodeY, node);
		
		// Connect to lambda
		rotate_z_connection = input_manager.EventRotateZ.Connect(connections, 
		angle => {
			node.Rotate(0, 0, angle);
		});

		// Connect to delegate with discarding last argument
		rotate_connection = input_manager.EventRotate.Connect(rotate_delegate);
	}
	
	private void Shutdown()
	{
		rotate_x_connection.Disconnect();
		rotate_y_connection.Disconnect();
		rotate_z_connection.Disconnect();
		rotate_connection.Disconnect();
	}

	private void rotate(float angleX, float angleY, float angleZ)
	{
		rotateNode(angleX, angleY, angleZ, node);
	}

	private static void rotateNode(float angleX, float angleY, float angleZ, Node node)
	{
		node?.Rotate(angleX, angleY, angleZ);
	}

	private static void rotateNodeY(float angle, Node node)
	{
		rotateNode(0, angle, 0, node);
	}
}
