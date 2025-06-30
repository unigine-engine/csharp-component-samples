using System;
using System.Collections.Generic;
using Unigine;

#region Math Variables
#if UNIGINE_DOUBLE
using Vec3 = Unigine.dvec3;
using Mat4 = Unigine.dmat4;
#else
using Vec3 = Unigine.vec3;
using Mat4 = Unigine.mat4;
#endif
#endregion

[Component(PropertyGuid = "2552d56023b38ff9364cb250207756c3868aee8b")]
public class DayNightSwitcher : Component
{
	public enum CONTROL_TYPE
	{
		Zenith = 0,
		Time = 1,
	};

	[ShowInEditor]
	[Parameter(Title = "Switch control type")]
	private CONTROL_TYPE switchControlType = CONTROL_TYPE.Zenith;


	[ShowInEditor]
	[Parameter(Title = "Sun controller")]
	private SunController sun = null;

	[ShowInEditor]
	[Parameter(Title = "Sun zenit threshold")]
	private double sunZenitThreshold = 85.0d;
	public double SunZenitThreshold { get { return sunZenitThreshold; } }

	[ShowInEditor]
	[Parameter(Title = "Morning time bound")]
	private ivec2 timeMorning = new ivec2(7, 30);

	[ShowInEditor]
	[Parameter(Title = "Evening time bound")]
	private ivec2 timeEvening = new ivec2(19, 30);
	public ivec2 TimeEvening { get { return timeEvening; } }


	[ShowInEditor]
	[Parameter(Title = "Emission material parameter name")]
	private String emissionMaterialParameterName = "emission_scale";

	[ShowInEditor]
	[Parameter(Title = "Materials that enabled during day")]
	private List<Material> materialsDayEnabled = new List<Material>();

	[ShowInEditor]
	[Parameter(Title = "Materials that enabled during night")]
	private List<Material> materialsNightEnabled = new List<Material>();

	[ShowInEditor]
	[Parameter(Title = "Nodes that enabled during day")]
	private List<Node> nodesDayEnabled = new List<Node>();

	[ShowInEditor]
	[Parameter(Title = "Nodes that enabled during night")]
	private List<Node> nodesNightEnabled = new List<Node>();

	private Dictionary<UGUID, float> defaultEmissionScale = new Dictionary<UGUID, float>();
	private int isDay = -1;

	private void Init()
	{
		if (!sun)
		{
			Log.Error("DayNightSwitchSample::init can't find SunController component on the sun node!\n");
		}
		sun.EventOnTimeChanged.Connect(OnTimeChange);

		foreach (var mat in materialsDayEnabled)
		{
			int param = mat.FindParameter(emissionMaterialParameterName);
			if (param == -1)
			{
				Log.Error("DayNightSwitchSample::init materials_day_enabled got wrong material without emission!\n");
			}
			defaultEmissionScale.Add(mat.GUID, mat.GetParameterFloat(param));
		}

		foreach (var mat in materialsNightEnabled)
		{
			int param = mat.FindParameter(emissionMaterialParameterName);
			if (param == -1)
			{
				Log.Error("DayNightSwitchSample::init materials_night_enabled got wrong material without emission!\n");
			}
			defaultEmissionScale.Add(mat.GUID, mat.GetParameterFloat(param));
		}
		OnTimeChange();
	}

	private void Shutdown()
	{
		defaultEmissionScale.Clear();
		sun.EventOnTimeChanged.Disconnect(OnTimeChange);
	}

	public void SetControlType(CONTROL_TYPE type)
	{
		switchControlType = type;
		OnTimeChange();
	}
	public void SetZenithThreshold(float value)
	{
		value = MathLib.Clamp(value, 0.0f, 180.0f);
		sunZenitThreshold = value;
		OnTimeChange();//so that new threshold would apply instantly
	}
	public void SetMorningControlTime(ivec2 timeMorning)
	{
		this.timeMorning = timeMorning;
		OnTimeChange();//so that change would apply instantly
	}
	public void SetEveningControlTime(ivec2 timeEvening)
	{
		this.timeEvening = timeEvening;
		OnTimeChange();//so that change would apply instantly
	}
	public int GetControlMorningTime() { return timeMorning.x * 60 + timeMorning.y; }
	public int GetControlEveningTime() { return timeEvening.x * 60 + timeEvening.y; }
	private void OnTimeChange()
	{
		switch (switchControlType)
		{
			case CONTROL_TYPE.Zenith:
				{//Zenth angle
					bool day = true;// is day after sun is rotated
					if (sun.node)
					{
						double currentAngle = MathLib.Angle(Vec3.UP, sun.node.GetWorldDirection(MathLib.AXIS.Z));
						day = currentAngle < sunZenitThreshold;
					}

					if ((day ? 1 : 0) != isDay)//isDay initialized value is -1 so that we are always swithcing nodes for the firts check depending on world initial setup
					{
						SwitchNodes(day);
						isDay = day ? 1 : 0;
					}
					break;
				}
			case CONTROL_TYPE.Time:
				{// Time
					int time = ((int)sun.Time / 60);
					bool day = time > (timeMorning.x * 60 + timeMorning.y)
							&& time < (timeEvening.x * 60 + timeEvening.y); // is day after sun is rotated
					if ((day ? 1 : 0) != isDay)//isDay initialized value is -1 so that we are always swithcing nodes for the firts time depending on world initial setup
					{
						SwitchNodes(day);
						isDay = day ? 1 : 0;
					}
					break;
				}
			default:
				break;
		}
	}
	private void SwitchNodes(bool day)
	{
		//Materials
		if (materialsDayEnabled != null)
		{
			foreach (var mat in materialsDayEnabled)
			{
				if (mat != null)
				{
					mat.SetParameterFloat(emissionMaterialParameterName, day ? defaultEmissionScale[mat.GUID] : 0);
				}
				else
				{
					Log.Warning("DayNightSwitcher::on_time_changed: day materail is null\n");
				}
			}
		}

		foreach (var mat in materialsNightEnabled)
		{
			if (mat != null)
			{
				mat.SetParameterFloat(emissionMaterialParameterName, !day ? defaultEmissionScale[mat.GUID] : 0);
			}
			else
			{
				Log.Warning("DayNightSwitcher::on_time_changed: day materail is null\n");
			}
		}

		//Nodes
		foreach (var node in nodesDayEnabled)
		{
			if (node)
			{
				node.Enabled = day;
			}
			else
			{
				Log.Warning("DayNightSwitcher::on_time_changed: got null node \n");
			}
		}
		foreach (var node in nodesNightEnabled)
		{

			if (node)
			{
				node.Enabled = !day;
			}
			else
			{
				Log.Warning("DayNightSwitcher::on_time_changed: got null node \n");
			}
		}
	}
}
