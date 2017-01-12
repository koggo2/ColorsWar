using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWTaskManager : CWSingletonBehavior
{
	private Queue<ICWTask> _taskQueue = null;

	override protected void Awake()
	{
		base.Awake ();

		_taskQueue = new Queue<ICWTask> ();
	}

	void Start()
	{
		StartCoroutine (TaskCheckRoutine ());
	}

	public void AddTask(ICWTask task)
	{
		if (_taskQueue != null && task != null) {
			_taskQueue.Enqueue (task);
		}
	}

	/// <summary>
	/// This task for basic game rule.
	/// </summary>
	/// <returns><c>true</c>, if node task was created, <c>false</c> otherwise.</returns>
	public bool CreateNodeTask()
	{
		return false;
	}

	private IEnumerator TaskCheckRoutine()
	{
		do {
			var task = _taskQueue.Dequeue ();
			if (task != null) {
				task.DoTask ();
			}

			yield return new WaitForEndOfFrame();
		} while(_taskQueue != null);
	}
}
