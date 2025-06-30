using Unigine;

[Component(PropertyGuid = "ec10a5720354c62988b25974f975b6439a92e6c6")]
public class PathRoute2D : Component
{
	public Node startPoint = null;
	public Node finishPoint = null;

	public bool visualizeRoute = false;

	[ParameterColor]
	public vec4 routeColor = vec4.ZERO;

	private PathRoute route = null;

	private void Init()
	{
		if (startPoint && finishPoint)
		{
			// create route to path calculation
			route = new PathRoute();

			// set point radius inside navigation mesh
			route.Radius = 0.5f;

			// enabled for visualization
			Visualizer.Enabled = true;
		}
	}
	
	private void Update()
	{
		// check points for correctness
		if (startPoint && finishPoint)
		{
			// try to calculate path from start to finish
			route.Create2D(startPoint.WorldPosition, finishPoint.WorldPosition);
			if (route.IsReached)
			{
				// if successful, show the current route
				if (visualizeRoute)
					route.RenderVisualizer(routeColor);
			}
			else
				Log.Message($"{node.Name} PathRoute not reached yet\n");
		}
	}

	private void Shutdown()
	{
		Visualizer.Enabled = false;
	}
}
