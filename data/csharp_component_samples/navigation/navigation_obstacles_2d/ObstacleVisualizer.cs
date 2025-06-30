using Unigine;

[Component(PropertyGuid = "51c164503d08eb6fe188220240177979b4d75484")]
public class ObstacleVisualizer : Component
{
	private Obstacle obstacle = null;

	private void Init()
	{
		obstacle = node as Obstacle;

		if (obstacle)
			Visualizer.Enabled = true;
	}
	
	private void Update()
	{
		if (obstacle)
			obstacle.RenderVisualizer();
	}

	private void Shutdown()
	{
		Visualizer.Enabled = false;
	}
}
