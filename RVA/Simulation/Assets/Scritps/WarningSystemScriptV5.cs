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
            warningSystemInstance = Instantiate(warningSystem, GameObject.Find("ButtonCanvas").transform);
            // Adjust the rect transform of the warningSystemInstance
            RectTransform rectTransform = warningSystemInstance.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector3(90f, -400f, 0f);
            
            isInstantiated = true;
        }

        destroyCoroutine = StartCoroutine(DestroyAfterDelay());
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