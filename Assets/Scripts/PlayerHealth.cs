using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
	public Animator animator;
	public Transform healthbarPosition;

	public Slider sliderPrefab;

	private Slider slider;

	private int health = 100;

	void Start()
	{
		Canvas canvas = FindObjectOfType< Canvas >();
		slider = Instantiate< Slider >( sliderPrefab );
		slider.transform.SetParent( canvas.transform, false );
	}

	void OnDestroy()
	{
		Destroy( slider.gameObject );
	}

	void Update()
	{
		slider.value = health;

		slider.transform.position = Camera.main.WorldToScreenPoint( healthbarPosition.position );
	}

	public void TakeDamage( int damage )
	{
		if( health <= 0 )
			return;

		health -= damage;

		if( health <= 0 )
		{
			animator.SetTrigger( "death" );
			GetComponent< PlayerController >().enabled = false;
			GetComponent< BallShooter >().enabled = false;
		}
		else
			animator.SetTrigger( "damage" );
	}
}
