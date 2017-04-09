/// Author: Samuel Arzt
/// Date: March 2017

#region Includes
using UnityEngine;
using UnityEngine.UI;
#endregion

/// <summary>
/// Class for controlling the overall GUI.
/// </summary>
public class UIController : MonoBehaviour
{
    #region Members
    /// <summary>
    /// The parent canvas of all UI elements.
    /// </summary>
    public Canvas Canvas
    {
        get;
        private set;
    }

    private UISimulationController simulationUI;
    private UIStartMenuController startMenuUI;
    #endregion

    #region Constructors
    void Awake()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.UIController = this;

        Canvas = GetComponent<Canvas>();
        simulationUI = GetComponentInChildren<UISimulationController>(true);
        startMenuUI = GetComponentInChildren<UIStartMenuController>(true);

        simulationUI.Show();
    }
    #endregion

    #region Methods
    /// <summary>
    /// Sets the CarController from which to get the data from to be displayed.
    /// </summary>
    /// <param name="target">The CarController to display the data of.</param>
    public void SetDisplayTarget(CarController target)
    {
        simulationUI.Target = target;
    }
    #endregion
}
