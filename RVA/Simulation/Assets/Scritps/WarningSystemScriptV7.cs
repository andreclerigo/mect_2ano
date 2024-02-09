using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WarningSystemScriptV7 : MonoBehaviour
{
    public GameObject warningSystem;
    public TMP_Text denmText;
    public GameObject arrow;
    private GameObject warningSystemInstance;
    private GameObject arrowInstance;
    private Coroutine destroyCoroutine;
    private double denmLatitude = 0.0;
    private double denmLongitude = 0.0;
    private double vamLatitude = 0.0;
    private double vamLongitude = 0.0;
    private float collisionBearing = 0f;
    private float compassBearing = 0f;
    private bool isInstantiated = false;

    public CompassControllerV6 compassReference;
    
    void Start()
    {

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
        denmText.text = "DENM: " + Mathf.RoundToInt(collisionBearing) + " degrees";

        compassBearing = compassReference.GetCompassBearing();
        //Debug.Log("Compass Bearing: " + compassBearing);

        // Calculate the relative bearing
        double bearingDiff = Math.Abs(collisionBearing - compassBearing);

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
                warningSystemInstance = Instantiate(warningSystem, GameObject.Find("ButtonCanvas").transform);
                RectTransform rectTransform = warningSystemInstance.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector3(90f, -400f, 0f);
                
                GameObject arCamera = GameObject.Find("AR Session Origin/AR Camera");
                arrowInstance = Instantiate(arrow, arCamera.transform);
                arrowInstance.transform.localPosition = new Vector3(0f, -2f, 25f);
                arrowInstance.SetActive(true);
                
                isInstantiated = true;
            }

            if (arrowInstance != null)
            {
                // Calculate the arrow bearing relative to the compass bearing
                float arrowYaw = collisionBearing - compassBearing;

                // Normalize the relative bearing to 0-360
                arrowYaw = (arrowYaw + 360) % 360;

                // Rotate the arrow to point towards the event
                arrowInstance.transform.localRotation = Quaternion.Euler(-25, arrowYaw, (float) CalculateRollFromYaw(arrowYaw));
                arrowInstance.transform.localPosition = new Vector3((float) CalculateXFromYaw(arrowYaw), -1 + (float) CalculateYFromYaw(arrowYaw), 25f);
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
            return -4 * (1 - Math.Exp((currentYaw - 360) / 90));
        }
        else if (currentYaw > 0 && currentYaw <= 90)
        {
            return 4 * (1 - Math.Exp(-currentYaw / 90));
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
            return -3 * (1 - Math.Exp((currentYaw - 360) / 90));
        }
        // Y value decreases exponentially as yaw increases from 0 to 90
        else if (currentYaw > 0 && currentYaw <= 90)
        {
            //Debug.Log("Y: " + -3 * (1 - Math.Exp(-currentYaw / 90)));
            return -3 * (1 - Math.Exp(-currentYaw / 90));
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
