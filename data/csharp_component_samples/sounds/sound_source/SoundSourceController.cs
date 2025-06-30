using System;
using Unigine;

[Component(PropertyGuid = "5f06580f304e6622e66daf19bd35cda45d1dbcb3")]
public class SoundSourceController : Component
{
	[ParameterFile]
	public string soundFile = "";

	private SoundSource sound = null;

	SampleDescriptionWindow window = null;

	private void Init()
	{
		sound = new SoundSource(soundFile);
		if (!sound)
			return;

		sound.MinDistance = 5.0f;
		sound.MaxDistance = 50.0f;
		sound.Loop = 1;
		sound.Play();

		Visualizer.Enabled = true;

		window = new SampleDescriptionWindow();
		window.createWindow();

		Widget parameters = window.getParameterGroupBox();

		WidgetButton button_play = new WidgetButton("Play");
		WidgetButton button_stop = new WidgetButton("Stop");
		button_play.EventClicked.Connect(() =>
		{
			sound.Play();
		});
		button_stop.EventClicked.Connect(() =>
		{
			sound.Stop();
		});
		WidgetHBox buttons = new WidgetHBox();
		buttons.SetSpace(10, 0);
		buttons.AddChild(button_play);
		buttons.AddChild(button_stop);
		parameters.AddChild(buttons, Gui.ALIGN_LEFT);

		window.addBoolParameter("Loop:", "Loop", Convert.ToBoolean(sound.Loop), (bool active) =>
		{
			sound.Loop = Convert.ToInt32(active);
		});
		window.addBoolParameter("Stream:", "Stream", sound.Stream, (bool active) =>
		{
			sound.Stream = active;
		});

		window.addFloatParameter("Gain:", "Gain", sound.Gain, 0.0f, 1.0f, (float val) =>
		{
			sound.Gain = val;
		});
		window.addFloatParameter("Pitch:", "Pitch", sound.Pitch, 0.1f, 5.0f, (float val) =>
		{
			sound.Pitch = val;
		});
	}

	private void Update()
	{
		sound.RenderVisualizer();
	}

	private void Shutdown()
	{
		Visualizer.Enabled = false;
		window.shutdown();
	}

}
