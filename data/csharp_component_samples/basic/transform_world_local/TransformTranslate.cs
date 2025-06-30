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

[Component(PropertyGuid = "ee05af9cbced064be24d272e4b7444b51a638e90")]
public class TransformTranslate : Component
{
	public dvec3 linearVelocity = dvec3.FORWARD;

	private float time = 0.0f;
	private float timeSign = 1.0f;

	private void Update()
	{
		if (time < -3.0f || time > 3.0f)
			timeSign *= -1.0f;

		time += Game.IFps * timeSign;

		node.Translate(new Vec3(linearVelocity * Game.IFps * timeSign));
	}
}
