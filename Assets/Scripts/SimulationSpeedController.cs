using UnityEngine;
using TMPro;

public class SimulationSpeedController : MonoBehaviour
{
    public TMP_Text speedDisplay;
    public float minSpeed = 0.1f;
    public float maxSpeed = 2.0f;
    public float speedStep = 0.1f;
    private float currentSpeed = 1.0f;

    private void Update()
    {
        // Check for scroll wheel input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // Increase or decrease speed based on the scroll input
        if (scrollInput > 0)
        {
            IncreaseSpeed();
        }
        else if (scrollInput < 0)
        {
            DecreaseSpeed();
        }

        // Update the text display with the current speed
        UpdateSpeedDisplay();
    }

    private void IncreaseSpeed()
    {
        currentSpeed += speedStep;
        if (currentSpeed > maxSpeed)
        {
            currentSpeed = maxSpeed;
        }
        Time.timeScale = currentSpeed;
    }

    private void DecreaseSpeed()
    {
        currentSpeed -= speedStep;
        if (currentSpeed < minSpeed)
        {
            currentSpeed = minSpeed;
        }
        Time.timeScale = currentSpeed;
    }

    private void UpdateSpeedDisplay()
    {
        if (speedDisplay != null)
        {
            speedDisplay.text = $"X{currentSpeed:0.00}";
        }
    }
}
