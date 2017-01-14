using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWStageCoreEngine : CWSingletonBehavior<CWStageCoreEngine>
{
	private const float NODE_UPDATE_TIME = 1f;

	private List<ICWTask> _coreTaskList = null;
	private Dictionary<int, ICWTask> _coreTaskMap = null;

	protected override void Awake()
	{
		base.Awake();

		_coreTaskList = new List<ICWTask>();
		_coreTaskMap = new Dictionary<int, ICWTask>();
	}

	void Start()
	{
		StartCoroutine(CoreNodeUpdateRoutine());
	}

	public void AddTask(ICWTask task)
	{
		if (task is CWTask_SendArmy)
		{
			//_coreTaskList.Add(task);

			CWTask_SendArmy sendArmyTask = task as CWTask_SendArmy;
			if (!_coreTaskMap.ContainsKey(sendArmyTask.Origin.DefaultId))
			{
				_coreTaskMap.Add(sendArmyTask.Origin.DefaultId, sendArmyTask);
			}
		}
	}

	private IEnumerator CoreNodeUpdateRoutine()
	{
		do
		{
			foreach (var task in _coreTaskList)
			{
				task.DoTask();
			}

			yield return new WaitForSecondsRealtime(NODE_UPDATE_TIME);

		} while (_coreTaskList != null);
	}
}
