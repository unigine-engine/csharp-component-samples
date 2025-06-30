using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "462e74d31324e6e2fb51941f826b2ec81a74eb07")]
public class DescriptionWindowCreator : Component
{
	protected SampleDescriptionWindow sampleDescriptionWindow = new SampleDescriptionWindow();

	[ParameterMask(MaskType = ParameterMaskAttribute.TYPE.GENERAL)]
	public int window_align = 0;
	public int window_width = 400;

	void Init()
	{
		sampleDescriptionWindow.createWindow(window_align, window_width);
	}

	void Shutdown()
	{
		sampleDescriptionWindow.shutdown();
	}

	public SampleDescriptionWindow getWindow()
	{
		return sampleDescriptionWindow;
	}
}
