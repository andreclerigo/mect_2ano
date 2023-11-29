using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningSystemScriptV5 : MonoBehaviour
{
    public GameObject warningSystem;
    private GameObject warningSystemInstance;
    private Coroutine destroyCoroutine;
    private float denmLatitude = 0f;
    private float denmLongitude = 0f;
    private float vamLatitude = 0f;
    private float vamLongitude = 0f;
    private float bearing = 0f;
    private float compassBearing = 0f;
    private bool isInstantiated = false;

    public CompassControllerV6 compassReference;
    
    void Start()
    {

    }

    public void DisplayWarningSystem()
    {
        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
        }

        if (!isInstantiated)
        {
            bearing = CalculateBearing(new Vector2(denmLatitude, denmLongitude), new Vector2(vamLatitude, vamLongitude));
            Debug.Log("Bearing: " + bearing);

            compassBearing = compassReference.GetCompassBearing();
            Debug.Log("Compass Bearing: " + compassBearing);
            
            // If the compassBearing is between bearing - 90 and bearing + 90, then the vru is approaching the hazard
            if (compassBearing >= (bearing - 90) && compassBearing <= (bearing + 90))
            {
                // Instantiate the warningSystem GameObject as a child of ButtonCanvas
                warningSystemInstance = Instantiate(warningSystem, GameObject.Find("ButtonCanvas").transform);
                // Adjust the rect transform of the warningSystemInstance
                RectTransform rectTransform = warningSystemInstance.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector3(90f, -400f, 0f);
                
                isInstantiated = true;
            }
        }

        destroyCoroutine = StartCoroutine(DestroyAfterDelay());
    }

    public void SetDENMCoordinates(float latitude, float longitude)
    {
        denmLatitude = latitude;
        denmLongitude = longitude;
    }

    public void SetVAMCoordinates(float latitude, float longitude)
    {
        vamLatitude = latitude;
        vamLongitude = longitude;
    }

    public static float CalculateBearing(Vector2 pointA, Vector2 pointB)
    {
        float lat1 = pointA.x * Mathf.Deg2Rad;
        float lon1 = pointA.y * Mathf.Deg2Rad;
        float lat2 = pointB.x * Mathf.Deg2Rad;
        float lon2 = pointB.y * Mathf.Deg2Rad;

        float dLon = lon2 - lon1;
        float y = Mathf.Sin(dLon) * Mathf.Cos(lat2);
        float x = Mathf.Cos(lat1) * Mathf.Sin(lat2) - Mathf.Sin(lat1) * Mathf.Cos(lat2) * Mathf.Cos(dLon);
        float bearing = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

        return (bearing + 360) % 360; // Normalize to 0-360
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(0.2f);
        if (warningSystemInstance != null)
        {
            Destroy(warningSystemInstance);
        }
        isInstantiated = false;
    }
}
