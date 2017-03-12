using UnityEngine;
using System.Collections;

public class DestroyAfter : MonoBehaviour
{
	public float delay = 1f;

	void Start()
	{
		Destroy( gameObject, delay );
	}
}
