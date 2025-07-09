using System;
using Unigine;
using Unigine.Plugins.FMOD;

[Component(PropertyGuid = "95d526786760889d8859911ec8ef8bf20e579a00")]
public class FMODCoreSample : Component
{
	private bool pluginInitialized = false;

	private ObjectMeshDynamic carSphere = null;

	private Unigine.Plugins.FMOD.Sound musicSound = null;
	private Unigine.Plugins.FMOD.Sound musicSound3D = null;
	private Channel musicChannel = null;
	private Channel musicChannel3D = null;

	private SampleDescriptionWindow sampleDescriptionWindow = null;
	private WidgetSlider musicPositionSlider;
	private WidgetSlider distortionSlider;
	private WidgetSlider volumeSlider;

	void Init()
	{
		sampleDescriptionWindow = new SampleDescriptionWindow();
		sampleDescriptionWindow.createWindow(Gui.ALIGN_LEFT, 500);

		// load the FMOD plugin
		if (Engine.FindPlugin("UnigineFMOD") == -1)
			Engine.AddPlugin("UnigineFMOD");
		if (!FMOD.CheckPlugin())
		{
			WidgetGroupBox parametersGroupbox = sampleDescriptionWindow.getParameterGroupBox();

			var infoLabel = new WidgetLabel();
			infoLabel.FontWrap = 1;
			infoLabel.Text = "Cannot find FMOD plugin. Please check UnigineFMOD and fmod.dll, fmodL.dll, fmodstudio.dll, fmodstudioL.dll (You can download these files from official site) in bin directory.";

			parametersGroupbox.AddChild(infoLabel);

			return;
		}

		pluginInitialized = true;

		// initialize FMOD Core with 1024 channels in NORMAL mode
		FMODCore core = FMOD.Core;
		core.InitCore(1024, FMODEnums.INIT_FLAGS.NORMAL);

		// load a 2D sound file
		musicSound = core.CreateSound(
			FileSystem.GetAbsolutePath(FileSystem.ResolvePartialVirtualPath("fmod_core/sounds/soundtrack.oga")),
			FMODEnums.FMOD_MODE._2D);

		// load the same file as a 3D sound
		musicSound3D = core.CreateSound(
			FileSystem.GetAbsolutePath(FileSystem.ResolvePartialVirtualPath("fmod_core/sounds/soundtrack.oga")),
			FMODEnums.FMOD_MODE._3D);

		// create a sphere to visualize the 3D sound source
		carSphere = Primitives.CreateSphere(1.0f);
		carSphere.SetMaterialParameterFloat4("albedo_color", new vec4(0.4f, 0.0f, 0.0f, 1.0f), 0);

		Visualizer.Enabled = true;
		InitDescriptionWindow();
	}
	
	void Update()
	{
		if (!pluginInitialized)
			return;

		// display the "Music 3D" label above the car sphere in the 3D world
		var len = musicSound.GetLength(FMODEnums.TIME_UNIT.MS);
		Visualizer.RenderMessage3D(carSphere.WorldPosition, vec3.ZERO, "Music 3D", vec4.WHITE, 0, 25);

		// update music progress slider
		uint pos;
		if (musicChannel)
		{
			musicChannel.GetPositionTimeLine(out pos, FMODEnums.TIME_UNIT.MS);
			int progress = Convert.ToInt32(pos / Convert.ToSingle(len) * 100);
			if (progress >= 100)
			{
				musicChannel.SetPositionTimeLine(0, FMODEnums.TIME_UNIT.MS);
				StopMusic();
				progress = 0;
			}
			musicPositionSlider.Value = progress;
		}
	}

	void Shutdown()
	{
		// release all FMOD sounds and channels
		if (musicSound)
		{
			musicSound.Release();
			musicSound = null;
		}

		if (musicSound3D)
		{
			musicSound3D.Release();
			musicSound3D = null;
		}

		if (musicChannel)
		{
			musicChannel.Release();
			musicChannel = null;
		}

		if (musicChannel3D)
		{
			musicChannel3D.Release();
			musicChannel3D = null;
		}
		if (pluginInitialized)
		{
			carSphere.DeleteLater();
		}

		Unigine.Console.Run("plugin_unload UnigineFMOD");
		pluginInitialized = false;

		Visualizer.Enabled = false;
		sampleDescriptionWindow.shutdown();
	}

	void InitDescriptionWindow()
	{
		// create GUI tabs and controls
		WidgetGroupBox parametersGroupbox = sampleDescriptionWindow.getParameterGroupBox();
		WidgetTabBox tab = new WidgetTabBox(4, 4);
		parametersGroupbox.AddChild(tab, Gui.ALIGN_EXPAND);

		// tab music
		{
			tab.AddTab("Music");

			// playback controls
			var playButton = new WidgetButton("Play");
			var stopButton = new WidgetButton("Stop");
			var pauseButton = new WidgetButton("Pause/Resume");
			var plusButton = new WidgetButton("+ 10 sec");
			var minusButton = new WidgetButton("- 10 sec");

			var hbox = new WidgetHBox();
			playButton.EventClicked.Connect(PlayMusic);
			stopButton.EventClicked.Connect(StopMusic);
			pauseButton.EventClicked.Connect(TogglePauseMusic);

			minusButton.EventClicked.Connect(MinusMS);
			plusButton.EventClicked.Connect(PlusMS);

			musicPositionSlider = new WidgetSlider();

			distortionSlider = new WidgetSlider();
			distortionSlider.EventChanged.Connect(DistortionChanged);

			volumeSlider = new WidgetSlider();
			volumeSlider.EventChanged.Connect(VolumeChanged);
			volumeSlider.Value = 100;
			hbox.AddChild(minusButton);
			hbox.AddChild(plusButton);

			tab.AddChild(new WidgetLabel("Time Line"), Gui.ALIGN_EXPAND);
			tab.AddChild(musicPositionSlider, Gui.ALIGN_EXPAND);


			tab.AddChild(hbox, Gui.ALIGN_EXPAND);
			tab.AddChild(playButton, Gui.ALIGN_EXPAND);
			tab.AddChild(stopButton, Gui.ALIGN_EXPAND);
			tab.AddChild(pauseButton, Gui.ALIGN_EXPAND);

			tab.AddChild(new WidgetLabel("Distortion Mix"), Gui.ALIGN_EXPAND);
			tab.AddChild(distortionSlider, Gui.ALIGN_EXPAND);

			tab.AddChild(new WidgetLabel("Volume"), Gui.ALIGN_EXPAND);
			tab.AddChild(volumeSlider, Gui.ALIGN_EXPAND);
		}

		// tab music 3D
		{
			tab.AddTab("Music 3D");
			var playButton = new WidgetButton("Play");
			var stopButton = new WidgetButton("Stop");
			var pauseButton = new WidgetButton("Pause/Resume");

			var hbox = new WidgetHBox();
			playButton.EventClicked.Connect(PlayMusic3D);
			stopButton.EventClicked.Connect(StopMusic3D);
			pauseButton.EventClicked.Connect(TogglePauseMusic3D);

			tab.AddChild(playButton, Gui.ALIGN_EXPAND);
			tab.AddChild(stopButton, Gui.ALIGN_EXPAND);
			tab.AddChild(pauseButton, Gui.ALIGN_EXPAND);
		}

		parametersGroupbox.Arrange();
	}

	void DistortionChanged()
	{
		if (!musicChannel)
		{
			return;
		}

		// adjust distortion effect on the music channel
		musicChannel.GetDSP(0).SetParameterFloat(0, distortionSlider.Value * 0.01f);
	}

	void VolumeChanged()
	{
		if (!musicChannel)
		{
			return;
		}

		// adjust volume on the music channel
		musicChannel.Volume = volumeSlider.Value * 0.01f;
	}

	void PlusMS()
	{
		if (!musicChannel)
		{
			return;
		}

		// jump forward by 10 seconds in the timeline
		uint currTimeLine;
		uint len = musicSound.GetLength(FMODEnums.TIME_UNIT.MS);
		musicChannel.GetPositionTimeLine(out currTimeLine, FMODEnums.TIME_UNIT.MS);
		if (currTimeLine + 10000 >= len)
		{
			musicChannel.SetPositionTimeLine(0, FMODEnums.TIME_UNIT.MS);
		}
		else
		{
			musicChannel.SetPositionTimeLine(currTimeLine + 10000, FMODEnums.TIME_UNIT.MS);
		}
	}

	void MinusMS()
	{
		if (!musicChannel)
		{
			return;
		}

		// jump backward by 10 seconds in the timeline
		uint currTimeLine;
		musicChannel.GetPositionTimeLine(out currTimeLine, FMODEnums.TIME_UNIT.MS);
		if (currTimeLine < 10000)
		{
			musicChannel.SetPositionTimeLine(0, FMODEnums.TIME_UNIT.MS);
		}
		else
		{
			musicChannel.SetPositionTimeLine(currTimeLine - 10000, FMODEnums.TIME_UNIT.MS);
		}
	}

	void PlayMusic()
	{
		// start 2D music playback if 3D music is not playing
		// adds a distortion DSP effect and sets the volume
		if (!musicChannel3D || !musicChannel3D.IsPlaying)
		{
			StopMusic();

			musicChannel = musicSound.Play();
			musicChannel.AddDSP(0, DSPType.TYPE.DISTORTION).SetParameterFloat(0, distortionSlider.Value * 0.01f);
			musicChannel.Volume = volumeSlider.Value * 0.01f;
		}
	}

	void StopMusic()
	{
		if (!musicChannel)
		{
			return;
		}

		musicChannel.Stop();
		musicChannel = null;
	}

	void TogglePauseMusic()
	{
		if (!musicChannel)
		{
			return;
		}

		musicChannel.Paused = !musicChannel.Paused;
	}

	void PlayMusic3D()
	{
		// start 3D music playback at the car_sphere position
		if (!musicChannel || !musicChannel.IsPlaying)
		{
			StopMusic3D();

			musicChannel3D = musicSound3D.Play();
			musicChannel3D.SetPosition(carSphere.WorldPosition);
		}
	}

	void StopMusic3D()
	{
		if (!musicChannel3D)
		{
			return;
		}

		musicChannel3D.Stop();
		musicChannel3D = null;
	}

	void TogglePauseMusic3D()
	{
		if (!musicChannel3D)
		{
			return;
		}

		musicChannel3D.Paused = !musicChannel3D.Paused;
	}
}
