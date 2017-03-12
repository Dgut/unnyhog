using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[ RequireComponent( typeof( CharacterController ) ) ]
public class PlayerController : MonoBehaviour
{
	public float movementSpeed = 5f;

	public bool owner = true;
	public Vector2 axis;
	public Vector3 target;

	private CharacterController characterController;
	public Transform look;
	public Animator animator;

	private Vector3 velocity;

	private Network network;

	void Start()
	{
		characterController = GetComponent< CharacterController >();
		network = FindObjectOfType< Network >();
	}

	void Update()
	{
		Vector3 direction = Vector3.zero;

		if( owner )
		{
			direction.x = Input.GetAxis( "Horizontal" );
			direction.z = Input.GetAxis( "Vertical" );
		}
		else
		{
			direction.x = axis.x;
			direction.z = axis.y;

			transform.position = Vector3.Lerp( transform.position, target, 0.1f );
		}

		Vector3 motion = direction * movementSpeed * Time.deltaTime;

		characterController.Move( motion );

		bool moves = direction.sqrMagnitude > 0f;

		animator.SetBool( "run", moves );

		if( moves )
			look.rotation = Quaternion.LookRotation( direction );
	}

	void FixedUpdate()
	{
		if( owner )
		{
			Vector2 position;
			Vector2 axis;
			position.x = transform.position.x;
			position.y = transform.position.z;
			axis.x = Input.GetAxis( "Horizontal" );
			axis.y = Input.GetAxis( "Vertical" );
			network.SendPlayerPosition( position, axis );
		}
	}
}
