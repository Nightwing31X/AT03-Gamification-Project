using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class should be attached to the world-space canvases used for the interactable tooltips.
/// </summary>
public class DigitalTooltip : InteractableObject
{
    [Tooltip("This is the sprite that will be displayed when the tooltip is inactive.")]
    [SerializeField] private Sprite icon;
    [Tooltip("This is the background sprite that will be displayed when the tooltip is active.")]
    [SerializeField] private Sprite background;
    [Tooltip("This is the audio clip that will play when notes are opened/closed.")]
    [SerializeField] private AudioClip interactClip;

    private Image imageRenderer; //should be a child of this object
    private GameObject textObject; //should be a child of the image renderer object
    private AudioSource audioSource;

    //Awake is executed before the Start method
    private void Awake()
    {
        //Create necessary component references
        if (TryGetComponent(out audioSource) == false)
        {
            Debug.LogWarning($"{name} needs an Audio Source attached to it!");
        }
        imageRenderer = GetComponentInChildren<Image>();
        if (imageRenderer == null)
        {
            Debug.LogWarning($"{name} should have a UI Image as a child!");
        }
        else
        {
            textObject = imageRenderer.GetComponentInChildren<Text>().gameObject;
            if (textObject == null)
            {
                Debug.LogWarning($"{imageRenderer.name} should have a UI Text as a child!");
            }
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        //Set icon sprite and disable text
        if (imageRenderer != null)
        {
            imageRenderer.sprite = icon;
        }
        if (textObject != null)
        {
            textObject.SetActive(false);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Rotates the object so that it faces the same direction as the main camera.
        // Canvas's are inverted by default, so by facing the same direction the camera is facing it should appear correct to the player.
        transform.forward = Camera.main.transform.forward;
    }

    /// <summary>
    /// Disables the current tooltip (if there is one) then sets the background sprite and activates the tooltip text.
    /// </summary>
    /// <returns></returns>
    public override bool Activate()
    {
        if (Interaction.Instance.CurrentTooltip != this)
        {
            if (Interaction.Instance.CurrentTooltip != null)
            {
                Interaction.Instance.CurrentTooltip.Deactivate();
            }
            Interaction.Instance.CurrentTooltip = this;
            if (imageRenderer != null)
            {
                imageRenderer.sprite = background;
            }
            if (textObject != null)
            {
                textObject.SetActive(true);
            }
            if (audioSource != null && interactClip != null)
            {
                audioSource.PlayOneShot(interactClip);
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// If this tooltip is the current tooltip, sets the icon sprite then disables the tooltip text.
    /// </summary>
    /// <returns></returns>
    public override bool Deactivate()
    {
        if (Interaction.Instance.CurrentTooltip == this)
        {        
            Interaction.Instance.CurrentTooltip = null;
            if (imageRenderer != null)
            {
                imageRenderer.sprite = icon;
            }
            if (textObject != null)
            {
                textObject.SetActive(false);
            }
            if (audioSource != null && interactClip != null)
            {
                audioSource.PlayOneShot(interactClip);
            }
            return true;
        }
        return false;
    }
}
