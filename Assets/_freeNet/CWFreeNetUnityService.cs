using System;
using UnityEngine;
using System.Net;
using FreeNet;

namespace FreeNetUnity
{
	public class CWFreeNetUnityService : MonoBehaviour
	{
		CWFreeNetEventManager _eventManager;

		// 연결된 게임 서버 객체.
		IPeer _gameServer;

		// TCP 통신을 위한 서비스 객체.
		CNetworkService _service;

		// 네트워크 상태 변경시 호출되는 델리게이트. 어플리케이션에서 콜백 메소드를 설정하여 사용한다.
		public delegate void StatusChageHandler(NETWORK_EVENT status);
		public StatusChageHandler AppCallbackOnStatusChanged;

		// 네트워크 메시지 수신시 호출되는 델리게이트. 어플리케이션에서 콜백 메소드를 설정하여 사용한다.
		public delegate void MessageHandler(CPacket msg);
		public MessageHandler AppCallbackOnMessage;

		void Awake()
		{
			CPacketBufferManager.initialize(10);
			this._eventManager = new CWFreeNetEventManager();
		}

		/// <summary>  
		/// 네트워크에서 발생하는 모든 이벤트를 클라이언트에게 알려주는 역할을 Update에서 진행한다.  
		/// FreeNet엔진의 메시지 송수신 처리는 워커스레드에서 수행되지만 유니티의 로직 처리는 메인 스레드에서 수행되므로  
		/// 큐잉처리를 통하여 메인 스레드에서 모든 로직 처리가 이루어지도록 구성하였다.  
		/// </summary>
		void Update()
		{
			// 수신된 메시지에 대한 콜백.  
			if (this._eventManager.HasMessage())
			{
				CPacket msg = this._eventManager.DequeueNetworkMessage();
				if (this.AppCallbackOnMessage != null)
				{
					this.AppCallbackOnMessage(msg);
				}
			}

			// 네트워크 발생 이벤트에 대한 콜백.  
			if (this._eventManager.HasEvent())
			{
				NETWORK_EVENT status = this._eventManager.DequeueNetworkEvent();
				if (this.AppCallbackOnStatusChanged!= null)
				{
					this.AppCallbackOnStatusChanged(status);
				}
			}
		}

		public void Connect(string host, int port)
		{
			// CNetworkService객체는 메시지의 비동기 송,수신 처리를 수행한다.  
			this._service = new CNetworkService();

			// endpoint정보를 갖고있는 Connector생성. 만들어둔 NetworkService객체를 넣어준다.  
			CConnector connector = new CConnector(_service);

			// 접속 성공시 호출될 콜백 매소드 지정.  
			connector.connected_callback += OnConnectedGameserver;
			IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(host), port);
			connector.connect(endpoint);
		}

		/// <summary>  
		/// 접속 성공시 호출될 콜백 매소드.  
		/// </summary>  
		/// <param name="server_token"></param>  
		void OnConnectedGameserver(CUserToken server_token)
		{
			this._gameServer = new CRemoteServerPeer(server_token);
			((CRemoteServerPeer)this._gameServer).set_eventmanager(this._eventManager);

			// 유니티 어플리케이션으로 이벤트를 넘겨주기 위해서 매니저에 큐잉 시켜 준다.  
			this._eventManager.EnqueueNetworkEvent(NETWORK_EVENT.connected);
		}

		public void Send(CPacket msg)
		{
			try
			{
				this._gameServer.send(msg);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}
		}
	}
}