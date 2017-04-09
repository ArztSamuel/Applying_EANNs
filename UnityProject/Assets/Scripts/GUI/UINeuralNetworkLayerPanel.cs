/// Author: Samuel Arzt
/// Date: March 2017

#region Includes
using UnityEngine;
using System.Collections.Generic;
#endregion

/// <summary>
/// Class for displaying a layer of a neural network on GUI.
/// </summary>
public class UINeuralNetworkLayerPanel : MonoBehaviour
{
    #region Members
    // References to be set in Unity Editor.
    [SerializeField]
    private RectTransform LayerContents;
    [SerializeField]
    public List<UINeuralNetworkConnectionPanel> Nodes;
    #endregion

    #region Methods
    /// <summary>
    /// Display the given neural network layer on GUI.
    /// </summary>
    /// <param name="layer">The layer to be displayed.</param>
    public void Display(NeuralLayer layer)
    {
        Display(layer.NeuronCount);
    }

    /// <summary>
    /// Displays given amount of neurons in this layer.
    /// </summary>
    /// <param name="neuronCount">The amount of neurons to be displayed for this layer.</param>
    public void Display(uint neuronCount)
    {
        UINeuralNetworkConnectionPanel dummyNode = Nodes[0];

        //Duplicate dummyNode
        for (int i = Nodes.Count; i < neuronCount; i++)
        {
            UINeuralNetworkConnectionPanel newNode = Instantiate(dummyNode);
            newNode.transform.SetParent(LayerContents.transform, false);
            Nodes.Add(newNode);
        }

        //Destory all unnecessary nodes
        for (int i = this.Nodes.Count - 1; i >= neuronCount; i++)
        {
            UINeuralNetworkConnectionPanel toBeDestroyed = Nodes[i];
            Nodes.RemoveAt(i);
            Destroy(toBeDestroyed);
        }
    }

    /// <summary>
    /// Displays all connections from between the given two layers.
    /// </summary>
    /// <param name="currentLayer">The layer that is connected to the other layer.</param>
    /// <param name="nextLayer">The layer that the other layer is connected to.</param>
    public void DisplayConnections(NeuralLayer currentLayer, UINeuralNetworkLayerPanel nextLayer)
    {
        for (int i = 0; i<Nodes.Count; i++)
            Nodes[i].DisplayConnections(i, currentLayer, nextLayer);
    }

    /// <summary>
    /// Hides all connections that are currently being drawn.
    /// </summary>
    public void HideAllConnections()
    {
        foreach (UINeuralNetworkConnectionPanel node in Nodes)
            node.HideConnections();
    }
    #endregion
}
