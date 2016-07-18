using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Node
{
	public int NodeId = 0;
	public CWStageNode StageNode = null;
	public List<int> LinkedNodeIdList = new List<int>();

	public Node()
	{
	}

	public Node(int id, CWStageNode stageNode)
	{
		NodeId = id;
		StageNode = stageNode;
	}
}

public class StageData
{
	public Dictionary<int, Node> NodeMap = new Dictionary<int, Node>();
}

public class CWMainStageManager : MonoBehaviour
{
	#region static
	public static CWMainStageManager Instance
	{
		get { return _instance; }
	}

	private static CWMainStageManager _instance = null;
	#endregion

	public Camera MainCam = null;
	public Transform CanvasTransform = null;
	public GameObject NodePrefab = null;
	public GameObject ArmyPrefab = null;
	public GameObject DirectionImagePrefab = null;

	public List<CWStageNode> CWStageNodeBufferForTest = new List<CWStageNode>();

	private StageData _stageData = null;
	private CWStageNode _selectedNode = null;
	private Dictionary<int, NodeConnectionData> _nodeLinkDataList = new Dictionary<int, NodeConnectionData>();

	private GameObject _dragTarget = null;
	private bool _isMouseDragging = false;
	private Vector3 _mousePos = Vector3.zero;
	private GameObject _instDirectionImage = null;

	void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
	}

	void Start()
	{
		ReadStageData();
		StartCoroutine(CoroutineUpdate());
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			CWUtility.Log("Mouse Button Down");
			_dragTarget = GetDragObject();

			if (_dragTarget != null)
			{
				_isMouseDragging = true;
				if (DirectionImagePrefab != null)
				{
					_instDirectionImage = Instantiate(DirectionImagePrefab) as GameObject;
					_instDirectionImage.transform.SetParent(CanvasTransform);
					_instDirectionImage.transform.localScale = Vector3.one;
					_instDirectionImage.transform.localPosition = _dragTarget.transform.localPosition;
				}
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			_isMouseDragging = false;
			if (_instDirectionImage != null)
			{
				Destroy(_instDirectionImage);
			}
		}

		if (_isMouseDragging)
		{
			//마우스 좌표를 받아온다.
			_mousePos = Input.mousePosition;
			_mousePos.x -= Screen.width * 0.5f;
			_mousePos.y -= Screen.height * 0.5f;

			//이미지 수정
			var rectCom = _instDirectionImage.GetComponent<RectTransform>();
			if (rectCom != null)
			{
				float size = Vector3.Magnitude(_mousePos - _instDirectionImage.transform.localPosition);
				rectCom.sizeDelta = new Vector2(rectCom.sizeDelta.x, size);

				Vector3 dragVector = _mousePos - _instDirectionImage.transform.localPosition;
				float dotProduct = Vector3.Dot(Vector3.up, dragVector);
				float theta = Mathf.Acos(dotProduct / dragVector.magnitude);

				if (dragVector.x > 0)
					theta *= -1;

				_instDirectionImage.transform.localRotation = Quaternion.AngleAxis(theta * 180.0f / Mathf.PI, Vector3.forward);
			}
		}
	}

	/// <summary>
	/// 스테이지 데이터를 읽어서 화면 구성을 채워줘야 한다.
	/// 일단 Node를 읽고 생성한뒤, 노드의 연결을 채워주자.
	/// (구성하는 부분이 이 클래스말고 다른 곳에 위치해도 되는가? 생각해 보자.)
	/// </summary>
	private void ReadStageData()
	{
		_stageData = new StageData();

		_stageData.NodeMap.Add(0, new Node(0, CWStageNodeBufferForTest[0]));
		_stageData.NodeMap.Add(1, new Node(1, CWStageNodeBufferForTest[1]));
		_stageData.NodeMap.Add(2, new Node(2, CWStageNodeBufferForTest[2]));
		_stageData.NodeMap.Add(3, new Node(3, CWStageNodeBufferForTest[3]));
	}

	/// <summary>
	/// 점령된 노드중 활성화된 노드를 지정해준다.
	/// </summary>
	/// <param name="cwStageNode"></param>
	public void SetActivatedNode(CWStageNode cwStageNode)
	{
		if (_selectedNode != null)
		{
			// for test.
			_selectedNode.SetSelected(false);
			_selectedNode = null;
		}

		_selectedNode = cwStageNode;
		if(_selectedNode != null)
		{
			_selectedNode.SetSelected(true);
		}
	}

	/// <summary>
	/// 미점령된 노드와 활성화된 노드를 연결 시켜 게임을 진행시킨다.
	/// </summary>
	/// <param name="cwStageNode"></param>
	public void SetNodeLink(CWStageNode cwStageNode)
	{
		CWUtility.Log(cwStageNode.DefaultId);
		if (_selectedNode == null)
		{
			return;
		}

		if (cwStageNode.State == NodeState.Occupied)
		{
			return;
		}
		
		if (!_nodeLinkDataList.ContainsKey(_selectedNode.NodeId))
		{
			_nodeLinkDataList.Add(_selectedNode.NodeId, null);
		}

		NodeConnectionData connectionData = new NodeConnectionData(_selectedNode, cwStageNode);
		_nodeLinkDataList[_selectedNode.NodeId] = connectionData;

		_selectedNode.SetSelected(false);
		_selectedNode = null;
	}

	/// <summary>
	/// 점령된 노드를 다시 클릭했을 때(차후에는 더블클릭으로), 분산해서 보내는 기능을 활성화 시킨다.
	/// </summary>
	public void SetDispersionNode()
	{
		if (_selectedNode == null)
		{
			return;
		}

		if (!_nodeLinkDataList.ContainsKey(_selectedNode.NodeId))
		{
			_nodeLinkDataList.Add(_selectedNode.NodeId, null);
		}

		NodeConnectionData connectionData = new NodeConnectionData(NodeConnectionData.NodeLinkState.Dispersion, _selectedNode);
		_nodeLinkDataList[_selectedNode.NodeId] = connectionData;

		_selectedNode.SetSelected(false);
		_selectedNode = null;
	}

	private IEnumerator CoroutineUpdate()
	{
		while (true)
		{
			if (_nodeLinkDataList != null && ArmyPrefab != null)
			{
				foreach (var iter in _nodeLinkDataList)
				{
					var armyObject = Instantiate(ArmyPrefab);
					armyObject.transform.SetParent(CanvasTransform);
					armyObject.transform.localScale = Vector3.one;

					iter.Value.SendColor(armyObject);
				}
			}
			//yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(1f);
		}
	}

	/// <summary>
	/// 마우스가 내려간 오브젝트를 가지고 옵니다.
	/// </summary>
	/// <returns>선택된 오브젝트</returns>
	private GameObject GetDragObject()
	{
		//충돌이 감지된 영역
		RaycastHit hit;
		//찾은 오브젝트
		GameObject target = null;

		//마우스 포이트 근처 좌표를 만든다.
		Ray ray = MainCam.ScreenPointToRay(Input.mousePosition);

		//마우스 근처에 오브젝트가 있는지 확인
		if (true == (Physics.Raycast(ray.origin, ray.direction * 10, out hit)))
		{
			//있으면 오브젝트를 저장한다.
			target = hit.collider.gameObject;
		}

		return target;
	}
}
