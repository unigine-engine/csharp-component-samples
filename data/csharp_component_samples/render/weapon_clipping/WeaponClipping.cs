using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unigine;

[Component(PropertyGuid = "c4af7e3a794d78510337557ccb69b2ab20b0280e")]
public class WeaponClipping : Component
{
	[ShowInEditor]
	private Player mainPlayer = null;

	[ShowInEditor]
	private Player weaponPlayer = null;

	private Viewport viewport = null;
	public Viewport RenderViewport { get { return viewport; } }

	private Texture texture = null;

	private int currentWidth = 0;
	private int currentHeight = 0;

	private bool isRenderingWeapon = false;

	private EventConnections componentConnections = new EventConnections();

	private void Init()
	{
		EngineWindow main_window = WindowManager.MainWindow;
		if (!main_window)
		{
			Engine.Quit();
			return;
		}

		ivec2 main_size = main_window.ClientRenderSize;

		currentWidth = main_size.x;
		currentHeight = main_size.y;

		viewport = new Viewport();
		viewport.NodeLightUsage = Viewport.USAGE_WORLD_LIGHT;
		viewport.SkipFlags = Viewport.SKIP_VELOCITY_BUFFER | Viewport.SKIP_SHADOWS;

		texture = new Texture();
		CreateTexture2d(ref texture);

		Render.EventBeginPostMaterials.Connect(componentConnections, RenderCallback);
		WindowManager.MainWindow.EventResized.Connect(componentConnections, UpdateScreenSize);
	}

	private void Update()
	{
		weaponPlayer.Transform = mainPlayer.Camera.IModelview;
	}

	private void PostUpdate()
	{
		if (Game.Player != mainPlayer)
			return;

		RenderState.SaveState();
		RenderState.ClearStates();
		RenderState.SetViewport(0, 0, currentWidth, currentHeight);
		var target = Render.GetTemporaryRenderTarget();
		target.BindColorTexture(0, texture);
		target.Enable();
		{
			bool flare = Render.LightsLensFlares;
			Render.LightsLensFlares = false;

			RenderState.ClearBuffer(RenderState.BUFFER_ALL, vec4.ZERO);
			RenderState.FlushStates();

			// render near plane with weapon to texture
			if (texture != null)
			{
				isRenderingWeapon = true;
				viewport.RenderTexture2D(weaponPlayer.Camera, texture);
				isRenderingWeapon = false;
			}

			Render.LightsLensFlares = flare;
		}
		target.Disable();
		target.UnbindColorTexture(0);
		RenderState.RestoreState();
		Render.ReleaseTemporaryRenderTarget(target);
	}

	private void RenderCallback()
	{
		if (Game.Player != mainPlayer)
			return;

		if (isRenderingWeapon)
		{
			// skip render to screen when we rendering weapon into custom texture
			return;
		}

		RenderState.SaveState();
		RenderState.ClearStates();
		RenderState.SetViewport(0, 0, currentWidth, currentHeight);

		var target = Render.GetTemporaryRenderTarget();
		target.BindColorTexture(0, Renderer.TextureColor);

		target.Enable();
		{
			RenderState.SetBlendFunc(RenderState.BLEND_SRC_ALPHA, RenderState.BLEND_ONE_MINUS_SRC_ALPHA);
			RenderState.FlushStates();

			// render texture with weapon to screen
			if (texture != null)
			{
				Render.RenderScreenMaterial("Unigine::render_copy_2d", texture);
			}
		}
		target.Disable();

		target.UnbindColorTexture(0);
		RenderState.RestoreState();
		Render.ReleaseTemporaryRenderTarget(target);
	}

	private void UpdateScreenSize()
	{
		EngineWindow main_window = WindowManager.MainWindow;
		if (!main_window)
		{
			Engine.Quit();
			return;
		}

		ivec2 main_size = main_window.ClientRenderSize;

		int appWidth = main_size.x;
		int appHeight = main_size.y;
		if (appWidth != currentWidth || appHeight != currentHeight)
		{
			currentWidth = appWidth;
			currentHeight = appHeight;
			CreateTexture2d(ref texture);
		}
	}

	private void Shutdown()
	{
		texture = null;
		viewport = null;
		componentConnections.DisconnectAll();
	}

	private void CreateTexture2d(ref Texture texture)
	{
		texture.Create2D(currentWidth, currentHeight, Texture.FORMAT_RGBA8,
			Texture.SAMPLER_FILTER_LINEAR | Texture.SAMPLER_ANISOTROPY_16 | Texture.FORMAT_USAGE_RENDER);
	}
}
