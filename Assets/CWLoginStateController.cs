using UnityEngine;
using System.Collections;

public class CWLoginStateController : MonoBehaviour, IStateController
{
	public event OnStateCompleted OnStateCompleted;

	public void OnClickLogin()
	{
		if (CWNetworkManager.Instance != null)
		{
			CWNetworkManager.Instance.OnSuccessConnected += OnSuccessNetworkConnected;
			CWNetworkManager.Instance.Connect();
		}
	}

	private void OnSuccessNetworkConnected()
	{
		if (OnStateCompleted != null) OnStateCompleted.Invoke();
	}
}
