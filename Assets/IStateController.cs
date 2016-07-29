using System;

public delegate void OnStateCompleted();

public interface IStateController
{
	event OnStateCompleted OnStateCompleted;
}
