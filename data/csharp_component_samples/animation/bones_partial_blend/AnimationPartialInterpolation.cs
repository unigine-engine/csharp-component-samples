using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "a94fe3f2ecf1f2a95de89bf4ea3616c3e74acd46")]
public class AnimationPartialInterpolation : Component
{
	public float firstAnimationSpeed = 30.0f;
	public float secondAnimationSpeed = 30.0f;

	[ParameterFile(Filter = ".anim")]
	public string firstAnimation = "";

	[ParameterFile(Filter = ".anim")]
	public string secondAnimation = "";

	public List<string> interpolatedBones = null;

	private ObjectMeshSkinned meshSkinned = null;
	private float currentTime = 0.0f;
	private List<int> bonesNumbers = null;
	private float weight = 0.5f;

	private enum LAYERS
	{
		FIRST_ANIMATION = 0,
		SECOND_ANIMATION,
		COUNT
	}

	private SampleDescriptionWindow sampleDescriptionWindow;

	private void Init()
	{
		sampleDescriptionWindow = new SampleDescriptionWindow();
		sampleDescriptionWindow.createWindow();

		meshSkinned = node as ObjectMeshSkinned;
		if (meshSkinned != null)
		{
			// set layers and animations
			meshSkinned.NumLayers = (int)LAYERS.COUNT;
			meshSkinned.SetLayerAnimationFilePath((int)LAYERS.FIRST_ANIMATION, firstAnimation);
			meshSkinned.SetLayerAnimationFilePath((int)LAYERS.SECOND_ANIMATION, secondAnimation);

			// find bones in mesh and save their numbers
			bonesNumbers = new List<int>();
			for (int i = 0; i < meshSkinned.NumBones; i++)
			{
				string name = meshSkinned.GetBoneName(i);
				if (interpolatedBones.Contains(name))
					bonesNumbers.Add(i);
			}
		}

		sampleDescriptionWindow.addFloatParameter("Weight:", "Weight", weight, 0.0f, 1.0f, (float value) =>
		{
			weight = value;
		});
	}

	private void Update()
	{
		if (meshSkinned == null)
			return;

		// set current frame for first and second animation
		meshSkinned.SetLayerFrame((int)LAYERS.FIRST_ANIMATION, currentTime * firstAnimationSpeed);
		meshSkinned.SetLayerFrame((int)LAYERS.SECOND_ANIMATION, currentTime * secondAnimationSpeed);

		currentTime += Game.IFps;

		// interpolate between layers
		meshSkinned.LerpLayer((int)LAYERS.SECOND_ANIMATION, (int)LAYERS.FIRST_ANIMATION, (int)LAYERS.SECOND_ANIMATION, weight);

		// set transform for interpolated bones
		if (bonesNumbers != null)
			foreach (int bone in bonesNumbers)
				meshSkinned.SetLayerBoneTransform((int)LAYERS.FIRST_ANIMATION, bone, meshSkinned.GetLayerBoneTransform((int)LAYERS.SECOND_ANIMATION, bone));
	}

	private void Shutdown()
	{
		sampleDescriptionWindow.shutdown();
	}

}
