using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unigine;
using System;


#if UNIGINE_DOUBLE
using Vec3 = Unigine.dvec3;
#else
using Vec3 = Unigine.vec3;
#endif


[Component(PropertyGuid = "8fc3ca2973f720aa9d76649e232482266fdbdeea")]
public class ProceduralMeshModifier : Component
{
	private int size = 128;
	private float isize;

	private ObjectMeshStatic objectMesh;
	private Mesh meshRAM;
	private MeshRender meshVRAM;

	// parameter to coordinate different threads
	private bool isDeleted;

	private ObjectMeshStatic.PROCEDURAL_MODE currentMode;
	private int currentMeshRenderFlag;

	// enables creating CollisionData for mesh
	private bool isCollisionEnabled = false;
	// enables mesh creation in async thread
	private bool isThreadAsync = true;
	// enables apply*MeshProceduralAsync instead of apply*MeshProceduralForce
	private bool isAsyncMode = true;
	// enables applyCopyMeshProcedural* instead of applyMoveMeshProcedural*
	private bool isCopyMode = true;
	// enables creating MeshRender yourself and not by engine inside applyMeshProcedural methods
	private bool isMeshvramManual = false;
	// reserve meshvram manual state, so it won't be changed mid mesh update in different thread
	private bool updatedMeshvramManual;

	// check if we already in the process of modifying a mesh
	private bool isRunning = false;

	// UI
	private SampleDescriptionWindow sampleDescriptionWindow;

	private WidgetComboBox threadCombo;
	private WidgetComboBox asyncCombo;
	private WidgetComboBox moveCombo;

	private Dictionary<string, ObjectMeshStatic.PROCEDURAL_MODE> modesMap;
	private Dictionary<string, int> usageMap;
	private WidgetComboBox modeCombo;
	private WidgetComboBox usageCombo;

	private WidgetCheckBox meshvramCheckbox;

	private WidgetLabel warningLabel;

	void Init()
	{
		InitGui();

		isDeleted = false;
		updatedMeshvramManual = isMeshvramManual;

		isize = 30.0f / size;

		meshRAM = new Mesh();
		meshVRAM = new MeshRender();
		objectMesh = new ObjectMeshStatic();
		objectMesh.SetMeshProceduralMode(currentMode);
		objectMesh.WorldPosition = Vec3.ONE;
	}
	
	void Update()
	{
		int id = Profiler.BeginMicro("Component Update");

		// check if mesh modification or applying is already in active state
		if (isRunning || objectMesh.IsMeshProceduralActive)
		{
			Profiler.EndMicro(id);
			return;
		}

		isRunning = true;

		isMeshvramManual = updatedMeshvramManual;

		// set new procedural mode if needed
		if (objectMesh.MeshProceduralMode != currentMode)
		{
			objectMesh.DeleteLater();
			objectMesh = new ObjectMeshStatic();
			objectMesh.SetMeshProceduralMode(currentMode);
			objectMesh.WorldPosition = Vec3.ONE;
		}

		// check if mesh must be generated on Background or Main thread
		if (isThreadAsync)
		{
			// updates mesh on Background thread without blocking Main thread
			// mesh modification will be executed on different thread for as long as it's needed without
			// affecting perfomance
			AsyncQueue.RunAsync(AsyncQueue.ASYNC_THREAD.BACKGROUND, AsyncUpdateRAM);
		}
		else
		{
			// updates mesh in current frame before update ends
			UpdateRAM();
			if (isMeshvramManual)
				UpdateVRAM();
			ApplyData();
		}

		Profiler.EndMicro(id);
	}

	void Shutdown()
	{
		// signal for other threads that Shutdown() was called on Main thread
		isDeleted = true;

		ShutdownGui();

		// if there is some active mesh modification in progress, wait till it's finished before
		// clearing meshes
		lock (meshRAM)
		{
			meshRAM.Clear();
			meshVRAM.Clear();
			objectMesh.DeleteLater();
		}
	}

	// updates geometry
	private void UpdateMesh(Mesh mesh)
	{
		int id = Profiler.BeginMicro("ProceduralMeshModifier.UpdateMesh");

		float time = Game.Time;

		if (mesh.NumSurfaces != 1)
		{
			mesh.Clear();
			mesh.AddSurface("");
		}
		else
		{
			mesh.ClearSurface();
		}

		var vertices = new vec3[size * size];

		for (int y = 0; y < size; y++)
		{
			float Y = y * isize - 15.0f;
			float Z = MathLib.Cos(Y + time);

			for (int x = 0; x < size; x++)
			{
				float X = x * isize - 15.0f;
				vertices[y * size + x] = (new vec3(X, Y, Z * MathLib.Sin(X + time)));
			}
		}

		mesh.AddVertex(vertices);

		// reserve enough memory for indices so vector won't be reallocated every time it's capacity
		// ends
		var cindices = new List<int>();
		cindices.Capacity = (size - 1) * (size - 1) * 6;
		var tindices = new List<int>();
		tindices.Capacity = (size - 1) * (size - 1) * 6;

		var addIndex = (int index) =>
		{
			cindices.Add(index);
			tindices.Add(index);
		};

		for (int y = 0; y < size - 1; y++)
		{
			int offset = size * y;
			for (int x = 0; x < size - 1; x++)
			{
				addIndex(offset);
				addIndex(offset + 1);
				addIndex(offset + size);
				addIndex(offset + size);
				addIndex(offset + 1);
				addIndex(offset + size + 1);
				offset++;
			}
		}

		mesh.AddCIndices(cindices.ToArray());
		mesh.AddTIndices(tindices.ToArray());

		cindices.Clear();
		tindices.Clear();

		mesh.CreateTangents();

		{
			int idScope = Profiler.BeginMicro("CreateCollisionData");

			// if you plan to use intersection and collision with this mesh then it's better to create
			// CollisionData. Otherwise intersections and collisions would be highly ineffective in
			// terms of perfomance
			if (isCollisionEnabled)
			{
				// creates both Spatial Tree and Edges for effective intersection and collision
				// respectively
				mesh.CreateCollisionData();

				// you can create only Spatial Tree if you need only intersection
				//		mesh.CreateSpatialTree();
				// or you can create only Edges if you need only collision
				//		mesh.CreateEdges();
			}
			else
				// or you can create mesh without collision data or even delete existing one if there is
				// no need for it.
				// to check if mesh has collisionData you can use:
				//		mesh.HasCollisionData();
				//		mesh.HasSpatialTree();
				//		mesh.HasEdges();
				mesh.ClearCollisionData();

			Profiler.EndMicro(idScope);
		}

		mesh.CreateBounds();

		Profiler.EndMicro(id);
	}

	// updates Mesh
	private void UpdateRAM()
	{
		int id = Profiler.BeginMicro("ProceduralMeshModifier.UpdateRAM");

		// check that Main thread is stil active
		if (isDeleted)
		{
			Profiler.EndMicro(id);
			return;
		}

		// lock mesh_ram so other threads won't interfere mesh update
		lock (meshRAM)
		{
			UpdateMesh(meshRAM);
		}
		Profiler.EndMicro(id);
	}

	private void AsyncUpdateRAM()
	{
		// modify mesh
		UpdateRAM();

		if (isMeshvramManual)
		{
			// if you need to load MeshRender manualy, do it on GPU_STREAM thread
			AsyncQueue.RunAsync(AsyncQueue.ASYNC_THREAD.GPU_STREAM, AsyncUpdateVRAM);
		}
		else
		{
			// if you don't need to load MeshRender manualy and automatic loading inside
			// apllyMeshProcedural methods is enough then return to Main thread
			AsyncQueue.RunAsync(AsyncQueue.ASYNC_THREAD.MAIN, () =>
			{
				// check that sample's logic on Main thread is still alive. If it's not then stop
				// modification and return
				if (!node || node.IsDeleted)
					return;
				ApplyData();
			});
		}
	}

	// updates MeshRender
	private void UpdateVRAM()
	{
		int id = Profiler.BeginMicro("ProceduralMeshModifier.UpdateVRAM");

		// check that Main thread is stil active
		if (isDeleted)
		{
			Profiler.EndMicro(id);
			return;
		}

		// lock so other threads won't interfere mesh update
		lock (meshRAM)
		{
			meshVRAM.Load(meshRAM);
		}
		Profiler.EndMicro(id);
	}

	private void AsyncUpdateVRAM()
	{
		// update MeshRender
		UpdateVRAM();

		// return to Main thread to apply new mesh
		AsyncQueue.RunAsync(AsyncQueue.ASYNC_THREAD.MAIN, () => {
			// check that sample's logic on Main thread is still alive. If it's not then stop
			// modification and return
			if (!node || node.IsDeleted)
				return;
			ApplyData();
		});
	}

	// applies updated procedural mesh only on Main thread!
	private void ApplyData()
	{
		int id = Profiler.BeginMicro("ProceduralMeshModifier.ApplyData");

		// if apply happens in async mode it will be processed on another thread without blocking Main
		// thread. Otherwise in Force mode Main thread won't leave the scope of this function until
		// apply is finished.

		if (isAsyncMode)
		{
			if (isMeshvramManual)
			{
				// you can use manualy created MeshRender only in Move mode
				objectMesh.ApplyMoveMeshProceduralAsync(meshRAM, meshVRAM);
			}
			else
			{
				if (isCopyMode)
					// with Copy mode data from mesh_ram will be copied for internal use and mesh_ram
					// itself won't be changed
					objectMesh.ApplyCopyMeshProceduralAsync(meshRAM, currentMeshRenderFlag);
			else
					// with Move mode data will be taken from mesh_ram for internal use so mesh_ram
					// will change
					objectMesh.ApplyMoveMeshProceduralAsync(meshRAM, currentMeshRenderFlag);
			}
		}
		else
		{
			if (isMeshvramManual)
			{
				objectMesh.ApplyMoveMeshProceduralForce(meshRAM, meshVRAM);
			}
			else
			{
				if (isCopyMode)
					objectMesh.ApplyCopyMeshProceduralForce(meshRAM, currentMeshRenderFlag);
			else
					objectMesh.ApplyMoveMeshProceduralForce(meshRAM, currentMeshRenderFlag);
			}
		}

		// full cycle of mesh modification is finished
		isRunning = false;

		Profiler.EndMicro(id);
	}

	// UI methods
	private void InitGui()
	{
		sampleDescriptionWindow = new SampleDescriptionWindow();
		sampleDescriptionWindow.createWindow(Gui.ALIGN_RIGHT);
		var params_box = sampleDescriptionWindow.getParameterGroupBox();

		var gridbox = new WidgetGridBox(2, 10);
		params_box.AddChild(gridbox, Gui.ALIGN_EXPAND);

		//	--------Async/Force Mode Selector--------
		var label = new WidgetLabel("Thread");
		gridbox.AddChild(label, Gui.ALIGN_LEFT);

		threadCombo = new WidgetComboBox();
		threadCombo.AddItem("Main");
		threadCombo.AddItem("Background");
		gridbox.AddChild(threadCombo, Gui.ALIGN_EXPAND);

		threadCombo.CurrentItem = 1;
		threadCombo.EventChanged.Connect(() =>
		{
			isThreadAsync = threadCombo.CurrentItem != 0;
		});

		//	--------Procedural Mode Selector--------
		modesMap = new Dictionary<string, ObjectMeshStatic.PROCEDURAL_MODE>();
		modesMap["Dynamic"] = ObjectMeshStatic.PROCEDURAL_MODE.DYNAMIC;
		modesMap["File"] = ObjectMeshStatic.PROCEDURAL_MODE.FILE;
		modesMap["Blob"] = ObjectMeshStatic.PROCEDURAL_MODE.BLOB;

		label = new WidgetLabel("Procedural Mode");
		gridbox.AddChild(label, Gui.ALIGN_LEFT);

		modeCombo = new WidgetComboBox();
		modeCombo.AddItem("Dynamic");
		modeCombo.AddItem("File");
		modeCombo.AddItem("Blob");
		gridbox.AddChild(modeCombo, Gui.ALIGN_EXPAND);

		currentMode = ObjectMeshStatic.PROCEDURAL_MODE.DYNAMIC;
		modeCombo.CurrentItem = 0;
		modeCombo.EventChanged.Connect(() =>
		{
			var item = modeCombo.GetCurrentItemText();
			currentMode = modesMap[item];
		});

		//	--------MeshRender Flag Selector--------
		usageMap = new Dictionary<string, int>();
		usageMap["None"] = 0;
		usageMap["DYNAMIC_VERTEX"] = MeshRender.USAGE_DYNAMIC_VERTEX;
		usageMap["DYNAMIC_INDICES"] = MeshRender.USAGE_DYNAMIC_INDICES;
		usageMap["DYNAMIC_ALL"] = MeshRender.USAGE_DYNAMIC_ALL;

		label = new WidgetLabel("MeshRender Flag");
		gridbox.AddChild(label, Gui.ALIGN_LEFT);

		usageCombo = new WidgetComboBox();
		usageCombo.AddItem("None");
		usageCombo.AddItem("DYNAMIC_VERTEX");
		usageCombo.AddItem("DYNAMIC_INDICES");
		usageCombo.AddItem("DYNAMIC_ALL");
		gridbox.AddChild(usageCombo, Gui.ALIGN_EXPAND);

		currentMeshRenderFlag = 0;
		usageCombo.CurrentItem = 0;
		usageCombo.EventChanged.Connect(() =>
		{
			var item = usageCombo.GetCurrentItemText();
			currentMeshRenderFlag = usageMap[item];
		});

		//	--------Async/Force Mode Selector--------
		label = new WidgetLabel("Async Mode");
		gridbox.AddChild(label, Gui.ALIGN_LEFT);

		asyncCombo = new WidgetComboBox();
		asyncCombo.AddItem("Async");
		asyncCombo.AddItem("Force");
		gridbox.AddChild(asyncCombo, Gui.ALIGN_EXPAND);

		asyncCombo.CurrentItem = 0;
		asyncCombo.EventChanged.Connect(() =>
		{
			isAsyncMode = asyncCombo.CurrentItem == 0;
		});

		//	--------MeshRender Dynamic Usage Selector--------
		label = new WidgetLabel("Apply mode");
		gridbox.AddChild(label, Gui.ALIGN_LEFT);

		moveCombo = new WidgetComboBox();
		moveCombo.AddItem("Copy");
		moveCombo.AddItem("Move");
		gridbox.AddChild(moveCombo, Gui.ALIGN_EXPAND);

		isCopyMode = true;
		moveCombo.CurrentItem = 0;
		moveCombo.EventChanged.Connect(() =>
		{
			isCopyMode = moveCombo.CurrentItem == 0;
		});

		//	--------Create Collision Data--------
		label = new WidgetLabel("Create CollisionData");
		gridbox.AddChild(label, Gui.ALIGN_LEFT);

		var collison_checkbox = new WidgetCheckBox();
		gridbox.AddChild(collison_checkbox, Gui.ALIGN_EXPAND);
		collison_checkbox.EventChanged.Connect(() => { isCollisionEnabled = collison_checkbox.Checked; });

		//	--------Create MeshRender Manualy--------
		label = new WidgetLabel("Manual MeshRender");
		gridbox.AddChild(label, Gui.ALIGN_LEFT);

		meshvramCheckbox = new WidgetCheckBox();
		gridbox.AddChild(meshvramCheckbox, Gui.ALIGN_EXPAND);
		meshvramCheckbox.EventChanged.Connect(() =>
		{
			warningLabel.Hidden = !meshvramCheckbox.Checked;
		});

		//	--------Create MeshRender Warning--------
		warningLabel = new WidgetLabel("MeshRender can be used only with mode \"Move\". Apply mode \"Copy\" will be ignored ")
		{
			FontWrap = 1,
			Hidden = true,
			FontColor = vec4.RED
		};

		params_box.FontWrap = 1;
		params_box.AddChild(warningLabel, Gui.ALIGN_EXPAND);
	}

	private void ShutdownGui()
	{
		sampleDescriptionWindow.shutdown();
	}
}
