using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum NodeState
{
	Unoccupied = 0,	// 점령되지 않음
	Charged,		// 점령중
	Occupied,		// 점령됨
}

public class CWStageNode : MonoBehaviour//, IPointerDownHandler
{
	// Node Label 상태 표시용
	public Text NodeText = null;
	// Node Button
	public Button NodeButton = null;
	// 점령에 필요한 충전 값.
	public int NodeChargeMaxValue = 10;
	// Node Id
	public int DefaultId = 0;
	// Node 상태
	public NodeState DefaultState = NodeState.Unoccupied;
	// Node 와 연결되어 있는 Node 리스트
	public List<CWStageNode> LinkedStageNode = new List<CWStageNode>(); 

	#region Properties
	public int NodeId { get { return _nodeId; } }

	public NodeState State { get { return _state; } }

	public bool IsSelected { get { return _isSelected; } }

	public Vector3 Position { get { return transform.localPosition; } }
	#endregion

	private int _nodeId = 0;
	private NodeState _state = NodeState.Unoccupied;
	private bool _isSelected = false;
	// 점령중일때 사용, 충전 값
	private int _chargeValue = 0;

	void Start()
	{
		// 기본 상태 값 설정
		_nodeId = DefaultId;
		_state = DefaultState;
		SetNodeText(_state);
	}

	/// <summary>
	/// 점령 중일 경우, 충전 값을 증가 시킨다.
	/// </summary>
	public void IncreaseChargeValue()
	{
		++_chargeValue;

		if (_chargeValue >= NodeChargeMaxValue)
		{
			// 점령에 필요한 값이 되면 상태 변경.
			_state = NodeState.Occupied;
			SetNodeText(_state);
		}
		else
		{
			if (NodeText != null)
			{
				NodeText.text = _chargeValue.ToString();
			}
		}
	}

	/// <summary>
	/// 점령 중일 경우, 상대방에 의해 충전 값을 상쇄 또는 감소시킨다.
	/// </summary>
	public void DecreaseChargeValue()
	{
		--_chargeValue;

		if (NodeText != null)
		{
			NodeText.text = _chargeValue.ToString();
		}
	}

	/// <summary>
	/// 노드 클릭 이벤트 핸들러
	/// </summary>
	public void OnClickNode()
	{
		if (_isSelected)
		{
			CWUtility.Log("Clicked selected node..!");
			if (CWMainStageManager.Instance != null)
			{
				CWMainStageManager.Instance.SetDispersionNode();
			}
			return;
		}

		// 점령된 노드일 경우, 병력을 보내기 위한 준비 작업을 한다.
		if (_state == NodeState.Occupied)
		{
			if (CWMainStageManager.Instance != null)
			{
				CWMainStageManager.Instance.SetActivatedNode(this);
			}
		}
		// 점령되지 않은 노드에 대해 (for test)
		else if (_state == NodeState.Unoccupied)
		{
			if (CWMainStageManager.Instance != null)
			{
				CWMainStageManager.Instance.SetNodeLink(this);
			}
		}
	}

	/// <summary>
	/// 선택된 노드를 판별하기 위해서 색을 변경하기 위한 함수.
	/// 차후에는 색이 아닌 선택 이미지를 그려주는 방식으로 수정되어야 한다.
	/// </summary>
	/// <param name="selected"></param>
	public void SetSelected(bool selected)
	{
		_isSelected = selected;

		if (NodeButton != null && NodeButton.enabled)
		{
			if(_isSelected)
				NodeButton.image.color = Color.green;
			else
				NodeButton.image.color = Color.white;
		}
	}

	/// <summary>
	/// 노드 상태 텍스트를 상태에 맞추어 설정
	/// </summary>
	/// <param name="nodeState">노드 상태 값</param>
	private void SetNodeText(NodeState nodeState)
	{
		if (NodeText != null)
		{
			switch (nodeState)
			{
				case NodeState.Unoccupied:
					NodeText.text = "Unoccupied";
					break;
				case NodeState.Charged:
					break;
				case NodeState.Occupied:
					NodeText.text = "Occupied";
					break;
				default:
					break;
			}
		}
	}

	//public void OnPointerDown(PointerEventData eventData)
	//{
	//	CWUtility.Log(eventData.pointerCurrentRaycast.distance + " " + eventData.dragging);
	//}
}
