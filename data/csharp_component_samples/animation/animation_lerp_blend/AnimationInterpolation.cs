using Unigine;

[Component(PropertyGuid = "4a532b80657d28df3fd9e0b23b684869dfd576b4")]
public class AnimationInterpolation : Component
{
	public float firstAnimationSpeed = 30.0f;
	public float secondAnimationSpeed = 30.0f;

	[ParameterFile(Filter = ".anim")]
	public string firstAnimation = "";

	[ParameterFile(Filter = ".anim")]
	public string secondAnimation = "";

	private ObjectMeshSkinned meshSkinned = null;
	private float currentTime = 0.0f;
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
			meshSkinned.NumLayers = (int)LAYERS.COUNT;
			meshSkinned.SetLayerAnimationFilePath((int)LAYERS.FIRST_ANIMATION, firstAnimation);
			meshSkinned.SetLayerAnimationFilePath((int)LAYERS.SECOND_ANIMATION, secondAnimation);
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
		meshSkinned.LerpLayer((int)LAYERS.FIRST_ANIMATION, (int)LAYERS.FIRST_ANIMATION, (int)LAYERS.SECOND_ANIMATION, weight);
	}

	private void Shutdown()
	{
		sampleDescriptionWindow.shutdown();
	}
}
