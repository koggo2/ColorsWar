using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWStageCoreEngine : CWSingletonBehavior
{
	private const float NODE_UPDATE_TIME = 1f;

	private List<ICWTask> _coreTaskList = null;

	protected override void Awake()
	{
		base.Awake();

		_coreTaskList = new List<ICWTask>();
	}

	void Start()
	{
		StartCoroutine(CoreNodeUpdateRoutine());
	}

	public void AddTask(ICWTask task)
	{
		if (task is CWNodeUpdateTask)
		{
			_coreTaskList.Add(task);
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
