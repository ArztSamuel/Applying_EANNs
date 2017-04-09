/// Author: Samuel Arzt
/// Date: March 2017

#region Includes
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion

/// <summary>
/// Class for displaying a neural networks topology.
/// </summary>
public class UINeuralNetworkPanel : MonoBehaviour
{
    #region Members
    // First (dummy) layer set by Unity editor
    [SerializeField]
    private List<UINeuralNetworkLayerPanel> Layers;
    #endregion

    #region Methods
    /// <summary>
    /// Displays the given neural network.
    /// </summary>
    /// <param name="neuralNet">The neural network to be displayed.</param>
    public void Display(NeuralNetwork neuralNet)
    {
        UINeuralNetworkLayerPanel dummyLayer = Layers[0];

        //Duplicate dummyLayer
        for (int i = Layers.Count; i < neuralNet.Layers.Length + 1; i++)
        {
            UINeuralNetworkLayerPanel newPanel = Instantiate(dummyLayer);
            newPanel.transform.SetParent(this.transform, false);
            Layers.Add(newPanel);
        }

        //Destory all unnecessary layers
        for (int i = this.Layers.Count-1; i >= neuralNet.Layers.Length + 1; i++)
        {
            UINeuralNetworkLayerPanel toBeDestroyed = Layers[i];
            Layers.RemoveAt(i);
            Destroy(toBeDestroyed);
        }
        
        //Set layer contents
        for (int l = 0; l<this.Layers.Count - 1; l++)
            this.Layers[l].Display(neuralNet.Layers[l]);

        this.Layers[Layers.Count - 1].Display(neuralNet.Layers[neuralNet.Layers.Length - 1].OutputCount);

        StartCoroutine(DrawConnections(neuralNet));
    }

    // Draw the connections (coroutine).
    private IEnumerator DrawConnections(NeuralNetwork neuralNet)
    {
        yield return new WaitForEndOfFrame();

        //Draw node connections
        for (int l = 0; l < this.Layers.Count - 1; l++)
            this.Layers[l].DisplayConnections(neuralNet.Layers[l], this.Layers[l + 1]);
        
        this.Layers[this.Layers.Count - 1].HideAllConnections();

    }
    #endregion
}
