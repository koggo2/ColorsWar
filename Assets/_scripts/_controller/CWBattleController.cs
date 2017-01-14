using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWBattleController : CWSingletonBehavior<CWBattleController>
{
	private CWStageNode _firstSelectedNode = null;
	private CWStageNode _secondSelectedNode = null;
	
	override protected void Awake()
	{
		base.Awake();
	}

	public void SelectStageNode(CWStageNode node)
	{
		CWUtility.Log(node);
		if (_firstSelectedNode == null)
		{
			_firstSelectedNode = node;
			return;
		}
		else if (_secondSelectedNode == null)
		{
			_secondSelectedNode = node;
			CreateTask_NodeSelect();
			return;
		}
	}

	private void ClearSelection()
	{
		_firstSelectedNode = null;
		_secondSelectedNode = null;
	}

	private void CreateTask_NodeSelect()
	{
		CWUtility.Log(CWTaskManager.Instance);
		if (CWTaskManager.Instance != null)
		{
			CWTaskManager.Instance.CreateTask_NodeSelect(_firstSelectedNode, _secondSelectedNode);
		}
	}
}
