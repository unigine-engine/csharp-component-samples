using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "d16617a2e6277e064d73c9810c14f58d672051de")]
public class BodyFractureFallingSpheresSample : Component
{
	void Init()
	{
		Visualizer.Enabled = true;
	}
	
	void Shutdown()
	{
		Visualizer.Enabled = false;
	}
}
