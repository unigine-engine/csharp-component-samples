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

[Component(PropertyGuid = "5db3a2ae745d8a35e96d1610b553722fd7c179c2")]
public class CameraPersecutorTarget : Component
{
	public float radius = 5.0f;
	public float speed = 0.2f;

	private void Update()
	{
		float x = radius * MathLib.Cos(Game.Time * speed);
		float y = radius * MathLib.Sin(Game.Time * speed);

		node.WorldPosition = new Vec3(x, y, 0);
	}
}
