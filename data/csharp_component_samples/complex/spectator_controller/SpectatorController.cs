using System.Collections.Generic;
using System.Diagnostics;
using Unigine;


#if UNIGINE_DOUBLE
using Vec3 = Unigine.dvec3;
using Mat4 = Unigine.dmat4;
#else
using Vec3 = Unigine.vec3;
using Mat4 = Unigine.mat4;
#endif

[Component(PropertyGuid = "4feac9fb387f583654c7454fba0ef83faa3f39dd")]
public class SpectatorController : Component
{
	[ShowInEditor, Parameter(Group="Input", Title="Controlled", Tooltip="Toggle spectator inputs")] 
	public bool isControlled = true; 

	[ShowInEditor, Parameter(Group="Input", Title="Forward Key", Tooltip="Move forward key")] 
	public string forwardKey = "W";

	[ShowInEditor, Parameter(Group="Input", Title="Left Key", Tooltip="Move left key")] 
	public string leftKey = "A";

	[ShowInEditor, Parameter(Group="Input", Title="Backward Key", Tooltip="Move backward key")] 
	public string backwardKey = "S";

	[ShowInEditor, Parameter(Group="Input", Title="Right Key", Tooltip="Move right key")] 
	public string rightKey = "D";

	[ShowInEditor, Parameter(Group="Input", Title="Down key", Tooltip="Move down key")]  
	public string downKey = "Q";

	[ShowInEditor, Parameter(Group="Input", Title="Up key", Tooltip="Move up key")] 
	public string upKey = "E";

	[ShowInEditor, Parameter(Group="Input", Title="Turn up key", Tooltip="Turn up")] 
	public string turnUpKey = "UP";

	[ShowInEditor, Parameter(Group="Input", Title="Turn down key", Tooltip="Turn down")] 
	public string turnDownKey = "DOWN";

	[ShowInEditor, Parameter(Group="Input", Title="Turn left key", Tooltip="Turn left")] 
	public string turnLeftKey = "LEFT";	

	[ShowInEditor, Parameter(Group="Input", Title="Turn right key", Tooltip="Turn right")] 
	public string turnRightKey = "RIGHT";

	[ShowInEditor, Parameter(Group="Input", Title="Sprint Key", Tooltip="Sprint mode activation key")] 
	public string accelerateKey = "ANY_SHIFT";

	[ShowInEditor, Parameter(Group="Input", Title="Mouse Sensitivity", Tooltip="Mouse sensitivity multiplier")] 
	public float mouseSensitivity = 0.4f;


	[ShowInEditor, Parameter(Group="Movement", Title="Velocity", Tooltip="Movement velocity")]
	public float velocity = 2.0f;

	[ShowInEditor, Parameter(Group="Movement", Title="Sprint velocity", Tooltip="Sprint movement velocity")]
	public float sprintVelocity = 4.0f;

	[ShowInEditor, Parameter(Group="Movement", Title="Acceleration", Tooltip="Movement acceleration")]
	public float acceleration = 4.0f;

	[ShowInEditor, Parameter(Group="Movement", Title="Damping", Tooltip="Movement damping")]
	public float damping = 8.0f;


	[ShowInEditor, Parameter(Group="Collision", Title="Collision", Tooltip="Toggle spectator collision")]
	public bool isCollided = true;

	[ShowInEditor, ParameterMask(Group="Collision", MaskType = ParameterMaskAttribute.TYPE.COLLISION, Title="Collision Mask", Tooltip="A bit mask of collisions")]
	public int mask = 1;

	[ShowInEditor, Parameter(Group="Collision", Title="Collision radius", Tooltip="The radius of the collision sphere")]
	public float collisionRadius = 0.5f;


	[ShowInEditor, Parameter(Group="Rotation", Title="Turning speed", Tooltip="Velocity of the spectator's turning action")]
	public float turning = 90f;

	[ShowInEditor, Parameter(Group="Rotation", Title="Min theta angle", Tooltip="Minimun pitch angle")]
	public float minThetaAngle = -89.9f;

	[ShowInEditor, Parameter(Group="Rotation", Title="Max theta angle", Tooltip="Maximum pitch angle")]
	public float maxThetaAngle = 89.9f;

	private const float PLAYER_SPECTATOR_IFPS = 1.0f / 60.0f;
	private const int PLAYER_SPECTATOR_COLLISIONS = 4;
	
	private Vec3 _position = Vec3.ZERO;
	
	private Unigine.vec3 _direction = Unigine.vec3.ZERO;

	private float _phiAngle = 0;
	private float _thetaAngle = 0;

	private Vec3 _lastPosition = 0;
	private Unigine.vec3 _lastDirection = 0;
	private Unigine.vec3 _lastUp = 0;
	private Mat4 transform;

	private List<ShapeContact> _contacts = [];

	// These properties provide convenient access to the specific key bindings 
	// for movement and camera control in the application. 
	// Each property retrieves the corresponding Input.KEY value using a key name string.
	// This design allows key mappings to be configured via string identifiers, 
	// making the input system more flexible and customizable.
	private Input.KEY ForwardKey => Input.GetKeyByName(forwardKey);
	private Input.KEY BackwardKey => Input.GetKeyByName(backwardKey);
	private Input.KEY RightKey => Input.GetKeyByName(rightKey);
	private Input.KEY LeftKey => Input.GetKeyByName(leftKey);
	private Input.KEY UpKey => Input.GetKeyByName(upKey);
	private Input.KEY DownKey => Input.GetKeyByName(downKey);
	private Input.KEY TurnUpKey => Input.GetKeyByName(turnUpKey);
	private Input.KEY TurnDownKey => Input.GetKeyByName(turnDownKey);
	private Input.KEY TurnLeftKey => Input.GetKeyByName(turnLeftKey);
	private Input.KEY TurnRightKey => Input.GetKeyByName(turnRightKey);
	private Input.KEY AccelerateKey => Input.GetKeyByName(accelerateKey);
	
	private ShapeSphere _shapeSphere = null;

	private Player _player = null;

	// These methods are used to control the player's view direction 
	// externally from this class. They provide access to and allow manipulation of 
	// the camera's orientation
	public void SetPhiAngle(float phiAngle)
	{
		phiAngle -= _phiAngle;
		_direction = new quat(_player.Up, phiAngle) * _direction;
		_phiAngle += phiAngle;

		FlushTransform();
	}

	public void SetThetaAngle(float thetaAngle)
	{
		thetaAngle = MathLib.Clamp(thetaAngle, minThetaAngle, maxThetaAngle) - _thetaAngle;
		_direction = new quat(MathLib.Cross(_player.Up, _direction), thetaAngle) * _direction;
		_thetaAngle += thetaAngle;

		FlushTransform(); 
	}

	public void SetViewDirection(vec3 direction)
	{
		_direction = MathLib.Normalize(direction);

		vec3 tangent = vec3.ZERO;
		vec3 binormal = vec3.ZERO;

		MathLib.OrthoBasis(_player.Up,out tangent,out binormal);

		_phiAngle = MathLib.Atan2(MathLib.Dot(_direction, tangent), MathLib.Dot(_direction, binormal)) * MathLib.RAD2DEG;
		_thetaAngle = MathLib.Acos(MathLib.Clamp(MathLib.Dot(_direction, _player.Up), -1.0f, 1.0f)) * MathLib.RAD2DEG - 90.0f;
		_thetaAngle = MathLib.Clamp(_thetaAngle,minThetaAngle,maxThetaAngle);

		FlushTransform();
	}

	public float GetPhiAngle() =>_phiAngle;

	public float GetThetaAngle() => _thetaAngle;

	public Unigine.vec3 GetViewdirection() => _direction;


	// This block contains methods for querying detailed information about collision contacts.
	// These accessors provide data such as contact points, normals, depth of penetration, and 
	// the involved objects and surfaces. Useful for physics responses.
	public int GetNumContacts() => _contacts.Count;

	public ShapeContact GetContact(int num)
	{
		Debug.Assert(num >= 0 && num < GetNumContacts(), "SpectatorController.GetContact(): bad contact number");
		return _contacts[num];
	}

	public float GetContactDepth(int num)
	{
		Debug.Assert(num >= 0 && num < GetNumContacts(), "SpectatorController.GetContact(): bad contact number");
		return _contacts[num].Depth;
	}

	public Unigine.vec3 GetContactNormal(int num)
	{
		Debug.Assert(num >= 0 && num < GetNumContacts(), "SpectatorController.GetContact(): bad contact number");
		return _contacts[num].Normal;
	}

	public Unigine.Object GetContactObject(int num)
	{
		Debug.Assert(num >= 0 && num < GetNumContacts(), "SpectatorController.GetContact(): bad contact number");
		return _contacts[num].Object;
	}

	public Vec3 GetContactPoint(int num)
	{
		Debug.Assert(num >= 0 && num < GetNumContacts(), "SpectatorController.GetContact(): bad contact number");
		return _contacts[num].Point;
	}

	public Shape GetContactShape(int num)
	{
		Debug.Assert(num >= 0 && num < GetNumContacts(), "SpectatorController.GetContact(): bad contact number");
		return _contacts[num].Shape0;
	}

	public int GetContactSurface(int num)
	{
		Debug.Assert(num >= 0 && num < GetNumContacts(), "SpectatorController.GetContact(): bad contact number");
		return _contacts[num].Surface;
	}


	// UpdateControls handles user input processing and triggers movement updates.
	// This method reads control states (mouse and buttons) and passes 
	// the relevant data to UpdateMovement for velocity computation.
	public void UpdateControls()
	{
		Unigine.vec3 up = _player.Up;

		Unigine.vec3 impulse = Unigine.vec3.ZERO;

		Unigine.vec3 tangent = Unigine.vec3.ZERO;
		Unigine.vec3 binormal = Unigine.vec3.ZERO;
		MathLib.OrthoBasis(_player.Up, out tangent, out binormal);

		if (isControlled && !Unigine.Console.Active)
		{
			if (Input.MouseCursorHide)
			{
				_phiAngle += Input.MouseDeltaPosition.x * mouseSensitivity;
				_thetaAngle += Input.MouseDeltaPosition.y * mouseSensitivity;
			}

			_thetaAngle += turning * Game.IFps * (MathLib.ToInt(Input.IsKeyPressed(TurnDownKey)) - MathLib.ToInt(Input.IsKeyPressed(TurnUpKey)));
			_thetaAngle = MathLib.Clamp(_thetaAngle, minThetaAngle, maxThetaAngle);

			_phiAngle += turning * Game.IFps * (MathLib.ToInt(Input.IsKeyPressed(TurnRightKey)) - MathLib.ToInt(Input.IsKeyPressed(TurnLeftKey)));

			Unigine.vec3 x = (new quat (up, -_phiAngle) * new quat(tangent, -_thetaAngle)) * binormal;
			Unigine.vec3 y = MathLib.Normalize(MathLib.Cross(up, x));
			Unigine.vec3 z = MathLib.Normalize(MathLib.Cross(x,y));

			_direction = x;

			impulse += x * (MathLib.ToInt(Input.IsKeyPressed(ForwardKey)) - MathLib.ToInt(Input.IsKeyPressed(BackwardKey)));
			impulse += y * (MathLib.ToInt(Input.IsKeyPressed(LeftKey)) - MathLib.ToInt(Input.IsKeyPressed(RightKey)));
			impulse += z * (MathLib.ToInt(Input.IsKeyPressed(UpKey)) - MathLib.ToInt(Input.IsKeyPressed(DownKey)));
			
			if (impulse != vec3.ZERO)
				impulse.Normalize();

			impulse *= Input.IsKeyPressed(AccelerateKey) ? sprintVelocity : velocity;
		}
		
		float time = Game.IFps;

		float targetVelocity = MathLib.Length(impulse);

		Vec3 playerVelocity = _player.Velocity;
		
		// Use do-while to ensure at least one update is processed,
		// even when the remaining time is very small (e.g., at high FPS).
		do
		{
			// Clamp the simulation step to a maximum fixed time interval (PLAYER_SPECTATOR_IFPS).
    		// This prevents instability or large jumps in movement and collisions when frame time is high (e.g., during frame drops). 
			float ifps = MathLib.Min(time, PLAYER_SPECTATOR_IFPS); 
			time -= ifps;
			UpdateMovement(impulse, ifps, targetVelocity);
		} while (time > MathLib.EPSILON);

	}

	// Applies the final transformation matrix to the scene Node based on 
	// the updated movement and orientation values computed in the previous steps.
	public void FlushTransform()
	{
		Unigine.vec3 up = _player.Up;
		
		if (_lastPosition != _position || _lastDirection != _direction || _lastUp != up)
		{
			node.WorldTransform = MathLib.SetTo(_position,_position + (Vec3)_direction, up);
			OnTransformChanged(); // update all internal params

			_lastPosition = _position;
			_lastDirection = _direction;
			_lastUp = up;
		}

	}

	// Syncs this component's internal state with the transform of the node.
	private void OnTransformChanged()
	{	
		Unigine.vec3 up = _player.Up;

		Unigine.vec3 tangent = Unigine.vec3.ZERO; 
		Unigine.vec3 binormal = Unigine.vec3.ZERO;
		MathLib.OrthoBasis(up,out tangent, out binormal);

		_position = node.WorldTransform.GetColumn3(3);

		_direction = MathLib.Normalize(new Unigine.vec3(-node.WorldTransform.GetColumn3(2)));

		_phiAngle = MathLib.Atan2(MathLib.Dot(_direction, tangent), MathLib.Dot(_direction, binormal)) * MathLib.RAD2DEG;
		_thetaAngle = MathLib.Acos(MathLib.Clamp(MathLib.Dot(_direction, _player.Up), -1.0f, 1.0f)) * MathLib.RAD2DEG - 90.0f;
		_thetaAngle = MathLib.Clamp(_thetaAngle,minThetaAngle,maxThetaAngle);
		_direction = new quat(up, -_phiAngle) * new quat(tangent, -_thetaAngle) * binormal;
		
		_lastPosition = _position;
		_lastDirection = _direction;
		_lastUp = up;
	}

	// Calculates the player's current velocity and adjusts camera position 
	// if collisions are detected. Called after input is processed in UpdateControls.
	private void UpdateMovement(Unigine.vec3 impulse, float ifps, float targetVelocity)
	{
		float oldVelocity = MathLib.Length(_player.Velocity);

		_player.Velocity += impulse * acceleration * ifps;

		float currentVelocity = MathLib.Length(_player.Velocity);
		if (targetVelocity < MathLib.EPSILON || currentVelocity > targetVelocity)
			_player.Velocity *= MathLib.Exp(-damping * ifps);
		
		currentVelocity = MathLib.Length(_player.Velocity);
		if(currentVelocity > oldVelocity && currentVelocity > targetVelocity)
			_player.Velocity *= targetVelocity / currentVelocity;
		
		if(currentVelocity < MathLib.EPSILON)
			_player.Velocity = Unigine.vec3.ZERO;

		_position += _player.Velocity * ifps;

		_contacts.Clear();

		if (_player.Enabled && isCollided)
		{
			for (int i = 0; i < PLAYER_SPECTATOR_COLLISIONS; i++)
			{
				_shapeSphere.Center = _position;
				_shapeSphere.GetCollision(_contacts, ifps); 
				if (_contacts.Count == 0)
					break;
				
				// Calculate the inverse of the number of contacts to evenly distribute the total push-out
				// This prevents applying the full depth for every contact, which would overcompensate the position
				float inversContacts = 1.0f / MathLib.ToFloat(_contacts.Count); 
				for (int j = 0; j < _contacts.Count; j++)
				{
					ShapeContact contact = _contacts[j];

					// Push the player out along the contact normal, scaled by penetration depth and evenly divided by contact count
					_position += new Vec3(contact.Normal * (contact.Depth * inversContacts));

					// Remove the velocity component that's directed into the contact surface
    				// This prevents the player from continuing to move into the object
					_player.Velocity -= contact.Normal * MathLib.Dot(_player.Velocity, contact.Normal);  
				}
			}
		}

		_shapeSphere.Center = _position;
	}

	private void Init()
	{
		_shapeSphere = new ShapeSphere(collisionRadius);
		_shapeSphere.Continuous = false;

		_player = node as Player;
		
		OnTransformChanged();
	}
	
	private void Update()
	{
		if (!transform.Equals(node.Transform)) // if somebody change our position outside -> update all internal params.
			OnTransformChanged();
			
		UpdateControls();
		FlushTransform();

		transform = node.Transform;
	}

	private void Shutdown()
	{
		_shapeSphere.DeleteLater();
	}
}
