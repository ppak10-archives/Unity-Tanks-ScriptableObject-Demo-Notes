using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Controller for handling events in the main menu scene
/// </summary>
public class MainMenuController : MonoBehaviour
{
	public GameSettings GameSettingsTemplate; // declares variable for game settings

	public Color[] AvailableColors; // declares variable for avaliable tank colors
	public TankBrain[] AvailableTankBrains; // declares variable for avaliable tank brains through a scriptable object

	public UnityEngine.UI.Button PanelSwitcher; // declares variable for panel switcher button to switch between player panel and settings panel
	public GameObject PlayersPanel; // declares players panel for tanks, brains, color
	public GameObject SettingsPanel; // declares settings panel for round limits

	/// <summary>
	/// Path to access saved settings in "tanks-settings.json" file
	/// </summary>
	public string SavedSettingsPath {
		get {
			return System.IO.Path.Combine(Application.persistentDataPath, "tanks-settings.json");
		}
	}

	void Start () {

		// Saved Settings
		if (System.IO.File.Exists(SavedSettingsPath)) // Checks to see if saved settings path already exists
			GameSettings.LoadFromJSON(SavedSettingsPath); // if so, load from the saved settings path
		else // if no saved settings file exists
			GameSettings.InitializeFromDefault(GameSettingsTemplate); // initialize game from default settings

		foreach(var info in GetComponentsInChildren<PlayerInfoController>())
			info.Refresh();

		NumberOfRoundsSlider.value = GameSettings.Instance.NumberOfRounds; // Sets rounds slider in settings to defined game settings value
	}

	/// <summary>
	/// Play method to start the game once Play! button is pressed
	/// </summary>
	public void Play()
	{
		GameSettings.Instance.SaveToJSON(SavedSettingsPath); // Saves current settings to JSON
		GameState.CreateFromSettings(GameSettings.Instance); // Creates game state from settings with a scriptable object
		SceneManager.LoadScene(1, LoadSceneMode.Single); // Uses SceneManager to load game scene
	}

	/// <summary>
	/// Method to cycle through tank colors
	/// </summary>
	/// <param name="color"></param>
	/// <returns></returns>
	public Color GetNextColor(Color color)
	{
		int existingColor = Array.FindIndex(AvailableColors, c => c == color);
		existingColor = (existingColor + 1)%AvailableColors.Length;
		return AvailableColors[existingColor];
	}


	/// <summary>
	/// Method to cycle through tank brains
	/// </summary>
	/// <param name="brain"></param>
	/// <returns></returns>
	public TankBrain GetNextBrain(TankBrain brain)
	{
		if (brain == null)
			return AvailableTankBrains[0];

		int index = Array.FindIndex(AvailableTankBrains, b => b == brain);
		index++;
		return (index < AvailableTankBrains.Length) ? AvailableTankBrains[index] : null;
	}

	public UnityEngine.UI.Text NumberOfRoundsLabel;
	public UnityEngine.UI.Slider NumberOfRoundsSlider;

	public void OnChangeNumberOfRounds(float value)
	{
		GameSettings.Instance.NumberOfRounds = (int) value;

		NumberOfRoundsLabel.text = GameSettings.Instance.NumberOfRounds.ToString();
	}

	public void OnSwitchPanels()
	{
		PlayersPanel.SetActive(!PlayersPanel.activeSelf);
		SettingsPanel.SetActive(!SettingsPanel.activeSelf);
		PanelSwitcher.GetComponentInChildren<UnityEngine.UI.Text>().text = PlayersPanel.activeSelf ? "Settings" : "Players";
	}
}
