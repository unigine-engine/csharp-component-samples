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

[Component(PropertyGuid = "fc0ff54b594d4a2ecf680bb55e0ea99069ccefaf")]
public class PrimitivesSpawner : Component
{
	private void Init()
	{
		CreateBox();
		CreateSphere();
		CreateCylinder();
		CreateCapsule();
		CreatePrism();
		CreatePlane();
	}

	private void CreateBox()
	{
		// first, we're creating a mesh instance as it is required to call an ObjectMeshDynamic constructor
		Mesh boxMesh = new Mesh();

		// creating a box surface with a given size to the mesh
		boxMesh.AddBoxSurface("box_surface", new vec3(1.0f, 1.0f, 1.0f));

		//create an ObjectMeshDynamic node and set position
		ObjectMeshDynamic box = new ObjectMeshDynamic(boxMesh);
		box.WorldPosition = new Vec3(-5.0f, 0.0f, 1.5f);

		// clearing the mesh
		boxMesh.Clear();
	}

	private void CreateSphere()
	{
		Mesh sphereMesh = new Mesh();
		sphereMesh.AddSphereSurface("sphere_surface", 0.5f, 16, 16);

		ObjectMeshDynamic sphere = new ObjectMeshDynamic(sphereMesh);
		sphere.WorldPosition = new Vec3(-3.0f, 0.0f, 1.5f);

		sphereMesh.Clear();
	}

	private void CreateCylinder()
	{
		Mesh cylinderMesh = new Mesh();
		cylinderMesh.AddCylinderSurface("cylinder_surface", 0.5f, 1.0f, 16, 16);

		ObjectMeshDynamic cylinder = new ObjectMeshDynamic(cylinderMesh);
		cylinder.WorldPosition = new Vec3(-1.0f, 0.0f, 1.5f);

		cylinderMesh.Clear();
	}

	private void CreateCapsule()
	{
		Mesh capsuleMesh = new Mesh();
		capsuleMesh.AddCapsuleSurface("capsule_surface", 0.5f, 1.0f, 16, 16);

		ObjectMeshDynamic capsule = new ObjectMeshDynamic(capsuleMesh);
		capsule.WorldPosition = new Vec3(1.0f, 0.0f, 1.5f);

		capsuleMesh.Clear();
	}

	private void CreatePrism()
	{
		Mesh prismMesh = new Mesh();
		prismMesh.AddPrismSurface("prism_surface", 0.5f, 1.0f, 0.5f, 5);

		ObjectMeshDynamic prism = new ObjectMeshDynamic(prismMesh);
		prism.WorldPosition = new Vec3(3.0f, 0.0f, 1.5f);

		prismMesh.Clear();
	}

	private void CreatePlane()
	{
		Mesh planeMesh = new Mesh();
		planeMesh.AddPlaneSurface("plane_surface", 1.0f, 1.0f, 1);

		ObjectMeshDynamic plane = new ObjectMeshDynamic(planeMesh);
		plane.WorldPosition = new Vec3(5.0f, 0.0f, 1.5f);

		planeMesh.Clear();
	}

}
