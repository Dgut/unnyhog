using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Network : MonoBehaviour
{
	public PlayerHealth localPlayer;
	
	public PlayerController playerPrefab;
	public BallController ballPrefab;

	private PlayerController remotePlayer;
	private BallShooter remoteShooter;

	private ConnectionConfig config;
	private HostTopology topology;

	private int channelId;

	void Start()
	{
		Application.runInBackground = true;
		
		NetworkTransport.Init();

		config = new ConnectionConfig();
		channelId = config.AddChannel( QosType.Reliable );

		topology = new HostTopology( config, 10 );
	}

	private int socketId = -1;

	private void Disconnect()
	{
		if( socketId != -1 )
			NetworkTransport.RemoveHost( socketId );
		socketId = -1;
	}

	public void CreateHost( int port )
	{
		Disconnect();
		socketId = NetworkTransport.AddHost( topology, port );
	}

	private int connectionId = -1;

	public void Connect( string address, int port )
	{
		Disconnect();
		socketId = NetworkTransport.AddHost( topology );

		byte error;
		connectionId = NetworkTransport.Connect( socketId, address, port, 0, out error );
		Debug.Log( "connectionId: " + connectionId + " error: " + error );
	}

	private void SendData( byte[] buffer )
	{
		if( connectionId == -1 )
			return;
		
		byte error;
		NetworkTransport.Send( socketId, connectionId, channelId, buffer, buffer.Length, out error );
	}

	public void SendPlayerPosition( Vector2 position, Vector2 axis )
	{
		MemoryStream stream = new MemoryStream( 1024 );
		BinaryWriter writer = new BinaryWriter( stream );

		writer.Write( ( byte )Data.PlayerPosition );
		writer.Write( position.x );
		writer.Write( position.y );
		writer.Write( axis.x );
		writer.Write( axis.y );

		SendData( stream.ToArray() );
	}

	public void SendShot()
	{
		MemoryStream stream = new MemoryStream( 1024 );
		BinaryWriter writer = new BinaryWriter( stream );

		writer.Write( ( byte )Data.Shot );

		SendData( stream.ToArray() );
	}

	public void SendCreateBall( Vector2 position, float angle )
	{
		MemoryStream stream = new MemoryStream( 1024 );
		BinaryWriter writer = new BinaryWriter( stream );

		writer.Write( ( byte )Data.CreateBall );
		writer.Write( position.x );
		writer.Write( position.y );
		writer.Write( angle );

		SendData( stream.ToArray() );
	}

	public void SendTakeDamage( int damage )
	{
		MemoryStream stream = new MemoryStream( 1024 );
		BinaryWriter writer = new BinaryWriter( stream );

		writer.Write( ( byte )Data.TakeDamage );
		writer.Write( damage );

		SendData( stream.ToArray() );
	}

	private enum Data : byte
	{
		PlayerPosition = 1,
		Shot,
		CreateBall,
		TakeDamage,
	}

	void Update()
	{
		int recHostId;
		int recConnectionId;
		int recChannelId;
		byte[] recBuffer = new byte[ 1024 ];
		int bufferSize = 1024;
		int dataSize;
		byte error;
		NetworkEventType recNetworkEvent = NetworkTransport.Receive( out recHostId, out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error );

		switch( recNetworkEvent )
		{
		case NetworkEventType.Nothing:
			break;

		case NetworkEventType.ConnectEvent:
			Debug.Log( "remote client connected" );
			connectionId = recConnectionId;
			remotePlayer = Instantiate< PlayerController >( playerPrefab );
			remoteShooter = remotePlayer.GetComponent< BallShooter >();
			break;

		case NetworkEventType.DataEvent:
			ParseData( recBuffer );
			break;

		case NetworkEventType.DisconnectEvent:
			Debug.Log( "remote client disconnected" );
			connectionId = -1;
			Destroy( remotePlayer.gameObject );
			remotePlayer = null;
			remoteShooter = null;
			break;
		}
	}

	private void ParseData( byte[] buffer )
	{
		MemoryStream stream = new MemoryStream( buffer );
		BinaryReader reader = new BinaryReader( stream );

		Data type = ( Data )reader.ReadByte();

		switch( type )
		{
		case Data.PlayerPosition:
			remotePlayer.target.x = reader.ReadSingle();
			remotePlayer.target.z = reader.ReadSingle();
			remotePlayer.axis.x = reader.ReadSingle();
			remotePlayer.axis.y = reader.ReadSingle();
			break;

		case Data.Shot:
			remoteShooter.Shoot();
			break;

		case Data.CreateBall:
			Vector3 position = ballPrefab.transform.position;
			position.x = reader.ReadSingle();
			position.z = reader.ReadSingle();
			Instantiate( ballPrefab, position, Quaternion.Euler( 0, reader.ReadSingle(), 0 ) );
			break;

		case Data.TakeDamage:
			localPlayer.TakeDamage( reader.ReadInt32() );
			break;
		}
	}
}
