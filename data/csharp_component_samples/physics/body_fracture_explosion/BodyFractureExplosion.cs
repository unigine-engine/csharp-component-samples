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

using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "ac14cbc709b25cc2da74edb278f89a4ac116cbac")]
public class BodyFractureExplosion : Component
{
	public float MaxRadius = 10.0f;
	public float Speed = 100.0f;
	public float Force = 100.0f;

	private float radius = 0.0f;

	public void Explode()
	{
		radius = 0.0f;
	}

	void Init()
	{
		radius = MaxRadius;	
	}
	
	void Update()
	{
		if (MaxRadius == 0 || radius > MaxRadius)
			return;

		BoundSphere sphere = new(new vec3(node.WorldPosition), radius);
		var actualForce = Force * (1 - radius / MaxRadius);
		Visualizer.RenderBoundSphere(sphere, mat4.IDENTITY, vec4.RED, 0.01f);

		List<Object> objects = [];
		if (World.GetIntersection(new WorldBoundSphere(sphere), objects))
		{
			foreach (var obj in objects)
			{
				var body = obj.Body;
				if (body == null)
					continue;

				var dir = new vec3(obj.WorldPosition - node.WorldPosition);
				if (dir.Length2 != 0)
					dir.Normalize();

				var fracture = GetComponent<BodyFractureUnit>(obj);
				fracture?.Crack(obj.WorldPosition, -dir, actualForce);

				var rigid = body as BodyRigid;
				rigid?.AddLinearImpulse(dir * actualForce);
			}
		}

		radius += Speed * Game.IFps;
	}
}
