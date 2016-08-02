using System;
using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using FreeNet;
using FreeNetUnity;

public class CWNetworkManager : MonoBehaviour
{
	private static CWNetworkManager _instance = null;
	public static CWNetworkManager Instance { get { return _instance;} }

	public Action OnSuccessConnected;
	public Action OnSuccessLogin;
	private CWFreeNetUnityService _gameServer;

	void Awake()
	{
		// 네트워크 통신을 위해 CWFreeNetUnityService객체를 추가합니다.
		this._gameServer = gameObject.AddComponent<CWFreeNetUnityService>();
		
		// 상태 변화(접속, 끊김 등)를 통보 받을 델리게이트 설정.
		this._gameServer.AppCallbackOnStatusChanged += OnStatusChanged;

		// 패킷 수신 델리게이트 설정.
		this._gameServer.AppCallbackOnMessage += OnMessage;

		_instance = this;
	}

	void Start()
	{
		//Connect();
	}

	void OnDestroy()
	{
		_gameServer.Disconnect();
	}

	public void Connect()
	{
		_gameServer.Connect("127.0.0.1", 7979);
	}

	private void OnMessage(CPacket msg)
	{
		// 제일 먼저 프로토콜 아이디를 꺼내온다.  
		PROTOCOL protocol_id = (PROTOCOL)msg.PopProtocol_Id();

		CWUtility.Log("Protocol ID : " + protocol_id);

		// 프로토콜에 따른 분기 처리.  
		switch (protocol_id)
		{
			case PROTOCOL.LOGIN_SUCCESS:
				{
					CWUtility.Log("Login Success.");
					OnSuccessLogin.Invoke();
				}
				break;
			case PROTOCOL.CHAT_MSG_ACK:
				{
					string text = msg.PopString();
					CWUtility.Log("CHAT_MSG_ACK : " + text);
					//GameObject.Find("GameMain").GetComponent<CGameMain>().on_receive_chat_msg(text);
				}
				break;
		}
	}

	/// <summary>
	/// 네트워크 상태 변경 시 호출될 콜백 메소드
	/// </summary>
	/// <param name="status"></param>
	private void OnStatusChanged(NETWORK_EVENT status)
	{
		switch (status)
		{
			case NETWORK_EVENT.CONNECTED:
				{
					CWUtility.Log("Connect Success");

					//CPacket msg = CPacket.create((short) PROTOCOL.CHAT_MSG_REQ);
					//msg.push("Hello!!");
					//this._gameServer.Send(msg);

					OnSuccessConnected.Invoke();
				}
				break;
			case NETWORK_EVENT.DISCONNECTED:
				CWUtility.Log("Disconnected");
				break;
		}
	}

	public void Login(string id)
	{
		CPacket msg = CPacket.create((short) PROTOCOL.LOGIN);
		msg.push(id);
		this._gameServer.Send(msg);
	}

	public void Send(CPacket msg)
	{
		this._gameServer.Send(msg);
	}
}
