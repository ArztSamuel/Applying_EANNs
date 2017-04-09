/// Author: Samuel Arzt
/// Date: March 2017

#region Includes
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
#endregion

/// <summary>
/// Class for drawing the connections between neurons of a neural network to GUI.
/// </summary>
public class UINeuralNetworkConnectionPanel : MonoBehaviour
{
    #region Members
    // References to be set in Unity Editor.
    [SerializeField]
    private List<Image> Connections;
    [SerializeField]
    private Color PositiveColor;
    [SerializeField]
    private Color NegativeColor;
    #endregion

    #region Methods
    /// <summary>
    /// Displays the connections of a neuron of one layer to all neurons of the next layer.
    /// </summary>
    /// <param name="neuronIndex">The neuron to display the connections of.</param>
    /// <param name="currentLayer">The layer of the neuron to be displayed.</param>
    /// <param name="nextLayer">The layer the neuron to be displayed is connected to.</param>
    public void DisplayConnections(int neuronIndex, NeuralLayer currentLayer, UINeuralNetworkLayerPanel nextLayer)
    {
        //Set dummyConnection to active again, just in case it was hidden at some point.
        Image dummyConnection = Connections[0];
        dummyConnection.gameObject.SetActive(true);

        //Duplicate dummyConnection
        for (int i = Connections.Count; i < currentLayer.OutputCount; i++)
        {
            Image newConnection = Instantiate(dummyConnection);
            newConnection.transform.SetParent(this.transform, false);
            Connections.Add(newConnection);
        }

        //Destory all unnecessary connections
        for (int i = this.Connections.Count - 1; i >= currentLayer.OutputCount; i++)
        {
            Image toBeDestroyed = Connections[i];
            Connections.RemoveAt(i);
            Destroy(toBeDestroyed);
        }


        //Position connections
        for (int i = 0; i < Connections.Count; i++)
            PositionConnection(Connections[i], nextLayer.Nodes[i], neuronIndex, i, currentLayer.Weights);

    }

    /// <summary>
    /// Hides all connections.
    /// </summary>
    public void HideConnections()
    {
        //Destory all but dummy connection
        for (int i = this.Connections.Count - 1; i >= 1; i++)
        {
            Image toBeDestroyed = Connections[i];
            Connections.RemoveAt(i);
            Destroy(toBeDestroyed);
        }

        //Hide dummy connection
        Connections[0].gameObject.SetActive(false);
    }
	
    // Method for positioning the connection correctly (a bit tricky to draw lines in GUI with unity)
    private void PositionConnection(Image connection, UINeuralNetworkConnectionPanel otherNode, int nodeIndex, int connectedNodeIndex, double[,] weights)
    {
        //Set local position to 0
        connection.transform.localPosition = Vector3.zero;

        //Set connection width
        Vector2 sizeDelta = connection.rectTransform.sizeDelta;
        double weight = weights[nodeIndex, connectedNodeIndex];
        sizeDelta.x = (float) System.Math.Abs(weight);
        if (sizeDelta.x < 1)
            sizeDelta.x = 1;

        //Set conenction color
        if (weight >= 0)
            connection.color = PositiveColor;
        else
            connection.color = NegativeColor;

        //Set connection length (height)
        Vector2 connectionVec = this.transform.position - otherNode.transform.position;
        sizeDelta.y = connectionVec.magnitude / GameStateManager.Instance.UIController.Canvas.scaleFactor;

        connection.rectTransform.sizeDelta = sizeDelta;

        //Set connection rotation
        float angle = Vector2.Angle(Vector2.up, connectionVec);
        connection.transform.rotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));

    }
    #endregion
}
