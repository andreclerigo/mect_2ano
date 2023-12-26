using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WarningSystemScriptV8 : MonoBehaviour
{
    public GameObject warningSystem;
    public TMP_Text denmText;
    public GameObject car;
    private GameObject warningSystemInstance;
    private GameObject carInstance;
    private Coroutine destroyCoroutine;
    private double denmLatitude = 0.0;
    private double denmLongitude = 0.0;
    private double camLatitude = 0.0;
    private double camLongitude = 0.0;
    private double vamLatitude = 0.0;
    private double vamLongitude = 0.0;
    private float collisionBearing = 0f;
    //private float compassBearing = 0f;
    private float cameraBearing = 0f;
    private float cameraBearingOffset = -1f;
    private bool isInstantiated = false;
    
    void Start()
    {
        //SetVAMCoordinates(40.634825, -8.660323);
        //SetCAMCoordinates(40.634734, -8.660395);
    }

    public void DisplayWarningSystem()
    {
        // Stop the coroutine that destroys the warning system 
        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
        }

        collisionBearing = CalculateBearing(vamLatitude, vamLongitude, denmLatitude, denmLongitude);
        //Debug.Log("Collision Bearing: " + collisionBearing);
        denmText.text = "DENM: " + Mathf.RoundToInt(collisionBearing) + " degrees";

        GameObject arCamera = GameObject.Find("AR Session Origin/AR Camera");
        cameraBearing = (arCamera.transform.rotation.eulerAngles.y + cameraBearingOffset + 360) % 360;
        Debug.Log("Camera Bearing: " + cameraBearing);

        if (!isInstantiated)
        {
            //GameObject arCamera = GameObject.Find("AR Session Origin/AR Camera");
            carInstance = Instantiate(car);
            // Calculate carInstace position in reference to the camera based on latitude and longitude of the camera and the car
            carInstance.transform.position = arCamera.transform.position + CalculatePositionBasedOnLatLong(vamLatitude, vamLongitude, camLatitude, camLongitude);
            carInstance.SetActive(true);
            isInstantiated = true;
        }
        else
        {
            // Calculate carInstace position in reference to the camera based on latitude and longitude of the camera and the car
            //GameObject arCamera = GameObject.Find("AR Session Origin/AR Camera");
            carInstance.transform.position = arCamera.transform.position + CalculatePositionBasedOnLatLong(vamLatitude, vamLongitude, camLatitude, camLongitude);
        }

        // Start the coroutine to destroy the warning system after a delay
        destroyCoroutine = StartCoroutine(DestroyAfterDelay());
    }

    private Vector3 CalculatePositionBasedOnLatLong(double vamLat, double vamLon, double camLat, double camLon)
    {
        float distance = CalculateDistance(vamLat, vamLon, camLat, camLon);
        float bearing = CalculateBearing(vamLat, vamLon, camLat, camLon);

        GameObject arCamera = GameObject.Find("AR Session Origin/AR Camera");
        cameraBearing = (arCamera.transform.rotation.eulerAngles.y + cameraBearingOffset + 360) % 360;
        // Adjust bearing based on the compass bearing
        float adjustedBearing = bearing - cameraBearing;
        adjustedBearing = (adjustedBearing + 360) % 360; // Normalize bearing

        // Assuming a scale of 1 Unity unit = 1 meter for simplicity
        // Convert distance and bearing to Unity world coordinates
        // North is along the z-axis and east is along the x-axis
        /*
        Vector3 position = new Vector3(
            distance * Mathf.Sin(adjustedBearing * Mathf.Deg2Rad),  // x-coordinate
            0,                                                      // y-coordinate (altitude not considered)
            distance * Mathf.Cos(adjustedBearing * Mathf.Deg2Rad)   // z-coordinate
        );
        */

        Vector3 position = Quaternion.Euler(0, adjustedBearing, 0) * Vector3.forward * distance;
        position = arCamera.transform.TransformPoint(position);

        Debug.Log("Distance: " + distance + ", Car Bearing: " + bearing + ", Adjusted Bearing: " + adjustedBearing + ", Position: " + position);

        return position;
    }

    private float CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const float R = 6371e3f; // Radius of the Earth in meters
        double phi1 = lat1 * Math.PI / 180.0;
        double phi2 = lat2 * Math.PI / 180.0;
        double deltaPhi = (lat2 - lat1) * Math.PI / 180.0;
        double deltaLambda = (lon2 - lon1) * Math.PI / 180.0;

        // Haversine formula to calculate distance
        double a = Math.Sin(deltaPhi / 2) * Math.Sin(deltaPhi / 2) +
                Math.Cos(phi1) * Math.Cos(phi2) *
                Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        float distance = (float)(R * c); // Distance in meters

        return distance;
    }

    public void SetDENMCoordinates(double latitude, double longitude)
    {
        denmLatitude = latitude;
        denmLongitude = longitude;
    }

    public void SetVAMCoordinates(double latitude, double longitude)
    {
        vamLatitude = latitude;
        vamLongitude = longitude;
    }

    public void SetCAMCoordinates(double latitude, double longitude)
    {
        camLatitude = latitude;
        camLongitude = longitude;
    }

    public void SetHeading(float heading)
    {
        // Set an offset for the Main Camera Rotation Y based on my current Heading
        GameObject arCamera = GameObject.Find("AR Session Origin/AR Camera");
        cameraBearingOffset = heading - arCamera.transform.rotation.eulerAngles.y;
    }

    // Calculates the geographical heading or bearing from point A to point B.
    public static float CalculateBearing(double pointALat, double pointALong, double pointBLat, double pointBLong)
    {
        //Debug.Log("pointALat: " + pointALat + ", pointALong: " + pointALong + ", pointBLat: " + pointBLat + ", pointBLong: " + pointBLong);
        double lat1 = pointALat * Math.PI / 180.0;
        double lon1 = pointALong * Math.PI / 180.0;
        double lat2 = pointBLat * Math.PI / 180.0;
        double lon2 = pointBLong * Math.PI / 180.0;

        double dLon = lon2 - lon1;
        double y = Math.Sin(dLon) * Math.Cos(lat2);
        double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
        double bearing = Math.Atan2(y, x) * 180.0 / Math.PI;

        return (float) (bearing + 360) % 360;
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        if (carInstance != null)
        {
            Destroy(carInstance);
        }
        isInstantiated = false;
    }
}
