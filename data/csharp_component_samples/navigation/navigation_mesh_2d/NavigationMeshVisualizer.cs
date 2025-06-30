using Unigine;

[Component(PropertyGuid = "a556356231a6b559e3d89ee4a201dcf3c84afdb6")]
public class NavigationMeshVisualizer : Component
{
	private NavigationMesh navigationMesh = null;

	private void Init()
	{
		navigationMesh = node as NavigationMesh;

		if (navigationMesh)
			Visualizer.Enabled = true;
	}
	
	private void Update()
	{
		if (navigationMesh)
			navigationMesh.RenderVisualizer();
	}

	private void Shutdown()
	{
		Visualizer.Enabled = false;
	}
}
