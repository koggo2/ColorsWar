using UnityEngine;

public class CWLobbyStateController : MonoBehaviour, IStateController
{
	public event OnStateCompleted OnStateCompleted;

	public void OnClickMatching()
	{
		// 매칭 시작 요청
		OnSuccessMatching();
	}

	public void OnSuccessMatching()
	{
		// 매칭이 완료되었을 때 콜백.
		if (OnStateCompleted != null) OnStateCompleted.Invoke();
	}
}
