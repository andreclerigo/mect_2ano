using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class YourInstantiationScript : MonoBehaviour
{
    public GameObject prefabCar;
    public string buttonName;
    public GameObject warningSystem;
    
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
        }
    }

    void PlaySafeAnimation(Animator animator)
    {
        animator.Play("CarDriveAndStop");
    }

    void PlayDangerousAnimation(Animator animator)
    {
        animator.Play("CarDrive");

        // Instantiate the warningSystem GameObject as a child of ButtonCanvas
        GameObject warningSystemInstance = Instantiate(warningSystem, GameObject.Find("ButtonCanvas").transform);
        // Adjust the rect transform of the warningSystemInstance
        RectTransform rectTransform = warningSystemInstance.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector3(90f, -400f, 0f);

        // Get the "CarSim" animation clip
        AnimationClip carSimClip = animator.runtimeAnimatorController.animationClips.FirstOrDefault(clip => clip.name == "CarDrive");
        // Destroy the warningSystemInstance 2 seconds after the animatition ends
        Destroy(warningSystemInstance, carSimClip.length + 2f);
    }
}
