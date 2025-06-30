using Unigine;

[Component(PropertyGuid = "c61fa7c14aba5e2c7bb7439b23d0243e867f7e85")]
public class SimpleAnimationPlayer : Component
{
	public float animationSpeed = 30.0f;

	private ObjectMeshSkinned meshSkinned = null;
	private float currentTime = 0.0f;

	private void Init()
	{
		meshSkinned = node as ObjectMeshSkinned;
	}

	private void Update()
	{
		if (meshSkinned != null)
		{
			meshSkinned.SetLayerFrame(0, currentTime * animationSpeed);
			currentTime += Game.IFps;
		}
	}
}
