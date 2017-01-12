using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWNodeUpdateTask : ICWTask
{
	static public CWNodeUpdateTask Create(CWStageNode origin, CWStageNode destination)
	{
		var task = new CWNodeUpdateTask(origin, destination);
		return task;
	}

	public CWNodeUpdateTask() { }
	public CWNodeUpdateTask(CWStageNode origin, CWStageNode destination)
	{
		_originNode = origin;
		_destNode = destination;
	}

	private CWStageNode _originNode = null;
	private CWStageNode _destNode = null;

	public void DoTask()
	{
		
	}
}
