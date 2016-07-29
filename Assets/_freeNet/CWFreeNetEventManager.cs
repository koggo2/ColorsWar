using System;
using System.Collections;
using System.Collections.Generic;
using FreeNet;

namespace FreeNetUnity
{
	public enum NETWORK_EVENT : byte
	{
		// 접속 완료.  
		connected,

		// 연결 끊김.  
		disconnected,

		// 끝.  
		end
	}

	/// <summary>  
	/// 네트워크 엔진에서 발생된 이벤트들을 큐잉시킨다.  
	/// 워커 스레드와 메인 스레드 양쪽에서 호출될 수 있으므로 스레드 동기화 처리를 적용하였다.  
	/// </summary>  
	public class CWFreeNetEventManager
	{
		// 동기화 객체.  
		object _csEvent;

		// 네트워크 엔진에서 발생된 이벤트들을 보관해놓는 큐.  
		Queue<NETWORK_EVENT> _networkEvents;

		// 서버에서 받은 패킷들을 보관해놓는 큐.  
		Queue<CPacket> _networkMessageEvents;

		public CWFreeNetEventManager()
		{
			this._networkEvents = new Queue<NETWORK_EVENT>();
			this._networkMessageEvents = new Queue<CPacket>();
			this._csEvent = new object();
		}

		public void EnqueueNetworkEvent(NETWORK_EVENT event_type)
		{
			lock (this._csEvent)
			{
				this._networkEvents.Enqueue(event_type);
			}
		}

		public bool HasEvent()
		{
			lock (this._csEvent)
			{
				return this._networkEvents.Count > 0;
			}
		}

		public NETWORK_EVENT DequeueNetworkEvent()
		{
			lock (this._csEvent)
			{
				return this._networkEvents.Dequeue();
			}
		}


		public bool HasMessage()
		{
			lock (this._csEvent)
			{
				return this._networkMessageEvents.Count > 0;
			}
		}

		public void EnqueueNetworkMessage(CPacket buffer)
		{
			lock (this._csEvent)
			{
				this._networkMessageEvents.Enqueue(buffer);
			}
		}

		public CPacket DequeueNetworkMessage()
		{
			lock (this._csEvent)
			{
				return this._networkMessageEvents.Dequeue();
			}
		}
	}
}