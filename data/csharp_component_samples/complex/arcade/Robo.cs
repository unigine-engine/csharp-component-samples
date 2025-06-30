using Unigine;

[Component(PropertyGuid = "7424e4b32ae8e2765203775d7040111821d76197")]
public class Robo : Component
{
	public float moveSpeed = 5.0f;
	public float turnSpeed = 90.0f;
	public int health = 50;

	[ParameterFile(Filter = ".node")]
	public string deathFx = "";

	private SampleDescriptionWindow sampleDescriptionWindow;

	private void Init()
	{
		var description = ComponentSystem.FindComponentInWorld<DescriptionWindowCreator>();
		if (description)
			sampleDescriptionWindow = description.getWindow();
	}

	private void Update()
	{
		if (Input.IsKeyPressed(Input.KEY.UP))
			Move(moveSpeed);

		if (Input.IsKeyPressed(Input.KEY.DOWN))
			Move(-moveSpeed);

		if (Input.IsKeyPressed(Input.KEY.LEFT))
			Turn(turnSpeed);

		if (Input.IsKeyPressed(Input.KEY.RIGHT))
			Turn(-turnSpeed);
	}

	private void Move(float speed)
	{
		node.WorldPosition += node.GetWorldDirection(MathLib.AXIS.Y) * speed * Game.IFps;
	}

	private void Turn(float speed)
	{
		node.Rotate(0, 0, speed * Game.IFps);
	}

	public void Hit(int damage)
	{
		health -= damage;
		var status = $"Health: {health}";
		if (sampleDescriptionWindow != null)
			sampleDescriptionWindow.setStatus(status);

		if (health <= 0)
			node.DeleteLater();
	}

	private void Shutdown()
	{
		if (deathFx != "")
		{
			Node deathFxNode = World.LoadNode(deathFx);
			deathFxNode.WorldPosition = node.WorldPosition;
		}
	}
}
