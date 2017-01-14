using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWTask_NodeSelect : ICWTask
{
	private CWStageNode _origin = null;
	public CWStageNode Origin
	{
		get
		{
			return _origin;
		}
	}

	private CWStageNode _destination = null;
	public CWStageNode Destination
	{
		get
		{
			return _destination;
		}
	}

	public CWTask_NodeSelect() { }

	public CWTask_NodeSelect(CWStageNode origin, CWStageNode destination)
	{
		_origin = origin;
		_destination = destination;
	}

	public void DoTask()
	{
		if (CWStageCoreEngine.Instance != null)
		{
			
		}
	}
}
