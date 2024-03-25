using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class should be attached to the objects used for navigation waypoints.
/// </summary>
public class NavigationWaypoint : InteractableObject
{
    [Tooltip("This is the audio clip that will play when notes are opened/closed.")]
    [SerializeField] private AudioClip interactClip;

    private ParticleSystem particles;
    private Collider objectCollider;
    private AudioSource audioSource;

    //Awake is executed before the Start method
    private void Awake()
    {
        //Create necessary component references
        if(TryGetComponent(out audioSource) == false) 
        {
            Debug.LogWarning($"{name} needs an Audio Source attached to it!");
        }
        if(TryGetComponent(out objectCollider) == false) 
        {
            Debug.LogWarning($"{name} needs a Collider attached to it!");
        }

        particles = GetComponentInChildren<ParticleSystem>();
        if(particles == null) 
        {
            Debug.LogWarning($"{name} should have a particle system as a child!");
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (objectCollider != null)
        {
            objectCollider.enabled = true;
        }
        if (particles != null)
        {
            particles.Play();
        }
    }

    /// <summary>
    /// Disables the currently active waypoint, then moves the player to the position of this waypoint.
    /// Stops the particles and disables this objects collider.
    /// </summary>
    /// <returns></returns>
    public override bool Activate()
    {
        if (Interaction.Instance.CurrentWaypoint != this)
        {
            if (Interaction.Instance.CurrentWaypoint != null)
            {
                Interaction.Instance.CurrentWaypoint.Deactivate();
            }
            Interaction.Instance.CurrentWaypoint = this;
            Interaction.Instance.transform.parent.position = transform.position;
            if (objectCollider != null)
            {
                objectCollider.enabled = false;
            }
            if (particles != null)
            {
                particles.Stop();
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
    /// Disables the currently active tooltip (if there is one) then re-enables this object's collider and starts the particles playing.
    /// </summary>
    /// <returns></returns>
    public override bool Deactivate()
    {
        if (Interaction.Instance.CurrentWaypoint == this)
        {
            if (Interaction.Instance.CurrentTooltip != null)
            {
                Interaction.Instance.CurrentTooltip.Deactivate();
            }
            Interaction.Instance.CurrentWaypoint = null;
            if (objectCollider != null)
            {
                objectCollider.enabled = true;
            }
            if (particles != null)
            {
                particles.Play();
            }
            return true;
        }
        return false;
    }
}
