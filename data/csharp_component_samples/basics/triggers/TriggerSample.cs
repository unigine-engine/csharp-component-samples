using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "c59b7f924043770709e88a625e90acd2ad16ebc6")]
public class TriggerSample : Component
{
	[ShowInEditor]
	private Node targetToCheck = null;
	[ShowInEditor]
	private Node postamentPhysicsSphere = null;
	[ShowInEditor]
	private Node postamentPhysicsCapsule = null;
	[ShowInEditor]
	private Node postamentPhysicsCylinder = null;
	[ShowInEditor]
	private Node postamentPhysicsBox = null;
	[ShowInEditor]
	private Node postamentWorld = null;
	[ShowInEditor]
	private Node postamentMathSphere = null;
	[ShowInEditor]
	private Node postamentMathBox = null;
	[ShowInEditor]
	private Node postamentIntersectionSphere = null;
	[ShowInEditor]
	private Node postamentIntersectionBox = null;

	[ShowInEditor]
	PhysicalTrigger physicalTriggerSphere = null;
	[ShowInEditor]
	PhysicalTrigger physicalTriggerCapsule = null;
	[ShowInEditor]
	PhysicalTrigger physicalTriggerCylinder = null;
	[ShowInEditor]
	PhysicalTrigger physicalTriggerBox = null;
	[ShowInEditor]
	WorldTrigger worldTrigger = null;
	[ShowInEditor]
	private MathTriggerComponent mathTriggerSphere = null;
	[ShowInEditor]
	private MathTriggerComponent mathTriggerBox = null;
	[ShowInEditor]
	private IntersectionTriggerComponent intersectionTriggerSphere = null;
	[ShowInEditor]
	private IntersectionTriggerComponent intersectionTriggerBox = null;

	[ShowInEditor]
	private NodeTrigger nodeTrigger = null;
	[ShowInEditor]
	private Node triggerNodeParentNode = null;
	[ShowInEditor]
	private Node triggerNodeText = null;

	[ShowInEditor]
	private Material postamentMat = null;
	[ShowInEditor]
	private Material postamentMatTriggered = null;

	SampleDescriptionWindow sampleDescriptionWindow = new SampleDescriptionWindow();
	Visualizer.MODE visualizerMode;

	void Init()
	{
		Visualizer.Enabled = true;
		visualizerMode = Visualizer.Mode;
		Visualizer.Mode = Visualizer.MODE.ENABLED_DEPTH_TEST_ENABLED;

		// setting callbacks
		mathTriggerSphere.AddObject(targetToCheck);
		mathTriggerBox.AddObject(targetToCheck);

		physicalTriggerSphere.EventEnter.Connect(() =>
		{
			ObjectMeshStatic obj = postamentPhysicsSphere as ObjectMeshStatic;
			obj.SetMaterial(postamentMatTriggered, 0);
		});

		physicalTriggerSphere.EventLeave.Connect(() =>
		{
			ObjectMeshStatic obj = postamentPhysicsSphere as ObjectMeshStatic;
			obj.SetMaterial(postamentMat, 0);
		});

		physicalTriggerCapsule.EventEnter.Connect(() =>
		{
			ObjectMeshStatic obj = postamentPhysicsCapsule as ObjectMeshStatic;
			obj.SetMaterial(postamentMatTriggered, 0);
		});

		physicalTriggerCapsule.EventLeave.Connect(() =>
		{
			ObjectMeshStatic obj = postamentPhysicsCapsule as ObjectMeshStatic;
			obj.SetMaterial(postamentMat, 0);
		});

		physicalTriggerCylinder.EventEnter.Connect(() =>
		{
			ObjectMeshStatic obj = postamentPhysicsCylinder as ObjectMeshStatic;
			obj.SetMaterial(postamentMatTriggered, 0);
		});

		physicalTriggerCylinder.EventLeave.Connect(() =>
		{
			ObjectMeshStatic obj = postamentPhysicsCylinder as ObjectMeshStatic;
			obj.SetMaterial(postamentMat, 0);
		});

		physicalTriggerBox.EventEnter.Connect(() =>
		{
			ObjectMeshStatic obj = postamentPhysicsBox as ObjectMeshStatic;
			obj.SetMaterial(postamentMatTriggered, 0);
		});

		physicalTriggerBox.EventLeave.Connect(() =>
		{
			ObjectMeshStatic obj = postamentPhysicsBox as ObjectMeshStatic;
			obj.SetMaterial(postamentMat, 0);
		});

		worldTrigger.EventEnter.Connect(() =>
		{
			ObjectMeshStatic obj = postamentWorld as ObjectMeshStatic;
			obj.SetMaterial(postamentMatTriggered, 0);
		});

		worldTrigger.EventLeave.Connect(() =>
		{
			ObjectMeshStatic obj = postamentWorld as ObjectMeshStatic;
			obj.SetMaterial(postamentMat, 0);
		});

		mathTriggerSphere.EventEnter.Connect(() =>
		{
			ObjectMeshStatic obj = postamentMathSphere as ObjectMeshStatic;
			obj.SetMaterial(postamentMatTriggered, 0);
		});

		mathTriggerSphere.EventLeave.Connect(() =>
		{
			ObjectMeshStatic obj = postamentMathSphere as ObjectMeshStatic;
			obj.SetMaterial(postamentMat, 0);
		});

		mathTriggerBox.EventEnter.Connect(() =>
		{
			ObjectMeshStatic obj = postamentMathBox as ObjectMeshStatic;
			obj.SetMaterial(postamentMatTriggered, 0);
		});

		mathTriggerBox.EventLeave.Connect(() =>
		{
			ObjectMeshStatic obj = postamentMathBox as ObjectMeshStatic;
			obj.SetMaterial(postamentMat, 0);
		});

		intersectionTriggerSphere.EventEnter.Connect((Node node_trigger) =>
		{
			Unigine.Object obj = node_trigger as Unigine.Object;
			if (obj && (obj.GetIntersectionMask(0) == intersectionTriggerSphere.MaterialBallIntersectionMask))
			{
				ObjectMeshStatic postament = postamentIntersectionSphere as ObjectMeshStatic;
				postament.SetMaterial(postamentMatTriggered, 0);
			}
		});

		intersectionTriggerSphere.EventLeave.Connect((Node node_trigger) =>
		{
			Unigine.Object obj = node_trigger as Unigine.Object;
			if (obj && (obj.GetIntersectionMask(0) == intersectionTriggerSphere.MaterialBallIntersectionMask))
			{
				ObjectMeshStatic postament = postamentIntersectionSphere as ObjectMeshStatic;
				postament.SetMaterial(postamentMat, 0);
			}
		});

		intersectionTriggerBox.EventEnter.Connect((Node node_trigger) =>
		{
			Unigine.Object obj = node_trigger as Unigine.Object;
			if (obj && (obj.GetIntersectionMask(0) == intersectionTriggerSphere.MaterialBallIntersectionMask))
			{
				ObjectMeshStatic postament = postamentIntersectionBox as ObjectMeshStatic;
				postament.SetMaterial(postamentMatTriggered, 0);
			}
		});

		intersectionTriggerBox.EventLeave.Connect((Node node_trigger) =>
		{
			Unigine.Object obj = node_trigger as Unigine.Object;
			if (obj && (obj.GetIntersectionMask(0) == intersectionTriggerSphere.MaterialBallIntersectionMask))
			{
				ObjectMeshStatic postament = postamentIntersectionBox as ObjectMeshStatic;
				postament.SetMaterial(postamentMat, 0);
			}
		});

		nodeTrigger.EventEnabled.Connect((NodeTrigger trigger) =>
		{
			var  objectText = triggerNodeText as ObjectText;
			if (trigger.Enabled)
				objectText.TextColor = vec4.WHITE;
			else
				objectText.TextColor = vec4.RED;
		});

		nodeTrigger.EventPosition.Connect((NodeTrigger trigger) =>
		{
			Unigine.Object parent = trigger.Parent as Unigine.Object;
			Material material = parent.GetMaterialInherit(0);
			vec4 color = material.GetParameterFloat4("albedo_color");
			color.z += Game.IFps;
			if (color.z > 1.0f)
				color.z = 0.0f;
			material.SetParameterFloat4("albedo_color", color);
		});


		sampleDescriptionWindow.createWindow();
		WidgetGroupBox parameters = sampleDescriptionWindow.getParameterGroupBox();
		var nodeTriggerCheckbox = new WidgetCheckBox("Cube Active");
		parameters.AddChild(nodeTriggerCheckbox, Gui.ALIGN_LEFT);
		nodeTriggerCheckbox.EventChanged.Connect(() =>
		{
			triggerNodeParentNode.Enabled = nodeTriggerCheckbox.Checked;
		});
		nodeTriggerCheckbox.Checked = true;
	}
	
	void Update()
	{
		Visualizer.RenderBoundBox(worldTrigger.BoundBox, worldTrigger.WorldTransform, vec4.RED);

		physicalTriggerSphere.RenderVisualizer();
		physicalTriggerCapsule.RenderVisualizer();
		physicalTriggerCylinder.RenderVisualizer();
		physicalTriggerBox.RenderVisualizer();
	}

	void Shutdown()
	{
		Visualizer.Mode = visualizerMode;
		Visualizer.Enabled = false;

		sampleDescriptionWindow.shutdown();
	}
}
