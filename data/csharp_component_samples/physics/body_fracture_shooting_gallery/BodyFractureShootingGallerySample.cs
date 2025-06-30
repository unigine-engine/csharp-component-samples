using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "fd2486a7ca974d39ef6d932909b6a2636f183941")]
public class BodyFractureShootingGallerySample : Component
{
	private Input.MOUSE_HANDLE mouse_handle;

	void Init()
	{
		Visualizer.Enabled = true;

		mouse_handle = Input.MouseHandle;
		Input.MouseHandle = Input.MOUSE_HANDLE.GRAB;
	}
	
	void Shutdown()
	{
		Visualizer.Enabled = false;
		Input.MouseHandle = mouse_handle;
	}
}
