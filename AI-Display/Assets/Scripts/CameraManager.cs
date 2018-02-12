using UnityEngine;

/// <summary>
/// Author: Dante Nardo
/// Controls which camera is active at any point in time.
/// Cycles between the cameras with the 'C' key.
/// </summary>
public class CameraManager : MonoBehaviour
{
	#region Camera Variables
	public Camera[] cameras;
	private GUIText text;
	private int currentCameraIndex;
	#endregion

	/// <summary>
	/// Initializes the cameras and camera switching system.
	/// </summary>
	void Start()
	{
		InitializeCameras();
		text = GetComponentInChildren<GUIText>();
	}

	/// <summary>
	/// Updates the camera cycling.
	/// </summary>
	void Update()
	{
		CycleCameras();
		DisplayGUIText();
	}

	/// <summary>
	/// Initializes the camera data and activation.
	/// </summary>
	private void InitializeCameras()
	{
		currentCameraIndex = 0;

		for (int i = 1; i < cameras.Length; i++)
			cameras[i].gameObject.SetActive(false);

		if (cameras.Length > 0)
			cameras[0].gameObject.SetActive(true);
	}

	/// <summary>
	/// Handles the logic for switching between cameras in the scene.
	/// </summary>
	private void CycleCameras()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			currentCameraIndex++;

			// Deactivates current camera and activates next current camera
			if (currentCameraIndex < cameras.Length)
			{
				cameras[currentCameraIndex-1].gameObject.SetActive(false);
				cameras[currentCameraIndex].gameObject.SetActive(true);
			}
			// Cycles back to first camera
			else
			{
				cameras[currentCameraIndex-1].gameObject.SetActive(false);
				currentCameraIndex = 0;
				cameras[currentCameraIndex].gameObject.SetActive(true);
			}
		}
	}

	/// <summary>
	/// Displays the current camera type using the GUI.
	/// </summary>
	void DisplayGUIText()
	{
		switch (currentCameraIndex)
		{
			case 0:
				text.text = "Terrain Overview Camera";
				break;
			case 1:
				text.text = "Leaders Camera";
				break;
			case 2:
				text.text = "Horde Camera";
				break;
			case 3:
				text.text = "Terrain Side Camera";
				break;
			case 4:
				text.text = "First Person Camera";
				break;
		}
		text.text += "\nPRESS 'C' to switch cameras.";
		text.text += "\nPRESS 'F' to activate and deactivate power analysis mode.";
	}
}
