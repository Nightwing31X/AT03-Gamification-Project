using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// This class should be attached to the main camera.
/// </summary>
public class MouseLook : MonoBehaviour
{
    [Tooltip("The amount of influence mouse input has on camera movement. Must have a value above 0.")]
    [SerializeField] private float sensitivity;
    [Tooltip("The amount of 'drag' applied to the camera. Must have a value above 0.")]
    [SerializeField] private float drag;
    [Tooltip("The minimum and maximum angle that the camera can move on the y axis.")]
    [SerializeField] private Vector2 verticalClamp = new Vector2(-45, 70);

    private Vector2 smoothing;
    private Vector2 result;
    private Transform character;
    private bool mouseLookEnabled = false;
    [SerializeField] public GameObject player_hud;
    [SerializeField] public GameObject map_menu;


    /// <summary>
    /// Use to turn mouse look on and off. To toggle cursor, use ToggleMouseLook method.
    /// </summary>
    public bool MouseLookEnabled { get { return mouseLookEnabled; } set { ToggleMouseLook(value); } }

    
    //Awake is executed before the Start method
    private void Awake()
    {
        if (transform.parent != null)
        {
            character = transform.parent;
        }
        else
        {
            Debug.LogWarning($"{name} should be the child of an empty object!");
        }

        if (transform.localPosition != Vector3.zero)
        {
            Debug.LogWarning($"{name} should have a local space of (0,0,0)!");
        }
    }


    // Start is called before the first frame update
    private void Start()
    {
        ToggleMouseLook(true, true);
        if (map_menu.activeInHierarchy == true)
        {
            // Reference to the PostProcessVolume 
            PostProcessVolume ppVolume = Camera.main.gameObject.GetComponent<PostProcessVolume>();

            map_menu.SetActive(false);
            ppVolume.enabled = false;
            player_hud.SetActive(true);
        }
    }


    // Update is called once per frame
    private void Update()
    {
        if (mouseLookEnabled == true)
        {
            var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            md = Vector2.Scale(md, new Vector2(sensitivity * drag, sensitivity * drag));
            smoothing.x = Mathf.Lerp(smoothing.x, md.x, 1f / drag);
            smoothing.y = Mathf.Lerp(smoothing.y, md.y, 1f / drag);
            result += smoothing;
            result.y = Mathf.Clamp(result.y, verticalClamp.x, verticalClamp.y);

            transform.localRotation = Quaternion.AngleAxis(-result.y, Vector3.right);
            character.localRotation = Quaternion.AngleAxis(result.x, character.up);
        }

        // Reference to the PostProcessVolume 
        PostProcessVolume ppVolume = Camera.main.gameObject.GetComponent<PostProcessVolume>();

        // Check if the "M" key is pressed
        if (Input.GetKeyDown(KeyCode.M))
        {
            //Debug.Log("M key pressed!");
            if (map_menu.activeInHierarchy == false)
            {
                ppVolume.enabled = true;
                map_menu.SetActive(true);
                ToggleMouseLook(false, true);
                player_hud.SetActive(false);
            }
            else
            {
                map_menu.SetActive(false);
                ToggleMouseLook(true, true);
                ppVolume.enabled = false;
                player_hud.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Toggles the mouse look on and off.
    /// Can optionally toggle the mouse cursor on and off.
    /// </summary>
    /// <param name="mouseLookActive"></param>
    /// <param name="toggleCursor"></param>
    public void ToggleMouseLook(bool mouseLookActive, bool toggleCursor = false)
    {
        mouseLookEnabled = mouseLookActive;
        if (toggleCursor == true)
        {
            if (mouseLookActive == true)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
            Cursor.visible = !mouseLookActive;
        }
    }
}


