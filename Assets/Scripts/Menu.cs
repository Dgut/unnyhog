using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Menu : MonoBehaviour
{
	public InputField address;
	public InputField port;

	private Network network;

	void Start()
	{
		network = FindObjectOfType< Network >();
	}

	public void CreateHost()
	{
		int portValue;
		if( int.TryParse( port.text, out portValue ) )
			network.CreateHost( portValue );
	}

	public void Connect()
	{
		int portValue;
		if( int.TryParse( port.text, out portValue ) )
			network.Connect( address.text, portValue );
	}
}
