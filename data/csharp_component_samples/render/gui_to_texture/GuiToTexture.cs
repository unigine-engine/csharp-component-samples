using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unigine;
using Object = Unigine.Object;

/// <summary>
/// This sample demonstrates how to render gui onto a custom texture
/// In <c> GuiToTexture </c> component we pass the texture into material's texture slot
/// </summary>
[Component(PropertyGuid = "58738de84f233d8ef90217a386f54c5fa43dd9c7")]
public class GuiToTexture : Component
{
	/// <summary>
	/// Name of surface to which the material is applied
	/// </summary>
	[ShowInEditor] private string surfaceName = "";

	/// <summary>
	/// Names of texture slots in material
	/// </summary>
	[ShowInEditor] private string[] textureSlotNames = [];

	/// <summary>
	/// Resolution of texture that will be created and assigned to material
	/// </summary>
	[ShowInEditor] public ivec2 TextureResolution = new(2048, 2048);

	/// <summary>
	/// Texture in which gui will be rendered
	/// </summary>
	private Texture guiTexture;

	/// <summary>
	/// Render target that will be used to render gui on texture
	/// </summary>
	private RenderTarget renderTarget;

	/// <summary>
	/// If this flag is enabled, gui texture be updated each frame
	/// </summary>
	public bool AutoUpdateEnabled = true;

	/// <summary>
	/// Gui that is rendered
	/// </summary>
	public Gui Gui { get; private set; }

	public void RenderToTexture()
	{
		// Render gui onto texture

		// Save render state and put it at the top of the stack
		// To pop current settings, we will need to call RenderState.RestoreState() at the and of this method
		RenderState.SaveState();

		// Now we clear state, so that our rendered texture won't be affected by other render activities
		RenderState.ClearStates();

		// Set viewport size matching texture resolution
		RenderState.SetViewport(0, 0, TextureResolution.x, TextureResolution.y);

		// Now we bind gui texture to slot 0, because gui renders in slot 0
		renderTarget.BindColorTexture(0, guiTexture);
		// Enable render target
		renderTarget.Enable();

		// Clear texture and fill it with black color
		RenderState.ClearBuffer(RenderState.BUFFER_COLOR, vec4.BLACK);

		// Now we need to perform the whole gui render loop

		// Enable gui so that it will be updated and rendered
		Gui.Enable();

		// Update all widgets
		Gui.Update();

		// Render gui
		Gui.PreRender();
		Gui.Render();

		// Disable gui
		Gui.Disable();

		// Now we need to free render target and unbind texture
		renderTarget.Disable();
		renderTarget.UnbindColorTexture(0);

		// Create texture mipmaps (set of textures of different resolutions to ensure correct rendering at longer distances)
		guiTexture.CreateMipmaps();

		// Pop render state from top of the stack to let render pipeline continue as usual
		RenderState.RestoreState();
	}

	[MethodInit(Order = -1)] // We need to initialize this component before other components that will use this gui
	void Init()
	{
		// Obtain object from the node this component is attached to
		var obj = node as Object;
		if (obj == null)
		{
			Log.Error("GuiToTexture.Init(): component must be assigned to an object");
			return;
		}

		// Find the required surface
		int surface = obj.FindSurface(surfaceName);
		if (surface == -1)
		{
			Log.Error("GuiToTexture.Init(): surface with name %s not found", surfaceName);
			return;
		}


		renderTarget = new RenderTarget();

		// We need to inherit material, because there might be other objects that are using this material
		// and we don't want all objects in the scene to get gui from this component
		Material material = obj.GetMaterialInherit(surface);

		Gui = Gui.Create();
		Gui.Size = TextureResolution;

		guiTexture = new Texture();

		// here we need to specify format of texture: rgba8
		// and set the flag FORMAT_USAGE_RENDER to be able to render into the texture
		// we also need to specify the sampler by setting another flag (bilinear sampler in our case)
		guiTexture.Create2D(TextureResolution.x, TextureResolution.y, Texture.FORMAT_RGBA8,
			Texture.FORMAT_USAGE_RENDER | Texture.SAMPLER_FILTER_BILINEAR);

		for (int i = 0, num_textures = textureSlotNames.Length; i < num_textures; i++)
		{
			material.SetTexture(textureSlotNames[i], guiTexture);
		}
	}

	void Update()
	{
		// Update gui if flag is set
		if (AutoUpdateEnabled)
			RenderToTexture();
	}

}
