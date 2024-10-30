using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DynamicSpookyProfile : MonoBehaviour
{
    public Transform player;
    public Transform monster;
    public Volume postProcessVolume;

    private DepthOfField depthOfField;

    public float maxDistance = 15;

    void Start()
    {
        // Check if Depth of Field is set in the post-processing profile
        if (postProcessVolume.profile.TryGet<DepthOfField>(out depthOfField))
        {
            depthOfField.active = true;
        }
        else
        {
            Debug.LogWarning("Depth of Field effect not found in the Post-Processing Volume.");
        }
    }

    void Update()
    {
        // Calculate the distance between the player and the monster
        float distance = Vector3.Distance(player.position, monster.position);

        if (distance < maxDistance)
        {
            float min = Mathf.Clamp(distance - 7, 0, maxDistance - 3);
            float max = Mathf.Clamp(distance - 3, 1, maxDistance);

            depthOfField.gaussianStart.Override(min);
            depthOfField.gaussianEnd.Override(max);
        }
    }
}
