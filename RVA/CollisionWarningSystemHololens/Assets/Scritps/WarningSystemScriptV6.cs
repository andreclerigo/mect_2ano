using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WarningSystemScriptV6 : MonoBehaviour
{
    public GameObject warningSystem;
    private GameObject warningSystemInstance;
    private Coroutine destroyCoroutine;
    private double denmLatitude = 0.0;
    private double denmLongitude = 0.0;
    private double vamLatitude = 0.0;
    private double vamLongitude = 0.0;
    private double bearing = 0.0;
    private float cameraBearingOffset = -1f;
    private bool isInstantiated = false;
    
    void Start()
    {
        //SetHeading(200f);
        //Debug.Log("Camera Bearing Offset: " + cameraBearingOffset);
    }

    void Update()
    {
        if (cameraBearingOffset != -1f)
        {
            Debug.Log("Camera Heading: " + (Camera.main.transform.rotation.eulerAngles.y + cameraBearingOffset + 360) % 360);
        }
    }

    public void DisplayWarningSystem()
    {
        // Stop the coroutine that destroys the warning system 
        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
        }

        bearing = CalculateBearing(vamLatitude, vamLongitude, denmLatitude, denmLongitude);
        Debug.Log("Collision Bearing: " + bearing);

        float cameraBearing = (Camera.main.transform.rotation.eulerAngles.y + cameraBearingOffset + 360) % 360;
        //Debug.Log("Camera Bearing: " + cameraBearing);

        // Check if the VRU is approaching the hazard
        if (cameraBearing >= (bearing - 90) && cameraBearing <= (bearing + 90))
        {
            // Instantiate the warning system only if it hasn't been instantiated before
            if (!isInstantiated)
            {
                // Instantiate the warningSystem GameObject as a child of Canvas
                warningSystemInstance = Instantiate(warningSystem, GameObject.Find("MRTK XR Rig/Camera Offset/Main Camera/Canvas").transform);
                // Adjust the rect transform of the warningSystemInstance
                RectTransform rectTransform = warningSystemInstance.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector3(-152f, 50f, 0f);
                warningSystemInstance.SetActive(true);
                
                isInstantiated = true;
                Debug.Log("Warning System Instantiated");
            }
        } 
        else 
        {
            // If the VRU is moving away from the hazard, destroy the warning system if it's instantiated
            if (warningSystemInstance != null)
            {
                Destroy(warningSystemInstance);
                isInstantiated = false;
            }
        }

        // Start the coroutine to destroy the warning system after a delay
        destroyCoroutine = StartCoroutine(DestroyAfterDelay());
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

    public void SetHeading(float heading)
    {
        // Set an offset for the Main Camera Rotation Y based on my current Heading
        cameraBearingOffset = heading - Camera.main.transform.rotation.eulerAngles.y;
    }

    public static double CalculateBearing(double pointALat, double pointALong, double pointBLat, double pointBLong)
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

        return (bearing + 360) % 360;
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        if (warningSystemInstance != null)
        {
            //warningSystem.SetActive(false);
            Destroy(warningSystemInstance);
            Debug.Log("Warning System Destroyed");
        }
        isInstantiated = false;
    }
}
