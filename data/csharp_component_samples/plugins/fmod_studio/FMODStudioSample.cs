using Unigine;
using Unigine.Plugins.FMOD;
using System;


#if UNIGINE_DOUBLE
using Vec3 = Unigine.dvec3;
using Vec4 = Unigine.dvec4;
using Mat4 = Unigine.dmat4;
#else
using Vec3 = Unigine.vec3;
using Vec4 = Unigine.vec4;
using Mat4 = Unigine.mat4;
#endif

[Component(PropertyGuid = "5cb85699ce485ecd4f4c9e8bead8eb7993f5d703")]
public class FMODStudioSample : Component
{
	private bool pluginInitialized = false;

	private float timer = 0.0f;

	private ObjectMeshDynamic carSphere;
	private ObjectMeshDynamic dopplerSphere;

	private EventInstance engineEvent = null;
	private EventInstance dopplerEngineEvent = null;
	private EventInstance forestEvent = null;
	private VCA envVCA = null;

	private Vec3 velocity;
	private Vec3 startPoint = new Vec3(-5, 80, 0);

	private SampleDescriptionWindow sampleDescriptionWindow = null;
	private WidgetSlider engineSlider;
	private WidgetSlider windForestSlider;
	private WidgetSlider rainForestSlider;
	private WidgetSlider coverForestSlider;
	private WidgetSlider envVCASlider;
	private WidgetSlider dopplerRPMSlider;
	private WidgetSlider dopplerVelocitySlider;
	private WidgetCheckBox showDopplerBoxCheckBox;

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

		// create two spheres: one static (car) and one moving (doppler)
		carSphere = Primitives.CreateSphere(2.0f);
		carSphere.SetMaterialParameterFloat4("albedo_color", new vec4(0.4f, 0.0f, 0.0f, 1.0f), 0);

		dopplerSphere = Primitives.CreateSphere(1.0f);
		dopplerSphere.SetMaterialParameterFloat4("albedo_color", new vec4(0.0f, 4.0f, 0.0f, 1.0f), 0);
		dopplerSphere.WorldPosition = startPoint;

		// initialize FMOD Studio and load sound banks
		FMODStudio studio = FMOD.Studio;
		studio.UseStudioLiveUpdateFlag();
		studio.InitStudio();
		studio.LoadBank(FileSystem.GetAbsolutePath(FileSystem.ResolvePartialVirtualPath("fmod_studio/fmod_banks/Master.bank")));
		studio.LoadBank(FileSystem.GetAbsolutePath(FileSystem.ResolvePartialVirtualPath("fmod_studio/fmod_banks/Master.strings.bank")));
		studio.LoadBank(FileSystem.GetAbsolutePath(FileSystem.ResolvePartialVirtualPath("fmod_studio/fmod_banks/Vehicles.bank")));
		studio.LoadBank(FileSystem.GetAbsolutePath(FileSystem.ResolvePartialVirtualPath("fmod_studio/fmod_banks/SFX.bank")));

		// play ambient forest and engine events
		forestEvent = studio.GetEvent("event:/Ambience/Forest");
		forestEvent.Play();

		engineEvent = studio.GetEvent("event:/Vehicles/Car Engine");
		engineEvent.Play();

		// doppler effect setup
		dopplerEngineEvent = studio.GetEvent("event:/Vehicles/Car Engine");
		dopplerEngineEvent.SetParent(dopplerSphere);
		dopplerEngineEvent.SetParameter("RPM", 4000);

		// initial movement direction
		velocity = -Vec3.FORWARD;

		// load VCA for master environmental volume
		envVCA = studio.GetVCA("vca:/Environment");

		Visualizer.Enabled = true;
		InitDescriptionWindow();
	}
	
	void Update()
	{
		if (!pluginInitialized)
			return;

		float t = Game.Time;
		float dt = Game.IFps;

		// Doppler simulation logic
		if (showDopplerBoxCheckBox.Checked)
		{
			dopplerSphere.Enabled = true;

			// reset position after 2.5 seconds
			if (timer >= 2.5f)
			{
				dopplerSphere.WorldPosition = startPoint;
				timer = 0.0f;
			}
			timer += dt;

			// restart Doppler sound if not playing
			if (!dopplerEngineEvent.IsPlaying && !dopplerEngineEvent.IsStarting)
			{
				dopplerEngineEvent.Play();
			}

			// move the Doppler object and update sound velocity
			dopplerSphere.WorldPosition = dopplerSphere.WorldPosition + velocity;
			Visualizer.RenderMessage3D(dopplerSphere.WorldPosition, vec3.ZERO, "Doppler", vec4.WHITE, 0, 20);
		}
		else
		{
			dopplerEngineEvent.Stop();
			dopplerSphere.Enabled = false;
		}

		// show label above the car
		Visualizer.RenderMessage3D(carSphere.WorldPosition, vec3.ZERO, "Car", vec4.WHITE, 0, 20);
	}

	void Shutdown()
	{
		// release all FMOD events
		if (engineEvent)
		{
			engineEvent.Release();
			engineEvent = null;
		}

		if (dopplerEngineEvent)
		{
			dopplerEngineEvent.Release();
			dopplerEngineEvent = null;
		}

		if (forestEvent)
		{
			forestEvent.Release();
			forestEvent = null;
		}

		if (envVCA)
		{
			envVCA.Release();
			envVCA = null;
		}

		Unigine.Console.Run("plugin_unload UnigineFMOD");
		pluginInitialized = false;

		carSphere.DeleteLater();
		dopplerSphere.DeleteLater();
		Visualizer.Enabled = false;
		sampleDescriptionWindow.shutdown();
	}

	void InitDescriptionWindow()
	{
		// create GUI tabs and controls
		WidgetGroupBox parametersGroupbox = sampleDescriptionWindow.getParameterGroupBox();
		WidgetTabBox tab = new WidgetTabBox(4, 4);
		parametersGroupbox.AddChild(tab, Gui.ALIGN_EXPAND);

		// ambience tab
		{
			tab.AddTab("Ambience");
			windForestSlider = new WidgetSlider();
			var wind_label = new WidgetLabel("Wind");
			tab.AddChild(wind_label, Gui.ALIGN_EXPAND);
			tab.AddChild(windForestSlider, Gui.ALIGN_EXPAND);
			windForestSlider.EventChanged.Connect(WindForestSliderChanged);

			rainForestSlider = new WidgetSlider();
			var forest_label = new WidgetLabel("Rain");
			tab.AddChild(forest_label, Gui.ALIGN_EXPAND);
			tab.AddChild(rainForestSlider, Gui.ALIGN_EXPAND);
			rainForestSlider.EventChanged.Connect(RainForestSliderChanged);

			coverForestSlider = new WidgetSlider();
			var cover_label = new WidgetLabel("Cover");
			tab.AddChild(cover_label, Gui.ALIGN_EXPAND);
			tab.AddChild(coverForestSlider, Gui.ALIGN_EXPAND);
			coverForestSlider.EventChanged.Connect(CoverForestSliderChanged);
		}

		// engine tab
		{
			tab.AddTab("Engine");
			engineSlider = new WidgetSlider();
			engineSlider.MinValue = 0;
			engineSlider.MaxValue = 8000;
			var label = new WidgetLabel("RPM");
			tab.AddChild(label, Gui.ALIGN_EXPAND);
			tab.AddChild(engineSlider, Gui.ALIGN_EXPAND);
			engineSlider.EventChanged.Connect(EngineSliderChanged);
		}

		// doppler tab
		{
			tab.AddTab("Doppler");
			showDopplerBoxCheckBox = new WidgetCheckBox();
			showDopplerBoxCheckBox.Checked = false;
			var label = new WidgetLabel("Show Doppler Effect");
			tab.AddChild(label, Gui.ALIGN_EXPAND);
			tab.AddChild(showDopplerBoxCheckBox, Gui.ALIGN_EXPAND);

			dopplerRPMSlider = new WidgetSlider();
			dopplerRPMSlider.MaxValue = 8000;
			dopplerRPMSlider.Value = 4000;
			tab.AddChild(new WidgetLabel("RPM"), Gui.ALIGN_EXPAND);
			tab.AddChild(dopplerRPMSlider, Gui.ALIGN_EXPAND);

			dopplerVelocitySlider = new WidgetSlider();
			tab.AddChild(new WidgetLabel("Velocity"), Gui.ALIGN_EXPAND);
			tab.AddChild(dopplerVelocitySlider, Gui.ALIGN_EXPAND);

			dopplerRPMSlider.EventChanged.Connect(DopplerRPMSliderChanged);
			dopplerVelocitySlider.EventChanged.Connect(DopplerVelocitySliderChanged);
			dopplerVelocitySlider.Value = 5;
		}

		// vca tab
		{
			tab.AddTab("VCA");
			envVCASlider = new WidgetSlider();
			envVCASlider.Value = 100;
			var label = new WidgetLabel("Sounds Volume");
			tab.AddChild(label, Gui.ALIGN_EXPAND);
			tab.AddChild(envVCASlider, Gui.ALIGN_EXPAND);
			envVCASlider.EventChanged.Connect(EnvVCASliderChanged);
		}

		parametersGroupbox.Arrange();
	}

	void EnvVCASliderChanged()
	{
		// adjust environment volume
		envVCA.Volume = envVCASlider.Value * 0.01f;
	}

	void EngineSliderChanged()
	{
		// set RPM for engine event
		engineEvent.SetParameter("RPM", Convert.ToSingle(engineSlider.Value));
	}

	void WindForestSliderChanged()
	{
		// set wind intensity in forest ambience
		forestEvent.SetParameter("Wind", windForestSlider.Value * 0.01f);
	}

	void RainForestSliderChanged()
	{
		// set rain intensity in forest ambience
		forestEvent.SetParameter("Rain", rainForestSlider.Value * 0.01f);
	}

	void CoverForestSliderChanged()
	{
		// set cover parameter in forest ambience
		forestEvent.SetParameter("Cover", coverForestSlider.Value * 0.01f);
	}

	void DopplerRPMSliderChanged()
	{
		// set RPM for Doppler engine event
		dopplerEngineEvent.SetParameter("RPM", Convert.ToSingle(dopplerRPMSlider.Value));
	}

	void DopplerVelocitySliderChanged()
	{
		// adjust velocity of Doppler object
		velocity.y = -dopplerVelocitySlider.Value * 0.1f;
	}
}
