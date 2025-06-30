using Unigine;

[Component(PropertyGuid = "095d1febeed4344e05aaaf55a742b8b05998744f")]
public class NavigationSectorVisualizer : Component
{
	private NavigationSector navigationSector = null;

	private void Init()
	{
		navigationSector = node as NavigationSector;

		if (navigationSector)
			Visualizer.Enabled = true;
	}

	private void Update()
	{
		if (navigationSector)
			navigationSector.RenderVisualizer();
	}

	private void Shutdown()
	{
		Visualizer.Enabled = false;
	}
}
