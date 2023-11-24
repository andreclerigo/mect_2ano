using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningSystemScriptV5 : MonoBehaviour
{
    public GameObject warningSystem;
    private GameObject warningSystemInstance;
    private Coroutine destroyCoroutine;
    private bool isInstantiated = false;
    
    void Start()
    {
        Debug.Log("500ms");
    }

    public void DisplayWarningSystem()
    {
        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
        }

        if (!isInstantiated)
        {
            // Instantiate the warningSystem GameObject as a child of ButtonCanvas
            warningSystemInstance = Instantiate(warningSystem, GameObject.Find("Canvas").transform);
            // Adjust the rect transform of the warningSystemInstance
            RectTransform rectTransform = warningSystemInstance.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector3(500f, -280f, 0f);
            warningSystem.SetActive(true);
            
            isInstantiated = true;
            Debug.Log("Warning System Instantiated");
        }

        destroyCoroutine = StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        if (warningSystemInstance != null)
        {
            warningSystem.SetActive(false);
            Destroy(warningSystemInstance);
            Debug.Log("Warning System Destroyed");
        }
        isInstantiated = false;
    }
}
