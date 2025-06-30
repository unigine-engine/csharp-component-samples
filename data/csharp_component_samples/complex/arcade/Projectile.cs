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

[Component(PropertyGuid = "bfa1e43dc28cd8785f8dc1777656306a5073a4c1")]
public class Projectile : Component
{
	public float moveSpeed = 5.0f;

	[ParameterFile(Filter = ".node")]
	public string bulletSpawnFx;

	[ParameterFile(Filter = ".node")]
	public string bulletHitFx;

	private WorldIntersectionNormal intersection = null;

	private void Init()
	{
		if (bulletSpawnFx != "")
		{
			Node bulletSpawn = World.LoadNode(bulletSpawnFx);
			bulletSpawn.WorldPosition = node.WorldPosition;
		}

		intersection = new WorldIntersectionNormal();
	}

	private void Update()
	{
		Vec3 oldPosition = node.WorldPosition;

		vec3 direction = node.GetWorldDirection(MathLib.AXIS.Y);
		node.WorldPosition += direction * moveSpeed * Game.IFps;

		WorldIntersectionNormal intersection = new WorldIntersectionNormal();
		Unigine.Object hitObj = World.GetIntersection(oldPosition, node.WorldPosition, 0xFFFFFF, intersection);
		if (hitObj)
		{
			if (hitObj.Name == "turret" || hitObj.Name == "bullet")
				return;

			Robo robo = hitObj.GetComponent<Robo>();
			if (robo != null)
				robo.Hit(20);

			if (bulletHitFx != "")
			{
				Node bulletHit = World.LoadNode(bulletHitFx);
				bulletHit.WorldPosition = intersection.Point;
				bulletHit.SetWorldDirection(intersection.Normal, vec3.UP, MathLib.AXIS.Y);
			}
			node.DeleteLater();
		}
	}

}
