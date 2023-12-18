using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;


public class CompassControllerV6 : MonoBehaviour
{
    private float bearing = 0f;
    public TMP_Text orientationText;

    void Start()
    {
        // Check if the device supports the compass sensor
        StartCoroutine(InitializeLocation());
        Input.compass.enabled = true;

         // Enable the gyroscope
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }
        else
        {
            Debug.Log("Gyroscope not supported on this device.");
        }
    }

    void Update()
    {
        if (Input.compass.enabled && Input.gyro.enabled)
        {
            // Get the true heading from the compass
            float compassHeading = Input.compass.trueHeading;

            // Get the device's rotation in Euler angles
            Quaternion deviceRotation = Input.gyro.attitude;
            Vector3 rotationEuler = deviceRotation.eulerAngles;

            // Determine if the device is held flat or upright
            if (Mathf.Abs(rotationEuler.x) < 45 || Mathf.Abs(rotationEuler.x) > 135)
            {
                bearing = compassHeading;
            }
            else
            {
                bearing = compassHeading - rotationEuler.y;
            }

            // Normalize the adjusted heading to be within 0 to 360 degrees
            bearing = (bearing + 360) % 360;

            // Display the adjusted bearing
            orientationText.text = "Orientation: " + Mathf.Round(bearing).ToString("F0") + " degrees";
        }
    }

    public float GetCompassBearing()
    {
        return bearing;
    }

    void OnDestroy()
    {
        // Disable compass updates when the script is destroyed
        Input.compass.enabled = false;
    }

    public IEnumerator InitializeLocation()
    {
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("location disabled by user");
            yield break;
        }

        // enable compass
        Input.compass.enabled = true;

        // start the location service
        Debug.Log("start location service");
        Input.location.Start(10, 0.01f);

        // Wait until service initializes
        int maxSecondsToWaitForLocation = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxSecondsToWaitForLocation > 0)
        {
            yield return new WaitForSeconds(1);
            maxSecondsToWaitForLocation--;
        }
        
        // Service didn't initialize in 20 seconds
        if (maxSecondsToWaitForLocation < 1)
        {
            Debug.Log("location service timeout");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("unable to determine device location");
            yield break;
        }

        Debug.Log("location service loaded");

        yield break;
    }
}
