#region Math Variables
#if UNIGINE_DOUBLE
using Scalar = System.Double;
using Vec2 = Unigine.dvec2;
using Vec3 = Unigine.dvec3;
using Vec4 = Unigine.dvec4;
using Mat4 = Unigine.dmat4;
#else
using Scalar = System.Single;
using Vec2 = Unigine.vec2;
using Vec3 = Unigine.vec3;
using Vec4 = Unigine.vec4;
using Mat4 = Unigine.mat4;
using WorldBoundBox = Unigine.BoundBox;
using WorldBoundSphere = Unigine.BoundSphere;
using WorldBoundFrustum = Unigine.BoundFrustum;
#endif
#endregion

using Unigine;

[Component(PropertyGuid = "871eb0829003b5f9cc716704dca0c693e4129ad6")]
public class SoundReverbController : Component
{
	public SoundSource soundSource = null;

	private SoundReverb reverb = null;
	private float reverbPower = 0.5f;

	SampleDescriptionWindow window = null;

	private void Init()
	{
		reverb = new SoundReverb(new vec3(20.0f, 20.0f, 20.0f));
		reverb.WorldTransform = Mat4.IDENTITY;
		reverb.Threshold = new vec3(10.0f, 10.0f, 10.0f);

		UpdateReverbSettings();

		Visualizer.Enabled = true;

		window = new SampleDescriptionWindow();
		window.createWindow();

		window.addFloatParameter("Gain:", "Gain", reverbPower, 0.0f, 1.0f, (float val) =>
		{
			reverbPower = val;
			UpdateReverbSettings();
		});
	}

	private void Update()
	{
		if (!reverb || !soundSource)
			return;

		reverb.RenderVisualizer();
		soundSource.RenderVisualizer();
	}

	private void Shutdown()
	{
		Visualizer.Enabled = false;
		window.shutdown();
	}

	private void UpdateReverbSettings()
	{
		reverb.Density = MathLib.Clamp(1.0f - reverbPower, 0.0f, 1.0f);
		reverb.Diffusion = MathLib.Clamp(1.0f - reverbPower, 0.0f, 1.0f);
		reverb.DecayTime = MathLib.Clamp(0.1f + 19.9f * reverbPower, 0.1f, 20.0f);
		reverb.ReflectionGain = MathLib.Clamp(3.16f * reverbPower, 0.0f, 2.16f);
		reverb.LateReverbGain = MathLib.Clamp(10.0f * reverbPower, 0.0f, 10.0f);
	}

}
