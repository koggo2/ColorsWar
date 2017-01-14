using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWTask_UpdateNode : ICWTask
{
	static public CWTask_UpdateNode Create(CWStageNode origin, CWStageNode destination)
	{
		var task = new CWTask_UpdateNode(origin, destination);
		return task;
	}

	public CWTask_UpdateNode() { }
	public CWTask_UpdateNode(CWStageNode origin, CWStageNode destination)
	{
		_originNode = origin;
		_destination = destination;
	}

	private CWStageNode _originNode = null;
	public CWStageNode Origin
	{
		get
		{
			return _originNode;
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

	public void DoTask()
	{
		
	}
}
