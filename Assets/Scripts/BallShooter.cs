using UnityEngine;
using System.Collections;

public class BallShooter : MonoBehaviour
{
	public bool owner = true;
	
	public Transform spawn;
	public Transform ballPrefab;

	public Animator animator;
	public AudioSource audioSource;

	public float delay = 0.5f;

	private Network network;

	void Start()
	{
		network = FindObjectOfType< Network >();
	}

	void Update()
	{
		if( owner && Input.GetButtonDown( "Fire1" ) )
		{
			Shoot();
			network.SendShot();
		}
	}

	public void Shoot()
	{
		animator.SetTrigger( "fire" );
		audioSource.Play();
		if( owner )
			StartCoroutine( ShootCoroutine() );
	}

	private IEnumerator ShootCoroutine()
	{
		yield return new WaitForSeconds( delay );
		Instantiate( ballPrefab, spawn.position, spawn.rotation );
		network.SendCreateBall( new Vector2( spawn.position.x, spawn.position.z ), spawn.eulerAngles.y );
	}
}
