using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SimulationScriptV4 : MonoBehaviour
{
    public GameObject prefabCar;
    public string buttonName;
    public GameObject warningSystem;
    private GameObject warningSystemInstance;
    private Coroutine destroyCoroutine;
    private bool isInstantiated = false;
    
    void Start()
    {
        // Access the Animator component and play the default animation
        Animator prefabAnimator = prefabCar.GetComponent<Animator>();
        if (prefabAnimator == null)
        {
            Debug.LogError("Animator component not found on the prefab.");
            return;
        }
        Debug.Log("Animator found on the prefab.");
        

        // Find the button that start the simulationin the scene
        Button buttonStartSim = GameObject.Find("ButtonCanvas/" + buttonName).GetComponent<Button>();
        if (buttonStartSim != null)
        {
            // Assign the PlayAnimation function to the button click event
            buttonStartSim.onClick.AddListener(PlayRandomAnimation);
            Debug.Log("Button found and assigned.");
        }
        else
        {
            Debug.LogError("Button not assigned. Please assign the button in the Inspector.");
        }
    }

    void PlayRandomAnimation()
    {
        Animator prefabAnimator = prefabCar.GetComponent<Animator>();
        if (prefabAnimator != null)
        {
            /*
            int randomAnimation = Random.Range(0, 3);
            if (randomAnimation == 0)
            {
                PlaySafeAnimation(prefabAnimator);
                Debug.Log("Safe animation played.");
            }
            else
            {
                PlayDangerousAnimation(prefabAnimator);
                Debug.Log("Dangerous animation played.");
            }
            */
            PlayDangerousAnimation(prefabAnimator);
            Debug.Log("Dangerous animation played.");
        }
    }

    void PlaySafeAnimation(Animator animator)
    {
        animator.Play("CarDriveAndStop");
    }

    void PlayDangerousAnimation(Animator animator)
    {
        animator.Play("CarDrive");
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
