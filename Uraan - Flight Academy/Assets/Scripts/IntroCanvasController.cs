using UnityEngine;

public class IntroCanvasController : MonoBehaviour
{
    public AutopilotPathFlight autopilot;
    public GameObject introCanvas;

    public void OnStartButtonPressed()
    {
        introCanvas.SetActive(false);   // hide UI screen
        autopilot.StartDemoFlight();    // start the flight
    }
}
