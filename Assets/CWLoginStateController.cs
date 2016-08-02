using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CWLoginStateController : MonoBehaviour, IStateController
{
	public enum LoginSceneState
	{
		NOT_CONNECTED = 0,
		CONNECTED,
	}

	public event OnStateCompleted OnStateCompleted;

	public InputField IdInputField = null;

	private LoginSceneState _sceneState = LoginSceneState.NOT_CONNECTED;

	void Start()
	{
		if (CWNetworkManager.Instance != null)
		{
			// delegate 초기화.
			CWNetworkManager.Instance.OnSuccessConnected += OnSuccessNetworkConnected;
			CWNetworkManager.Instance.OnSuccessLogin += OnSuccessLogin;

			CWNetworkManager.Instance.Connect();
		}
	}

	void OnDestroy()
	{
		if (CWNetworkManager.Instance != null)
		{
			// delegate 제거.
			if (CWNetworkManager.Instance.OnSuccessConnected != null)
				CWNetworkManager.Instance.OnSuccessConnected -= OnSuccessNetworkConnected;
			if (CWNetworkManager.Instance.OnSuccessLogin != null)
				CWNetworkManager.Instance.OnSuccessLogin -= OnSuccessLogin;
		}
	}

	public void OnClickLogin()
	{
		if (_sceneState == LoginSceneState.CONNECTED && CWNetworkManager.Instance != null)
		{
			if (IdInputField != null && !string.IsNullOrEmpty(IdInputField.text))
			{
				CWNetworkManager.Instance.Login(IdInputField.text);
			}
		}
	}

	private void OnSuccessNetworkConnected()
	{
		_sceneState = LoginSceneState.CONNECTED;
	}

	private void OnSuccessLogin()
	{
		if (OnStateCompleted != null) OnStateCompleted.Invoke();
	}
}
