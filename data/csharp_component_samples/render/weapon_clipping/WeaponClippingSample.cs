using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "1c692232a97a420aa8ff22fad0d3f4984bc0ce17")]
public class WeaponClippingSample : Component
{
	[ShowInEditor]
	private WeaponClipping weaponClipping = null;

	private SampleDescriptionWindow sampleDescriptionWindow = new SampleDescriptionWindow();
	private Input.MOUSE_HANDLE mouse_handle = Input.MOUSE_HANDLE.USER;

	void Init()
	{
		sampleDescriptionWindow.createWindow();

		mouse_handle = Input.MouseHandle;
		Input.MouseHandle = Input.MOUSE_HANDLE.GRAB;

		sampleDescriptionWindow.addBoolParameter("Skip Shadow", null, true, OnShadowsCheckboxChanged);		
	}

	private void OnShadowsCheckboxChanged(bool is_checked)
	{
		if (weaponClipping == null)
		{
			Log.Message("WeaponClippingSample::OnShadowsCheckboxChanged(): weaponClippingNode is not set");
			return;
		}

		int flags = Viewport.SKIP_VELOCITY_BUFFER;
		if (is_checked)
		{
			flags |= Viewport.SKIP_SHADOWS;
		}
		weaponClipping.RenderViewport.SkipFlags = flags;
	}

	private void Shutdown()
	{
		Input.MouseHandle = mouse_handle;
		sampleDescriptionWindow.shutdown();
	}
}
