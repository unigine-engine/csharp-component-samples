using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unigine;

#if UNIGINE_DOUBLE
using Vec3 = Unigine.dvec3;
using Vec4 = Unigine.dvec4;
using Mat4 = Unigine.dmat4;
#else
using Vec3 = Unigine.vec3;
using Vec4 = Unigine.vec4;
using Mat4 = Unigine.mat4;
#endif

[Component(PropertyGuid = "f57beaf90b900209989d36d8cad84699660d88e5")]
public class ProceduralMeshApply : Component
{
	private Mesh mesh;
	private ObjectMeshCluster cluster;

	private float radius = 0.5f;

	private const int maxNumStacks = 30;
	private const int minNumStacks = 2;

	private int numStacks = 2;
	private int numSlices = 3;

	// signals if we increase or decrease number of slices and stacks
	private bool isIncreasing = true;

	// timer to change sphere parameters
	private float changeRate = 0.1f;
	private float currentTime = 0.0f;

	// x/y-size of cluster field
	private const int size = 20;
	// offset between meshes
	private float offset = 1.0f;

	void Init()
	{
		mesh = new Mesh();
		cluster = new ObjectMeshCluster();

		// before changing mesh choose Procedural Mode:
		// - Disable - procedural mode is disabled
		// - Dynamic - fastest performance, stored in RAM and VRAM, not automatically unloaded from
		// memory.
		// - Blob - moderate performance, stored in RAM and VRAM, automatically unloaded from memory.
		// - File - slowest performance, all data stored on disk, automatically unloaded from memory.
		cluster.SetMeshProceduralMode(ObjectMeshStatic.PROCEDURAL_MODE.DYNAMIC);
		cluster.WorldPosition = new Vec3(0.0f, 0.0f, 3.0f);

		// create cluster transforms
		List<Mat4> transforms = new List<Mat4>();
		float fieldOffset = (1.0f + offset) * size / 2.0f;

		for (int y = 0; y < size; y++)
		{
			for (int x = 0; x < size; x++)
			{
				transforms.Add(MathLib.Translate(new Vec3(x + x * offset - fieldOffset, y + y * offset - fieldOffset, 1.5f)));
			}
		}

		cluster.AppendMeshes(transforms.ToArray());

		Visualizer.Enabled = true;
	}
	
	void Update()
	{
		// change mesh before applying
		UpdateMesh(mesh);

		// Apply new mesh. You can do it Force or Async.
		// Changing mesh_render_flag you can choose where to store MeshRender data: in RAM or VRAM
		// 0 - store everything in VRAM (default behavior)
		// USAGE_DYNAMIC_VERTEX - store vertices on RAM
		// USAGE_DYNAMIC_INDICES - store indices on RAM
		// USAGE_DYNAMIC_ALL - store both vertices and indices on RAM
		cluster.ApplyMoveMeshProceduralAsync(mesh, 0);
		Visualizer.RenderObject(cluster, vec4.GREEN);
	}

	void Shitdown()
	{
		mesh.Clear();
		cluster.DeleteLater();

		Visualizer.Enabled = false;
	}

	private void UpdateMesh(Mesh mesh)
	{
		currentTime += Game.IFps;

		if (currentTime > changeRate)
		{
			currentTime = 0.0f;

			numSlices = isIncreasing ? numSlices + 1 : numSlices - 1;
			numStacks = isIncreasing ? numStacks + 1 : numStacks - 1;

			if (numStacks == maxNumStacks)
				isIncreasing = false;

			if (numStacks <= minNumStacks)
			{
				isIncreasing = true;
				numStacks = minNumStacks;
				numSlices = numStacks + 1;
			}
		}

		mesh.Clear();
		mesh.AddSphereSurface("sphere", radius, numStacks, numSlices);
	}
}
