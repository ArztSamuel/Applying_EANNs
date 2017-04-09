/// Author: Samuel Arzt
/// Date: March 2017

#region Includes
using UnityEngine;
using System.Collections;
#endregion

/// <summary>
/// Class representing a checkpoint of a race track.
/// </summary>
public class Checkpoint : MonoBehaviour
{
    #region Members
    /// <summary>
    /// The radius in Unity units in which this checkpoint can be captured.
    /// </summary>
    public float CaptureRadius = 3;
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// The reward value earned by capturing this checkpoint.
    /// </summary>
    public float RewardValue
    {
        get;
        set;
    }

    /// <summary>
    /// The distance in Unity units to the previous checkpoint on the track.
    /// </summary>
    public float DistanceToPrevious
    {
        get;
        set;
    }

    /// <summary>
    /// The accumulated distance in Unity units from the first to this checkpoint.
    /// </summary>
    public float AccumulatedDistance
    {
        get;
        set;
    }

    /// <summary>
    /// The accumulated reward earned for capturing all checkpoints from the first to this one.
    /// </summary>
    public float AccumulatedReward
    {
        get;
        set;
    }

    /// <summary>
    /// Whether or not this checkpoint is being drawn to screen.
    /// </summary>
    public bool IsVisible
    {
        get { return spriteRenderer.enabled; }
        set { spriteRenderer.enabled = value; }
    }
    #endregion

    #region Constructors
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    #endregion

    #region Methods
    /// <summary>
    /// Calculates the reward earned for the given distance to this checkpoint.
    /// </summary>
    /// <param name="currentDistance">The distance to this checkpoint.</param>
    public float GetRewardValue(float currentDistance)
    {
        //Calculate how close the distance is to capturing this checkpoint, relative to the distance from the previous checkpoint
        float completePerc = (DistanceToPrevious - currentDistance) / DistanceToPrevious; 

        //Reward according to capture percentage
        if (completePerc < 0)
            return 0;
        else return completePerc * RewardValue;
    }
    #endregion
}
