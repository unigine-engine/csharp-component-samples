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

using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;
using System.Runtime.Intrinsics.X86;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

[Component(PropertyGuid = "391587e0fb8a19bf8c5b5c12c183cc4986a66914")]
public class Buoyancy : Component
{
	[ShowInEditor]
	private Node centerOfMassNode = null;

	[ShowInEditor]
	private ivec2 volumeGridSize = new ivec2(5, 10);

	[ShowInEditor]
	private float waterDensity = 45.0f;

	[ShowInEditor]
	private float waterLinearDamping = 0.02f;

	[ShowInEditor]
	private float waterAngularDamping = 0.02f;

	[ShowInEditor]
	private bool UseVisualizer = false;

	private ObjectWaterGlobal water = null;

	private BodyRigid bodyRigid = null;
	private ShapeBox volumeBoxShape = null;

	private struct VolumePart
	{
		static public vec3 size = vec3.ZERO;
		static public mat3 rotation = mat3.IDENTITY;

		static public float Width => size.x;
		static public float Depth => size.y;
		static public float Height => size.z;

		static public float HalfWidth => size.x * 0.5f;
		static public float HalfDepth => size.y * 0.5f;
		static public float HalfHeight => size.z * 0.5f;

		static public vec3 AxisX => rotation.AxisX;
		static public vec3 AxisY => rotation.AxisY;
		static public vec3 AxisZ => rotation.AxisZ;

		public Vec3 center;
		public float waterHeight;

		public Vec3 AnchorPoint => center - AxisZ * HalfHeight;
		public bool IsUnderWater => waterHeight != 0.0f;
		public float WaterVolume => size.x * size.y * waterHeight;
	}

	private VolumePart[] volumeParts = null;

	private void Init()
	{
		water = World.GetNodeByType((int)Node.TYPE.OBJECT_WATER_GLOBAL) as ObjectWaterGlobal;
		if (water == null)
			Log.ErrorLine("BuoyComponent Init(): can't find ObjectWaterGlobal on scene!");

		bodyRigid = node.ObjectBodyRigid;
		if (bodyRigid == null)
			Log.ErrorLine("BuoyComponent Init(): body rigid is null");

		for (int i = 0; i < bodyRigid.NumShapes; i++)
		{
			Shape s = bodyRigid.GetShape(i);
			if (s.Type == Shape.TYPE.SHAPE_BOX && string.Equals(s.Name, "volume"))
			{
				volumeBoxShape = (ShapeBox)s;
				break;
			}
		}

		if (volumeBoxShape == null)
			Log.ErrorLine("BuoyComponent Init(): volume shape box is null");

		volumeParts = new VolumePart[volumeGridSize.x * volumeGridSize.y];

		if (centerOfMassNode != null)
		{
			bodyRigid.ShapeBased = false;
			bodyRigid.CenterOfMass = new vec3(centerOfMassNode.Position);
		}
		else
		{
			Log.WarningLine("BuoyComponent Init(): center of mass node is null");
		}
	}

	private void Update()
	{
		if (water == null || bodyRigid == null || volumeBoxShape == null)
			return;

		VolumePart.size = new vec3(volumeBoxShape.Size.x / volumeGridSize.x, volumeBoxShape.Size.y / volumeGridSize.y, volumeBoxShape.Size.z);
		VolumePart.rotation = new mat3(volumeBoxShape.Transform);

		Mat4 t = volumeBoxShape.Transform;
		Vec3 size = new Vec3(volumeBoxShape.Size);

		Vec3 start = new Vec3(t.Translate - t.AxisX * size.x * 0.5 - t.AxisY * size.y * 0.5);

		for (int i = 0; i < volumeParts.Length; i++)
		{
			int y = i / volumeGridSize.x;
			int x = i % volumeGridSize.x;

			volumeParts[i].center = start + VolumePart.AxisX * (x * VolumePart.Width + VolumePart.HalfWidth)
				+ VolumePart.AxisY * (y * VolumePart.Depth + VolumePart.HalfDepth);
		}

		for (int i = 0; i < volumeParts.Length; i++)
		{
			float h = water.FetchHeight(new Vec3(volumeParts[i].AnchorPoint.x, volumeParts[i].AnchorPoint.y, 0.0f));
			if (volumeParts[i].AnchorPoint.z < h)
				volumeParts[i].waterHeight = MathLib.Clamp((float)(h - volumeParts[i].AnchorPoint.z), 0.0f, VolumePart.Height);
			else
				volumeParts[i].waterHeight = 0.0f;
		}

		if (UseVisualizer)
		{
			Visualizer.Enabled = true;
			Visualizer.Mode = Visualizer.MODE.ENABLED_DEPTH_TEST_DISABLED;

			for (int i = 0; i < volumeParts.Length; i++)
			{
				Visualizer.RenderPoint3D(volumeParts[i].AnchorPoint, 0.3f, vec4.WHITE);

				vec3 volume_size = new vec3(VolumePart.size.x, VolumePart.size.y, volumeParts[i].waterHeight);
				Mat4 volume_transform = new Mat4(VolumePart.rotation, volumeParts[i].AnchorPoint + VolumePart.AxisZ * volume_size.z * 0.5f);

				if (volumeParts[i].IsUnderWater)
				{
					Visualizer.RenderVector(volumeParts[i].AnchorPoint, volumeParts[i].AnchorPoint + VolumePart.AxisZ * volume_size.z, vec4.RED);
					Visualizer.RenderBox(new vec3(volume_size), volume_transform, vec4.BLUE);
				}
			}
		}
		else
		{
			Visualizer.Enabled = false;
		}
	}

	private void UpdatePhysics()
	{
		if (water == null || bodyRigid == null || volumeBoxShape == null)
			return;

		float water_volume = 0.0f;
		float all_volume = volumeBoxShape.Size.x * volumeBoxShape.Size.y * volumeBoxShape.Size.z;

		for (int i = 0; i < volumeParts.Length; i++)
		{
			if (volumeParts[i].IsUnderWater == false)
				continue;

			vec3 force = -Physics.Gravity * volumeParts[i].WaterVolume * waterDensity;
			force /= volumeParts.Length;
			bodyRigid.AddForce(force);

			vec3 radius = new vec3(volumeParts[i].AnchorPoint + VolumePart.AxisZ * volumeParts[i].waterHeight - bodyRigid.WorldCenterOfMass);
			vec3 torque = MathLib.Cross(radius, force);

			bodyRigid.AddTorque(torque);

			water_volume += volumeParts[i].WaterVolume;
		}

		float coeff = 0.0f;
		if (all_volume > 0.0f)
			coeff= water_volume / all_volume;

		bodyRigid.AddLinearImpulse(bodyRigid.Mass * (-bodyRigid.LinearVelocity * waterLinearDamping * coeff));
		bodyRigid.AddAngularImpulse((-bodyRigid.AngularVelocity * waterAngularDamping * coeff) * MathLib.Inverse(bodyRigid.IWorldInertia));
	}

	public void SetVisualizer(bool enabled)
	{
		UseVisualizer = enabled;
	}
}
