using Unigine;
using static Unigine.MathLib;

#region Math Variables
#if UNIGINE_DOUBLE
using Vec3 = Unigine.dvec3;
using Mat4 = Unigine.dmat4;
#else
using Vec3 = Unigine.vec3;
using Mat4 = Unigine.mat4;
#endif
#endregion


[Component(PropertyGuid = "e9f7147ab66bfdcf2382a8eac3bb09a65652d53d")]
public class SunController : Component
{
	public const string COMPONENT_DESCRIPTION = "This component controls sun based on its own game time and various adjustable parameters";

	[ShowInEditor]
	[Parameter(Title = "Is sun moving continuously")]
	public bool isContinuous = true;
	public bool IsContinuous { get { return isContinuous; } set { isContinuous = value; } }


	[ShowInEditor]
	[Parameter(Title = "Scale of continuous time rotation")]
	private float timescale = 2000.0f;
	public float Timescale { get { return timescale; } set { timescale = value; } }

	private quat sunInitTilt = quat.IDENTITY;

	private double currentTime = 720 * 60; // in secons
	/// <summary>
	/// Returns current time in seconds
	/// </summary>
	/// <returns></returns>
	public double Time { get { return currentTime; } }

	private const int maxTimeSec = 60 * 60 * 24; // 60 minutes in 24 hours
	public int MaxTimeSec => maxTimeSec;

	private EventInvoker<double> timeChangedEvent = new EventInvoker<double>();

	public Event<double> EventOnTimeChanged
	{
		get
		{
			return timeChangedEvent;
		}
	}

	public void SetTime(double time)
	{
		currentTime = time;
		RefreshSunPostion();
		timeChangedEvent.Run((int)currentTime);
	}


	private void Init()
	{
		sunInitTilt = node.GetWorldRotation();
	}

	private void Update()
	{
		if (IsContinuous)
		{
			currentTime += Game.IFps * Timescale;
			if (currentTime > maxTimeSec)
				currentTime -= maxTimeSec;//so we wont loose delta time 
			RefreshSunPostion();
			timeChangedEvent.Run(currentTime); // displaying only integer part
		}
	}

	private void RefreshSunPostion()
	{
		//converting time to rotation
		double time = currentTime % maxTimeSec;
		double k = MathLib.InverseLerp(0.0, maxTimeSec, time);
		float angle = (float)MathLib.Lerp(-180, 180.0, k);
		node.SetWorldRotation(sunInitTilt * new quat(angle, 0.0f, 0.0f));
	}
}
