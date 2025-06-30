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
using Unigine;

[Component(PropertyGuid = "6e8dd5fff1811413ba214248e2a30c62d6c35ef1")]
public class BodyFractureUnit : Component
{ 
	public enum BREAK_MODE
	{
		CRACK,
		SHATTER,
		SLICE
	};

	public bool Debug = true;

	public float MaxImpulse = 1.0f;
	public float Treshold = 1.0f;
	public Material Material = null;
	public BREAK_MODE Mode = BREAK_MODE.CRACK;

	[ParameterCondition(nameof(Mode), 0)]
	public int CrackCuts = 10;

	[ParameterCondition(nameof(Mode), 0)]
	public int CrackRings = 3;

	[ParameterCondition(nameof(Mode), 0)]
	public float CrackStep = 1.0f;

	[ParameterCondition(nameof(Mode), 1)]
	public int ShatterPieces = 10;

	private BodyFracture ownBody;

	public void Crack(Vec3 point, vec3 normal, float impulse)
	{
		if (impulse >= MaxImpulse)
			Crack(point, normal, ownBody);
	}

	void Init()
	{
		ownBody = node.ObjectBody as BodyFracture;
		if (!ownBody)
		{
			Log.WarningLine("BodyFractureUnit requires BodyFracture body");
			return;
		}

		ownBody.EventContactEnter.Connect((Body body, int num) => OnContact(body, num));
	}

	void Update()
	{
		if (!Debug || !ownBody)
			return;

		if (!ownBody.Broken)
		{
			Visualizer.RenderObject(ownBody.Object, vec4.WHITE);
		}
		else
		{
			RenderBroken(ownBody);
		}
	}

	private void RenderBroken(Body body)
	{
		Visualizer.RenderObject(body.Object, vec4.GREEN);

		for (int i = 0; i < body.NumChildren; ++i)
			RenderBroken(body.GetChild(i));
	}

	private void OnContact(Body body, int num)
	{
		if (body.GetContactImpulse(num) < MaxImpulse)
			return;

		var b0 = body.GetContactBody0(num);
		var b1 = body.GetContactBody1(num);
		while (b0 && b0.Type != Body.TYPE.BODY_FRACTURE)
			b0 = b0.Parent;
		while (b1 && b1.Type != Body.TYPE.BODY_FRACTURE)
			b1 = b1.Parent;

		BodyFracture fracture = b1 ? b1 as BodyFracture : b0 as BodyFracture;
		if (fracture == null)
			return;

		Crack(body.GetContactPoint(num), body.GetContactNormal(num), fracture);
	}

	private void Crack(Vec3 point, vec3 normal, BodyFracture fracture)
	{
		fracture.Threshold = Treshold;
		fracture.Material = Material;

		switch (Mode)
		{
			case BREAK_MODE.CRACK:
				fracture.CreateCrackPieces(point, normal, CrackCuts, CrackRings, CrackStep);
				break;
			case BREAK_MODE.SHATTER:
				fracture.CreateShatterPieces(ShatterPieces);
				break;
			case BREAK_MODE.SLICE:
				fracture.CreateSlicePieces(point, normal);
				break;
			default:
				break;
		}

		fracture.Broken = true;
	}
}
