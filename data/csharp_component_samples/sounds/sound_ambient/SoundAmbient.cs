using System;
using Unigine;

[Component(PropertyGuid = "45d24605667f7fa0b4622e7f0e58e9d2399e7f09")]
public class SoundAmbient : Component
{
	[ParameterFile]
	public string soundFile = "";

	private AmbientSource ambientSource = null;
	private bool isStream = false;

	private SampleDescriptionWindow window = null;
	private void Init()
	{
		ambientSource = new AmbientSource(soundFile);
		if (!ambientSource)
			return;

		ambientSource.Loop = 1;
		ambientSource.Gain = 1.0f;
		ambientSource.Play();

		window = new SampleDescriptionWindow();
		window.createWindow();

		Widget parameters = window.getParameterGroupBox();

		WidgetButton button_play = new WidgetButton("Play");
		WidgetButton button_stop = new WidgetButton("Stop");
		button_play.EventClicked.Connect(() =>
		{
			ambientSource.Play();
		});
		button_stop.EventClicked.Connect(() =>
		{
			ambientSource.Stop();
		});
		WidgetHBox buttons = new WidgetHBox();
		buttons.SetSpace(10, 0);
		buttons.AddChild(button_play);
		buttons.AddChild(button_stop);
		parameters.AddChild(buttons, Gui.ALIGN_LEFT);

		window.addBoolParameter("Loop:", "Loop", Convert.ToBoolean(ambientSource.Loop), (bool active) =>
		{
			ambientSource.Loop = Convert.ToInt32(active);
		});
		window.addBoolParameter("Stream:", "Stream", isStream, (bool active) =>
		{
			ChangeSourceType();
		});

		window.addFloatParameter("Gain:", "Gain", ambientSource.Gain, 0.0f, 1.0f, (float val) =>
		{
			ambientSource.Gain = val;
		});
		window.addFloatParameter("Pitch:", "Pitch", ambientSource.Pitch, 0.1f, 5.0f, (float val) =>
		{
			ambientSource.Pitch = val;
		});
	}

	private void Update()
	{
	}

	private void Shutdown()
	{
		ambientSource.DeleteLater();
		window.shutdown();
	}

	private void ChangeSourceType()
	{
		int isLoop = ambientSource.Loop;
		bool isPlaying = ambientSource.IsPlaying;
		float gain = ambientSource.Gain;
		float pitch = ambientSource.Pitch;

		ambientSource.DeleteLater();
		ambientSource = null;

		isStream = !isStream;
		if (isStream)
			ambientSource = new AmbientSource(soundFile, 1);
		else
			ambientSource = new AmbientSource(soundFile, 0);

		ambientSource.Loop = isLoop;
		ambientSource.Gain = gain;
		ambientSource.Pitch = pitch;

		if (isPlaying)
			ambientSource.Play();
	}

}
