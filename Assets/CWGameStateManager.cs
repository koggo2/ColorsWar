using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CWGameStateManager : MonoBehaviour
{
	public enum GameState
	{
		NONE = 0,
		LOGIN,
		LOBBY,
		STAGE,
		STAGE_FINISH,
	}

	public GameObject LoginPrefab = null;
	public GameObject LobbyPrefab = null;
	public GameObject StagePrefab = null;
	public GameObject StageFinishPrefab = null;

	private GameState _currentState = GameState.LOGIN;
	private Dictionary<GameState, GameObject> _stateDictionary = null;

	void Start()
	{
		_stateDictionary = new Dictionary<GameState, GameObject>();
		SetStateDictionary(GameState.LOGIN, LoginPrefab, OnLoginCompleted);
		SetStateDictionary(GameState.LOBBY, LobbyPrefab, OnLobbyCompleted);
		SetStateDictionary(GameState.STAGE, StagePrefab, OnStageCompleted);
		SetStateDictionary(GameState.STAGE_FINISH, StageFinishPrefab, OnStageFinishCompleted);

		LoadState(GameState.STAGE);
	}

	private void SetStateDictionary(GameState gameState, GameObject gameObject, OnStateCompleted stateCompletedCallback)
	{
		if (gameObject != null && !_stateDictionary.ContainsKey(gameState))
		{
			GameObject instStateGameObject = Instantiate(gameObject);
			instStateGameObject.transform.SetParent(this.transform);
			instStateGameObject.transform.localPosition = Vector3.zero;
			instStateGameObject.transform.localScale = Vector3.one;
			instStateGameObject.SetActive(false);

			IStateController controller = instStateGameObject.GetComponent<IStateController>();
			if(controller != null)
			{
				controller.OnStateCompleted += stateCompletedCallback;
			}

			_stateDictionary.Add(gameState, instStateGameObject);
		}
	}

	private void LoadState(GameState gameState)
	{
		SetStateActive(_currentState, false);

		_currentState = gameState;
		SetStateActive(_currentState, true);
	}

	private void SetStateActive(GameState gameState, bool value)
	{
		if (gameState == GameState.NONE)
			return;

		if (_stateDictionary.ContainsKey(gameState))
		{
			if (_stateDictionary[gameState] != null)
			{
				_stateDictionary[_currentState].SetActive(value);
			}
		}
	}

	private void OnLoginCompleted()
	{
		LoadState(GameState.LOBBY);
	}

	private void OnLobbyCompleted()
	{
		LoadState(GameState.STAGE);
	}

	private void OnStageCompleted()
	{
		
	}

	private void OnStageFinishCompleted()
	{
		
	}
}
