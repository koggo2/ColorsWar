using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FreeNet;
using UnityEngine.UI;

[Serializable]
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

	public Transform CanvasUITransform = null;
	public Transform StageTransform = null;
	public GameObject NodePrefab = null;
	public GameObject ArmyPrefab = null;
	public GameObject LinePrefab = null;

	public List<Node> NodeList = new List<Node>();

	public List<CWStageNode> CWStageNodeBufferForTest = new List<CWStageNode>();

	// 네트워크
	private string _input_text;
	private CWNetworkManager _networkManager;
	private List<string> _received_texts;
	private Vector2 _currentScrollPos = new Vector2();

	private CWStageNode _selectedNode = null;
	private Dictionary<int, NodeConnectionData> _nodeLinkDataList = new Dictionary<int, NodeConnectionData>();

	//private GameObject _dragTarget = null;
	//private bool _isMouseDragging = false;
	//private Vector3 _mousePos = Vector3.zero;
	//private GameObject _instDirectionImage = null;

	void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}

		_input_text = "";
		_received_texts = new List<string>();
		_networkManager = GameObject.Find("NetworkManager").GetComponent<CWNetworkManager>();
	}

	void Start()
	{
		ReadStageData();
		DrawLine();
		StartCoroutine(CoroutineUpdate());
	}

	void Update()
	{
		//if (Input.GetMouseButtonDown(0))
		//{
		//	CWUtility.Log("Mouse Button Down");
		//	_dragTarget = GetDragObject();

		//	if (_dragTarget != null)
		//	{
		//		_isMouseDragging = true;
		//		if (DirectionImagePrefab != null)
		//		{
		//			_instDirectionImage = Instantiate(DirectionImagePrefab) as GameObject;
		//			_instDirectionImage.transform.SetParent(CanvasUITransform);
		//			_instDirectionImage.transform.localScale = Vector3.one;
		//			_instDirectionImage.transform.localPosition = _dragTarget.transform.localPosition;
		//		}
		//	}
		//}
		//else if (Input.GetMouseButtonUp(0))
		//{
		//	_isMouseDragging = false;
		//	if (_instDirectionImage != null)
		//	{
		//		Destroy(_instDirectionImage);
		//	}
		//}

		//if (_isMouseDragging)
		//{
		//	//마우스 좌표를 받아온다.
		//	_mousePos = Input.mousePosition;
		//	_mousePos.x -= Screen.width * 0.5f;
		//	_mousePos.y -= Screen.height * 0.5f;

		//	//이미지 수정
		//	var rectCom = _instDirectionImage.GetComponent<RectTransform>();
		//	if (rectCom != null)
		//	{
		//		float size = Vector3.Magnitude(_mousePos - _instDirectionImage.transform.localPosition);
		//		rectCom.sizeDelta = new Vector2(rectCom.sizeDelta.x, size);

		//		Vector3 dragVector = _mousePos - _instDirectionImage.transform.localPosition;
		//		float dotProduct = Vector3.Dot(Vector3.up, dragVector);
		//		float theta = Mathf.Acos(dotProduct / dragVector.magnitude);

		//		if (dragVector.x > 0)
		//			theta *= -1;

		//		_instDirectionImage.transform.localRotation = Quaternion.AngleAxis(theta * 180.0f / Mathf.PI, Vector3.forward);
		//	}
		//}
	}

	void OnGUI()
	{
		// Received text.  
		GUILayout.BeginVertical();
		_currentScrollPos = GUILayout.BeginScrollView(_currentScrollPos,
			GUILayout.MaxWidth(Screen.width), GUILayout.MinWidth(Screen.width),
			GUILayout.MaxHeight(Screen.height - 100), GUILayout.MinHeight(Screen.height - 100));

		foreach (string text in this._received_texts)
		{
			GUILayout.BeginHorizontal();
			GUI.skin.label.wordWrap = true;
			GUILayout.Label(text);
			GUILayout.EndHorizontal();
		}

		GUILayout.EndScrollView();
		GUILayout.EndVertical();


		// Input.  
		GUILayout.BeginHorizontal();
		this._input_text = GUILayout.TextField(this._input_text, GUILayout.MaxWidth(Screen.width - 100), GUILayout.MinWidth(Screen.width - 100),
			GUILayout.MaxHeight(50), GUILayout.MinHeight(50));

		if (GUILayout.Button("Send", GUILayout.MaxWidth(100), GUILayout.MinWidth(100), GUILayout.MaxHeight(50), GUILayout.MinHeight(50)))
		{
			CPacket msg = CPacket.create((short)PROTOCOL.CHAT_MSG_REQ);
			msg.push(this._input_text);
			this._networkManager.Send(msg);

			this._input_text = "";
		}
		GUILayout.EndHorizontal();
	}

	/// <summary>
	/// 스테이지 데이터를 읽어서 화면 구성을 채워줘야 한다.
	/// 일단 Node를 읽고 생성한뒤, 노드의 연결을 채워주자.
	/// (구성하는 부분이 이 클래스말고 다른 곳에 위치해도 되는가? 생각해 보자.)
	/// </summary>
	private void ReadStageData()
	{
		for (int i = 0; i < NodeList.Count; ++i)
		{
			Node node = NodeList[i];
			CWStageNode originNode = node.StageNode;

			if (originNode == null)
				continue;

			if (originNode.NodeLinkList == null)
				originNode.NodeLinkList = new List<CWStageNode>();

			for (int linkIndex = 0; linkIndex < node.LinkedNodeIdList.Count; ++linkIndex)
			{
				CWStageNode targetNode = NodeList[node.LinkedNodeIdList[linkIndex]].StageNode;
				if(targetNode != null)
					originNode.NodeLinkList.Add(targetNode);
			}
		}
	}

	private void DrawLine()
	{
		if (LinePrefab == null)
		{
			return;
		}

		for (int i = 0; i < NodeList.Count; ++i)
		{
			Node node = NodeList[i];
			if (node.StageNode != null && node.LinkedNodeIdList != null)
			{
				for (int idListIndex = 0; idListIndex < node.LinkedNodeIdList.Count; ++idListIndex)
				{
					Node targetNode = NodeList[node.LinkedNodeIdList[idListIndex]];

					GameObject line = Instantiate(LinePrefab) as GameObject;
					LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
					if (lineRenderer != null)
					{
						lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
						lineRenderer.SetColors(Color.yellow, Color.black);
						lineRenderer.SetWidth(0.2F, 0.2F);
						lineRenderer.SetVertexCount(2);
						lineRenderer.SetPositions(new Vector3[]
						{
						new Vector3(node.StageNode.Position.x, node.StageNode.Position.y, 1),
						new Vector3(targetNode.StageNode.Position.x, targetNode.StageNode.Position.y, 1),
						});
					}
				}
			}
		}
	}

	public void ClickOccupiedNode(CWStageNode cwStageNode)
	{
		if(_selectedNode != null)
		{
			if (_selectedNode == cwStageNode)
			{
				SetDispersionNode();
			}
			else
			{
				SetNodeLink(cwStageNode);
			}
		}
		else
		{
			SetActivatedNode(cwStageNode);
		}
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
		// 선택된 노드가 없다면 아무것도 수행하지 않는다.
		if (_selectedNode == null)
		{
			return;
		}

		// 선택된 노드와 대상 노드는 서로 연결관계가 아니라면 아무것도 수행하지 않는다.
		if (!NodeList[_selectedNode.DefaultId].LinkedNodeIdList.Contains(cwStageNode.DefaultId))
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
					armyObject.transform.SetParent(StageTransform);
					armyObject.transform.localScale = Vector3.one;

					iter.Value.SendColor(armyObject);
				}
			}
			//yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(0.6f);
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
		if(Camera.main != null)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			//마우스 근처에 오브젝트가 있는지 확인
			if (true == (Physics.Raycast(ray.origin, ray.direction * 10, out hit)))
			{
				//있으면 오브젝트를 저장한다.
				target = hit.collider.gameObject;
			}
		}

		return target;
	}
}
