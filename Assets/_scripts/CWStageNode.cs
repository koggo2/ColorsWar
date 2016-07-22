using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProgressBar;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum NodeState
{
	Unoccupied = 0,	// 점령되지 않음
	Charged,		// 점령중
	Occupied,		// 점령됨
}

public class CWStageNode : MonoBehaviour
{
	// Sprite Renderer
	public SpriteRenderer SpriteRenderer = null;
	public SpriteRenderer SelectedSprite = null;
	public string OccupiedSpritePath = string.Empty;
	public string UnoccupiedSpritePath = string.Empty;
	public string OccupingSpritePath = string.Empty;
	public GameObject ProgressPrefab = null;
	
	// 점령에 필요한 충전 값.
	public int NodeChargeMaxValue = 10;
	// Node Id
	public int DefaultId = 0;
	// Node 상태
	public NodeState DefaultState = NodeState.Unoccupied;

	// 노드 연결 상태
	public List<CWStageNode> NodeLinkList = new List<CWStageNode>(); 

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
	private ProgressRadialBehaviour _instProgressComp = null;

	void Start()
	{
		// 기본 상태 값 설정
		_nodeId = DefaultId;
		_state = DefaultState;
		SetNodeText(_state);
	}

	void OnGUI()
	{
		Vector2 size = new Vector2(100f, 100f);
		Vector3 screenPoint = Camera.main.WorldToScreenPoint(this.Position);
		Vector2 guiPoint = GUIUtility.ScreenToGUIPoint(screenPoint);

		GUI.BeginGroup(new Rect(guiPoint.x, guiPoint.y, size.x, size.y));
		GUI.Box(new Rect(guiPoint.x, guiPoint.y, size.x, size.y), "A");

		// draw the filled-in part:
		//GUI.BeginGroup(new Rect(0, 0, size.x * 0.5f, size.y));
		//GUI.Box(new Rect(0, 0, size.x, size.y), "A");
		//GUI.EndGroup();

		GUI.EndGroup();
	}

	private void OnMouseDown()
	{
		CWUtility.Log("On Mouse Down");
	}

	/// <summary>
	/// 노드 클릭 이벤트 핸들러
	/// </summary>
	private void OnMouseUpAsButton()
	{
		CWUtility.Log("On Mouse Up as Button");
		//if (_isSelected)
		//{
		//	CWUtility.Log("Clicked selected node..!");
		//	if (CWMainStageManager.Instance != null)
		//	{
		//		CWMainStageManager.Instance.SetDispersionNode();
		//	}
		//	return;
		//}

		// 점령된 노드일 경우, 병력을 보내기 위한 준비 작업을 한다.
		if (_state == NodeState.Occupied)
		{
			if (CWMainStageManager.Instance != null)
			{
				CWMainStageManager.Instance.ClickOccupiedNode(this);
			}
		}
		// 점령되지 않은 노드에 대해 (for test)
		else if (_state == NodeState.Unoccupied || _state == NodeState.Charged)
		{
			if (CWMainStageManager.Instance != null)
			{
				CWMainStageManager.Instance.SetNodeLink(this);
			}
		}
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
		}
		else
		{
			_state = NodeState.Charged;
		}

		SetNodeText(_state);
	}

	/// <summary>
	/// 점령 중일 경우, 상대방에 의해 충전 값을 상쇄 또는 감소시킨다.
	/// </summary>
	public void DecreaseChargeValue()
	{
		--_chargeValue;

		if (_chargeValue <= NodeChargeMaxValue)
			_state = NodeState.Charged;

		SetNodeText(_state);

		//if (NodeText != null)
		//{
		//	NodeText.text = _chargeValue.ToString();
		//}
	}

	/// <summary>
	/// 선택된 노드를 판별하기 위해서 색을 변경하기 위한 함수.
	/// 차후에는 색이 아닌 선택 이미지를 그려주는 방식으로 수정되어야 한다.
	/// </summary>
	/// <param name="selected"></param>
	public void SetSelected(bool selected)
	{
		_isSelected = selected;

		if (SelectedSprite != null)
		{
			SelectedSprite.enabled = selected;
		}
	}

	/// <summary>
	/// 노드 상태 텍스트를 상태에 맞추어 설정
	/// </summary>
	/// <param name="nodeState">노드 상태 값</param>
	private void SetNodeText(NodeState nodeState)
	{
		if (SpriteRenderer != null)
		{
			switch (nodeState)
			{
				case NodeState.Unoccupied:
					SpriteRenderer.sprite = Resources.Load<Sprite>(UnoccupiedSpritePath);
					break;
				case NodeState.Charged:
					SpriteRenderer.sprite = Resources.Load<Sprite>(OccupingSpritePath);
					break;
				case NodeState.Occupied:
					SpriteRenderer.sprite = Resources.Load<Sprite>(OccupiedSpritePath);
					break;
				default:
					break;
			}
		}

		if(nodeState == NodeState.Charged)
		{
			if (_instProgressComp == null && ProgressPrefab != null)
			{
				RectTransform canvasRect = CWMainStageManager.Instance.CanvasUITransform.GetComponent<RectTransform>();

				if (canvasRect != null)
				{
					GameObject progressObj = Instantiate(ProgressPrefab) as GameObject;
					progressObj.transform.SetParent(canvasRect.transform);
					Vector2 viewportPosition = Camera.main.WorldToViewportPoint(this.Position);
					Vector2 screenPosition = new Vector2((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
						(viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f));

					progressObj.transform.localPosition = screenPosition;
					progressObj.transform.localScale = canvasRect.transform.localScale;
					_instProgressComp = progressObj.GetComponent<ProgressRadialBehaviour>();
				}
			}

			if (_instProgressComp != null)
			{
				_instProgressComp.gameObject.SetActive(true);
				_instProgressComp.Value = ((float)_chargeValue / (float)NodeChargeMaxValue) * 100f;
			}
		}
		else
		{
			if(_instProgressComp != null)
				_instProgressComp.gameObject.SetActive(false);
		}
	}
}
