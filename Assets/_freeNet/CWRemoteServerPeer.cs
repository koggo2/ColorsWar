using System;
using FreeNet;
using FreeNetUnity;

public class CRemoteServerPeer : IPeer
{
	public CUserToken _token { get; private set; }
	WeakReference _freenetEventManager;

	public CRemoteServerPeer(CUserToken token)
	{
		this._token = token;
		this._token.SetPeer(this);
	}

	public void set_eventmanager(CWFreeNetEventManager event_manager)
	{
		this._freenetEventManager = new WeakReference(event_manager);
	}

	/// <summary>
	/// 메시지를 수신했을 때 호출된다.
	/// 파라미터로 넘어온 버퍼는 워커 스레드에서 재사용 되므로 복사한 뒤 어플리케이션으로 넘겨준다.
	/// </summary>
	/// <param name="buffer"></param>
	void IPeer.OnMessage(Const<byte[]> buffer)
	{
		// 버퍼를 복사한 뒤 CPacket클래스로 감싼 뒤 넘겨준다.
		// CPacket클래스 내부에서는 참조로만 들고 있는다.
		byte[] app_buffer = new byte[buffer.Value.Length];
		Array.Copy(buffer.Value, app_buffer, buffer.Value.Length);
		CPacket msg = new CPacket(app_buffer);
		(this._freenetEventManager.Target as CWFreeNetEventManager).EnqueueNetworkMessage(msg);
	}

	void IPeer.OnRemoved()
	{
		(_freenetEventManager.Target as CWFreeNetEventManager).EnqueueNetworkEvent(NETWORK_EVENT.DISCONNECTED);
	}

	void IPeer.Send(CPacket msg)
	{
		_token.Send(msg);
	}

	void IPeer.Disconnect()
	{
		if (_token != null)
		{
			_token.Disconnect();
		}
	}

	void IPeer.ProcessUserOperation(CPacket msg)
	{
	}
}
