using System; // imported in for [Serializable]
using System.Collections.Generic;
using System.Linq; // imported in for FirstOrDefault() method
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.Assertions;

public class GameState : ScriptableObject
{

	private static GameState _instance; // Creates a private static Gamestate instance

	/// <summary>
	/// Creates a static instance for the game state
	/// </summary>
	public static GameState Instance
	{
		get
		{
			if (!_instance) // if no private static GameState exists
			{
				_instance = Resources.FindObjectsOfTypeAll<GameState>()
					.FirstOrDefault();
				Debug.Log("no instance");
			} // find the first object that is GameState
			

#if UNITY_EDITOR
			if (!_instance || _instance.players.Count == 0)
			{
				CreateFromSettings(GameSettings.Instance);
			}
				
#endif

			return _instance; // returns the found or created instance
		}
	}

	[Serializable]
	public class PlayerState
	{
		public TankThinker Tank;
		public int TotalWins;
		[NonSerialized] public GameSettings.PlayerInfo PlayerInfo;

		public bool IsAlive {  get { return Tank && Tank.gameObject.activeSelf; } }
	}

	public List<PlayerState> players; // declares a list of player states

	public int RoundNumber; // integer which indicates round number

	/// <summary>
	/// Method to create an instance from settings from MainMenuController
	/// </summary>
	/// <param name="settings"></param>
	public static void CreateFromSettings(GameSettings settings)
	{
		Assert.IsNotNull(settings);

		_instance = CreateInstance<GameState>();
		_instance.hideFlags = HideFlags.HideAndDontSave;

		_instance.players = new List<PlayerState>();
		foreach (var playerInfo in settings.players)
		{
			if (!playerInfo.Brain) continue;

			_instance.players.Add(new PlayerState {PlayerInfo = playerInfo});
		}
	}

	public PlayerState this[GameSettings.PlayerInfo playerInfo]
	{
		get { return players.FirstOrDefault(p => p.PlayerInfo == playerInfo); }
	}

	public PlayerState this[TankThinker thinker]
	{
		get { return players.FirstOrDefault(p => p.Tank == thinker); }
	}

#if UNITY_EDITOR
	[UnityEditor.MenuItem("Window/Game State")]
	public static void ShowGameState()
	{
		UnityEditor.Selection.activeObject = Instance;
	}
#endif

	public PlayerState GetPlayerWithMostWins()
	{
		players.Sort((a, b) => Comparer<int>.Default.Compare(b.TotalWins, a.TotalWins));
		if (players.Count > 1 && players[0].TotalWins == players[1].TotalWins) return null; // Draw
		return players[0];
	}
}
