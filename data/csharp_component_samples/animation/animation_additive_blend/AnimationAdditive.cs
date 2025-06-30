using System;
using Unigine;
using static Unigine.VREyeTracking;

[Component(PropertyGuid = "d61485a9563c12f466dd21c3dd285c468f1586ed")]
public class AnimationAdditive : Component
{
	public float firstAnimationSpeed = 30.0f;
	public float secondAnimationSpeed = 30.0f;

	[ParameterFile(Filter = ".anim")]
	public string firstAnimation = "";

	[ParameterFile(Filter = ".anim")]
	public string secondAnimation = "";

	public int ReferncesFramesCount => meshSkinned.GetLayerNumFrames((int)LAYERS.SECOND_ANIMATION);

	private ObjectMeshSkinned meshSkinned = null;
	private float currentTime = 0.0f;
	private float animationRefereceFrame = 0.0f;
	private float weight = 0.5f;

	private enum LAYERS
	{
		FIRST_ANIMATION = 0,
		SECOND_ANIMATION,
		AUXILIARY,
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
			meshSkinned.NumLayers = (int)LAYERS.COUNT;
			meshSkinned.SetLayerAnimationFilePath((int)LAYERS.FIRST_ANIMATION, firstAnimation);
			meshSkinned.SetLayerAnimationFilePath((int)LAYERS.SECOND_ANIMATION, secondAnimation);

			// inverse reference frame into the auxiliary layer
			meshSkinned.SetLayerFrame((int)LAYERS.SECOND_ANIMATION, animationRefereceFrame);
			meshSkinned.InverseLayer((int)LAYERS.AUXILIARY, (int)LAYERS.SECOND_ANIMATION);
		}

		sampleDescriptionWindow.addFloatParameter("Weight:", "Weight", weight, 0.0f, 4.0f, (float value) =>
		{
			weight = value;
		});
		sampleDescriptionWindow.addFloatParameter("Reference Frame:", "ReferenceFrame", animationRefereceFrame, 0.0f, ReferncesFramesCount,
			(float value) =>
			{
				animationRefereceFrame = value;

				meshSkinned.SetLayerFrame((int)LAYERS.SECOND_ANIMATION, animationRefereceFrame);
				meshSkinned.InverseLayer((int)LAYERS.AUXILIARY, (int)LAYERS.SECOND_ANIMATION);
			}
		);
	}

	private void Update()
	{
		// set frames
		meshSkinned.SetLayerFrame((int)LAYERS.FIRST_ANIMATION, currentTime * firstAnimationSpeed);
		meshSkinned.SetLayerFrame((int)LAYERS.SECOND_ANIMATION, currentTime * secondAnimationSpeed);

		currentTime += Game.IFps;

		// multiple second layer by inverse reference frame
		meshSkinned.MulLayer((int)LAYERS.SECOND_ANIMATION, (int)LAYERS.AUXILIARY, (int)LAYERS.SECOND_ANIMATION);

		// combine two animations
		meshSkinned.MulLayer((int)LAYERS.FIRST_ANIMATION, (int)LAYERS.FIRST_ANIMATION, (int)LAYERS.SECOND_ANIMATION, weight);
	}

	private void Shutdown()
	{
		sampleDescriptionWindow.shutdown();
	}

}
