using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour
{
	public bool owner = true;

	public float speed = 15f;
	public float maxDistance = 30f;

	public int damage = 20;

	public GameObject impactPrefab;

	private float distance = 0;

	private Network network;

	void Start()
	{
		network = FindObjectOfType< Network >();

		Collider[] colliders = Physics.OverlapSphere( transform.position, 0 );
		if( colliders.Length == 0 )
			return;

		foreach( Collider c in colliders )
		{
			PlayerHealth playerHealth = c.GetComponent< PlayerHealth >();
			if( playerHealth )
				Kill( playerHealth );
		}

		Kill( null );
	}

	void Update()
	{
		float current = Time.deltaTime * speed;

		RaycastHit hit;

		if( Physics.Raycast( transform.position, transform.forward, out hit, current ) )
		{
			Kill( hit.collider.GetComponent< PlayerHealth >() );
		}

		transform.position += transform.forward * current;
		distance += current;

		if( distance >= maxDistance )
			Destroy( gameObject );
	}

	private void Kill( PlayerHealth playerHealth )
	{
		if( owner && playerHealth )
		{
			playerHealth.TakeDamage( damage );
			network.SendTakeDamage( damage );
		}
		
		Instantiate( impactPrefab, transform.position, transform.rotation );
		Destroy( gameObject );
	}
}
