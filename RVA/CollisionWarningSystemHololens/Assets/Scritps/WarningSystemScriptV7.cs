using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WarningSystemScriptV7 : MonoBehaviour
{
    public GameObject warningSystem;
    public GameObject arrow;
    private GameObject warningSystemInstance;
    private GameObject arrowInstance;
    private Coroutine destroyCoroutine;
    private double denmLatitude = 0.0;
    private double denmLongitude = 0.0;
    private double vamLatitude = 0.0;
    private double vamLongitude = 0.0;
    private float collisionBearing = 0f;
    private float cameraBearing = 0f;
    private float cameraBearingOffset = -1f;
    private bool isInstantiated = false;

    
    void Start()
    {

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

        collisionBearing = CalculateBearing(vamLatitude, vamLongitude, denmLatitude, denmLongitude);
        Debug.Log("Collision Bearing: " + collisionBearing);

        cameraBearing = (Camera.main.transform.rotation.eulerAngles.y + cameraBearingOffset + 360) % 360;
        //Debug.Log("Camera Bearing: " + cameraBearing);

        // Calculate the relative bearing
        double bearingDiff = Math.Abs(collisionBearing - cameraBearing);

        // Adjust for circular bearing differences
        if (bearingDiff > 180)
        {
            bearingDiff = 360 - bearingDiff;
        }

        // Check if the VRU is approaching the hazard
        if (bearingDiff <= 90)
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
                
                GameObject mrCamera = GameObject.Find("MRTK XR Rig/Camera Offset/Main Camera");
                arrowInstance = Instantiate(arrow, mrCamera.transform);
                arrowInstance.transform.localPosition = new Vector3(0f, -4f, 25f);
                arrowInstance.SetActive(true);
                
                isInstantiated = true;
            }

            if (arrowInstance != null)
            {
                // Calculate the arrow bearing relative to the compass bearing
                float arrowYaw = collisionBearing - cameraBearing;

                // Normalize the relative bearing to 0-360
                arrowYaw = (arrowYaw + 360) % 360;

                // Rotate the arrow to point towards the event
                arrowInstance.transform.localRotation = Quaternion.Euler(-25, arrowYaw, (float) CalculateRollFromYaw(arrowYaw));
                arrowInstance.transform.localPosition = new Vector3((float) CalculateXFromYaw(arrowYaw), -4f + (float) CalculateYFromYaw(arrowYaw), 25f);
            }
        } 
        else 
        {
            // If the VRU is moving away from the hazard, destroy the warning system if it's instantiated
            if (warningSystemInstance != null)
            {
                Destroy(warningSystemInstance);
                Destroy(arrowInstance);
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

    public static double CalculateRollFromYaw(float currentYaw)
    {
        Debug.Log("Current Yaw: " + currentYaw);

        if (currentYaw < 360 && currentYaw >= 270)
        {
            Debug.Log("Roll: " + 35 * (1 - Math.Exp((currentYaw - 360) / 90)));
            return 35 * (1 - Math.Exp((currentYaw - 360) / 90));
        }
        else if (currentYaw > 0 && currentYaw <= 90)
        {
            Debug.Log("Roll: " + 35 * (1 - Math.Exp(-currentYaw / 90)));
            return -35 * (1 - Math.Exp(-currentYaw / 90));
        }
        else
        {
            return 0;
        }
    }

    public static double CalculateXFromYaw(float currentYaw)
    {
        if (currentYaw < 360 && currentYaw >= 270)
        {
            return -3 * (1 - Math.Exp((currentYaw - 360) / 90));
        }
        else if (currentYaw > 0 && currentYaw <= 90)
        {
            return 3 * (1 - Math.Exp(-currentYaw / 90));
        }
        else
        {
            return 0;
        }
    }

    public static double CalculateYFromYaw(float currentYaw)
    {
        //Debug.Log("Current Yaw: " + currentYaw);

        // Y value decreases exponentially as yaw decreases from 270 to 360
        if (currentYaw < 360 && currentYaw >= 270)
        {
            //Debug.Log("Y: " + -3 * (1 - Math.Exp((currentYaw - 360) / 90)));
            return -2 * (1 - Math.Exp((currentYaw - 360) / 90));
        }
        // Y value decreases exponentially as yaw increases from 0 to 90
        else if (currentYaw > 0 && currentYaw <= 90)
        {
            //Debug.Log("Y: " + -3 * (1 - Math.Exp(-currentYaw / 90)));
            return -2 * (1 - Math.Exp(-currentYaw / 90));
        }
        else
        {
            return 0;
        }
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
        yield return new WaitForSeconds(0.5f);
        if (warningSystemInstance != null)
        {
            Destroy(warningSystemInstance);
            Destroy(arrowInstance);
        }
        isInstantiated = false;
    }
}
