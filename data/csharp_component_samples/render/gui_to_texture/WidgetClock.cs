using System;
using Unigine;

/// <summary>
/// This sample demonstrates how the <c> GuiToTexture </c> component can be used to render custom widgets
/// In this component we're going to use <c> GuiToTexture </c> with auto update disabled and will update gui manually in SetTime method
/// </summary>
[Component(PropertyGuid = "168dc2de5e12d126e849878199728ecf744bc3c5")]
public class WidgetClock : Component
{
	void Init()
	{
		guiToTexture = ComponentSystem.GetComponent<GuiToTexture>(node);
		if (guiToTexture == null)
		{
			Log.Error("WidgetClock.Init(): No GuiToTexture component found\n");
			return;
		}

		// Get our custom gui
		var gui = guiToTexture.Gui;
		widgetTimer = new WidgetLabel(gui) { FontSize = 150, FontColor = vec4.RED };

		// Disable auto update, because we will update gui manually in <c> SetTime </c> method
		guiToTexture.AutoUpdateEnabled = false;

		// Add widget as a child in gui
		gui.AddChild(widgetTimer, Gui.ALIGN_OVERLAP);

		CenterPosition = guiToTexture.TextureResolution / 2;
		previousTime = DateTime.Now.TimeOfDay;


		// Set time and update gui
		SetTime(previousTime);
		// Now we don't need to interact with GuiToTexture, it will be updated on its own
		// starting from here, we will just update the state of our custom widget
	}

	void Update()
	{
		var now = DateTime.Now.TimeOfDay;
		if (now - previousTime < TimeSpan.FromSeconds(1))
		{
			return;
		}

		SetTime(now);
		previousTime = now;
	}


	public ivec2 CenterPosition
	{
		get
		{
			return centerPosition;
		}
		set
		{
			centerPosition = value;
			AdjustScreenPosition();
		}
	}

	private void SetTime(TimeSpan time)
	{
		widgetTimer.Text = time.ToString("hh\\:mm\\:ss");
		AdjustScreenPosition();
		guiToTexture.RenderToTexture();
	}


	private void AdjustScreenPosition()
	{
		ivec2 widgetSize;
		widgetSize.y = widgetTimer.GetTextRenderSize(widgetTimer.Text).y;
		widgetSize.x = widgetTimer.GetTextRenderSize(widgetTimer.Text).x;
		widgetTimer.SetPosition(centerPosition.x - widgetSize.x / 2, centerPosition.y - widgetSize.y / 2);
	}

	private ivec2 centerPosition;
	private WidgetLabel widgetTimer;
	private TimeSpan previousTime;
	private GuiToTexture guiToTexture;

}
