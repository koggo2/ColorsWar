using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWTask_SendArmy : ICWTask
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
	
	public void DoTask()
	{
	}
}
